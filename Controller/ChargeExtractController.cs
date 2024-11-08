﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model.ChargeExtract;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions.Controller
{
    public interface IChargeExtractController
    {
        public ChargeExtractResponseDto? CreateChargeExtract(CreateChargeExtractRequestDto chargeExtractReq);
        public ChargeExtractResponseDto GetChargeExtract(string chargeExtractId);
        public bool InvoiceAlreadyExtracted(string invoiceId, string contractNumber);
        public bool InvoiceHasSESNumber(string invoiceId, string contractNumber);
    }
    public class ChargeExtractController : IChargeExtractController
    {
        ApplicationDBContext _dbContext;
        ILogger _logger;
        IMapper _mapper;
        CreateChargeExtractRequestDto _requestDto;
        ChargeExtractResponseDto _responseDto;
        List<InvoiceOtherCostDetails> _otherCosts;
        List<InvoiceTimeReportCostDetails> _timeReportCosts;
        List<ChargeExtractRowDto> _unGroupedRows;
        List<Invoice> _updatedInvoices;
        List<ChargeExtract> _newChargeExtracts;
        List<string> _filesPutInAzureStorage;
        List<ChargeExtractFileDto> _extractFiles;
        List<ChargeExtractDto> _extendedExtract;
        private readonly IWildfireFinanceService _wildfireFinanceService;

        int _maxNumberOfCostItems = 995;
        StringBuilder _output;
        bool _abort = false;

        public ChargeExtractController(ILogger log, IMapper mapper, IWildfireFinanceService wildfireFinanceService, ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            _responseDto = new ChargeExtractResponseDto();
            _otherCosts = new List<InvoiceOtherCostDetails>();
            _timeReportCosts = new List<InvoiceTimeReportCostDetails>();
            _unGroupedRows = new List<ChargeExtractRowDto>();

            _updatedInvoices = new List<Invoice>();
            _newChargeExtracts = new List<ChargeExtract>();
            _filesPutInAzureStorage = new List<string>();
            _extractFiles = new List<ChargeExtractFileDto>();
            _extendedExtract = new List<ChargeExtractDto>();
            _wildfireFinanceService = wildfireFinanceService;

            _logger = log;
            _mapper = mapper;
        }

        /// <summary>
        /// Main method to CreateChargeExtract
        /// </summary>
        /// <param name="chargeExtractReq"></param>
        /// <returns>ChargeExtractResponseDto?</returns>
        public ChargeExtractResponseDto? CreateChargeExtract(CreateChargeExtractRequestDto chargeExtractReq)
        {
            bool _singleFileExtract = true;
            _requestDto = chargeExtractReq;

            // invoices to process
            var _invoicesToProcess = _dbContext.Invoice.Include(p => p.InvoiceOtherCostDetails).Include(p => p.InvoiceTimeReportCostDetails)
                                    .Where(p => chargeExtractReq.Invoices.Contains(p.InvoiceId.ToString())
                                            && p.ContractNumber == chargeExtractReq.ContractNumber).ToList();

            // vendor list we need to process because extracts are generated by vendors
            var _vendors = _invoicesToProcess.Select(p => p.VendorBusinessId).Distinct().ToList();
            foreach (var vendor in _vendors)
            {
                try
                {
                    var _vendorInvoices = _invoicesToProcess.Where(p => p.VendorBusinessId == vendor).ToList();
                    _vendorInvoices.ForEach(p =>
                    {
                        if (p.InvoiceOtherCostDetails.Count > 0)
                            _otherCosts.AddRange(p.InvoiceOtherCostDetails);

                        if (p.InvoiceTimeReportCostDetails.Count > 0)
                            _timeReportCosts.AddRange(p.InvoiceTimeReportCostDetails);

                        _unGroupedRows.AddRange(_timeReportCosts.Where(param => param.InvoiceId == p.InvoiceId).Select(q => new ChargeExtractRowDto()
                        { InvoiceNumber = p.InvoiceNumber, InvoiceAmount = q.Cost, CostCenter = q.CostCenter, InternalOrder = q.InternalOrder, Fund = q.Fund }));

                        _unGroupedRows.AddRange(_otherCosts.Where(param => param.InvoiceId == p.InvoiceId).Select(r => new ChargeExtractRowDto()
                        { InvoiceNumber = p.InvoiceNumber, InvoiceAmount = r.Cost, CostCenter = r.CostCentre, InternalOrder = r.InternalOrder, Fund = r.Fund }));
                    });

                    var _groupedRows = _unGroupedRows.GroupBy(x => new { x.InvoiceNumber, x.CostCenter, x.InternalOrder, x.Fund })
                                                    .Select(y => new { id = y.Key, total = y.Sum(x => x.InvoiceAmount) });

                    if (_groupedRows.Count() > _maxNumberOfCostItems)
                        _singleFileExtract = false;

                    if (_singleFileExtract)
                        _abort = !ProcessSingleFileExtract(vendor, _vendorInvoices);
                    else
                        _abort = !ProcessMultiFileExtract(vendor, _vendorInvoices);

                    _otherCosts = new List<InvoiceOtherCostDetails>();
                    _timeReportCosts = new List<InvoiceTimeReportCostDetails>();

                    if (_abort)
                        break;
                }
                catch (Exception ex)
                {
                    _abort = true;
                    _logger.LogError(string.Format("CreateChargeExtract: An error has occured while creating extract for Vendor: {0}, ErrorMessage: {1}, InnerException: {2}", vendor, ex.Message, ex.InnerException));
                }
            } // End of Foreach of Vendors

            // Clean any data if created in the database and throw back null
            if (_abort)
            {
                //Delete files from Azure
                AzureStorageController azureStorageController = new AzureStorageController(_logger, _mapper);
                foreach (var item in _filesPutInAzureStorage)
                {
                    bool success = azureStorageController.CheckFileExistsAsync(item).GetAwaiter().GetResult();
                    if (success)
                    {
                        _logger.LogInformation(string.Format("File found: {0}", item));
                        success = azureStorageController.DeleteFileAsync(item).GetAwaiter().GetResult();
                        if (success)
                            _logger.LogInformation(string.Format("File : {0} ---- deleted from server.", item));
                    }
                    else
                    {
                        _logger.LogError(string.Format("File : {0}  --- not found on server.", item));
                    }
                }

                // Remove records from database
                IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();

                foreach (var item in _updatedInvoices)
                {
                    item.ChargeExtractId = null;
                    _dbContext.Invoice.Update(item);
                    _dbContext.SaveChanges();
                }

                _newChargeExtracts.ForEach(item =>
                {
                    _dbContext.ChargeExtract.Remove(item);
                    _dbContext.SaveChanges();
                });
                transaction.Commit();

                _responseDto = null;
            }

            return _responseDto;
        }

        /// <summary>
        /// Charge Extract - Multi File Scenario
        /// </summary>
        /// <param name="vendor"></param>
        /// <param name="invoices"></param>
        /// <returns>bool</returns>
        private bool ProcessMultiFileExtract(string vendor, List<Invoice> invoices)
        {
            bool bResult = true;
            var parentExtractInvoice = invoices.FirstOrDefault();
            _logger.LogInformation(string.Format("CreateChargeExtract:ProcessMultiFileExtract - " +
                        "Started processing multi file Charge Extract for Vendor: {0} ", vendor));
            try
            {
                var parentChargeExtractDto = ProcessInvoiceForMultiFileExtract(vendor, parentExtractInvoice, null);

                if (parentChargeExtractDto != null)
                {
                    _extractFiles.Add(new ChargeExtractFileDto() { ExtractFile = parentChargeExtractDto.ExtractFile, ExtractFileName = parentChargeExtractDto.ChargeExtractFileName });
                    foreach (var invoice in invoices.Where(p => p.InvoiceId != parentExtractInvoice.InvoiceId))
                    {
                        var chargeExtractDto = ProcessInvoiceForMultiFileExtract(vendor, invoice, parentChargeExtractDto.ChargeExtractId);
                        if (chargeExtractDto != null)
                        {
                            _extendedExtract.Add(chargeExtractDto);
                            _extractFiles.Add(new ChargeExtractFileDto()
                            {
                                ExtractFile = chargeExtractDto.ExtractFile,
                                ExtractFileName = chargeExtractDto.ChargeExtractFileName
                            });
                        }
                        else
                        {
                            _logger.LogError(string.Format("CreateChargeExtract:ProcessMultiFileExtract - Failed to create Charge Extract for Vendor: " +
                                "{0} - Invoice {1} ", vendor, invoice.InvoiceNumber));
                            bResult = false;
                            break;
                        }
                    }
                    parentChargeExtractDto.ExtractFiles = _extractFiles;
                    parentChargeExtractDto.ExtendedExtract = _extendedExtract;
                    _responseDto.ChargeExtract = parentChargeExtractDto;
                }
                else
                {
                    _logger.LogError(string.Format("CreateChargeExtract:ProcessMultiFileExtract - Failed to create Charge Extract for Vendor: {0} ", vendor));
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("CreateChargeExtract:ProcessMultiFileExtract - An error has occured while creating extract for Vendor: {0} " +
                                                   "ErrorMessage: {1}, InnerException: {2}", vendor, ex.Message, ex.InnerException));
            }

            return bResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vendor"></param>
        /// <param name="invoice"></param>
        /// <param name="parentChargeExtractId"></param>
        /// <returns>ChargeExtractDto</returns>
        private ChargeExtractDto ProcessInvoiceForMultiFileExtract(string vendor, Invoice invoice, Guid? parentChargeExtractId = null)
        {
            var _groupedRows = _unGroupedRows.Where(p => p.InvoiceNumber == invoice.InvoiceNumber)
                                                .GroupBy(x => new { x.InvoiceNumber, x.CostCenter, x.InternalOrder, x.Fund })
                                                .Select(y => new { id = y.Key, total = y.Sum(x => x.InvoiceAmount) });
            var vendorName = invoice.VendorName;
            var contractNumber = invoice.ContractNumber;
            decimal grandTotal = _groupedRows.Sum(x => x.total);


            var resp = _wildfireFinanceService.GetFinanceDocuments(new Core.Model.FinanceDocument.FinanceDocumentRequestDto
            {
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceAmount = invoice.InvoiceAmount.Value,
                VendorBusinessId = invoice.VendorBusinessId
            }).Result?.Data?.FirstOrDefault();
            string documentOrVoucherId = resp == null ? string.Empty : resp.AccountingDocument;

            ChargeExtractDto chargeExtractDto = null;
            _output = new StringBuilder();
            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();

            try
            {
                //First Row
                _output.Append(GetFirstRow());
                _output.Append("\r\n");

                // Main Header Row
                _output.Append(GetHeaderRow());
                _output.Append("\r\n");

                // Detail Row Header
                _output.Append(GetDetailHeaderRow());
                _output.Append("\r\n");

                // Main Header Row Data
                var formattedVendorInfo = vendorName + " " + vendor;
                _output.Append(GetHeaderDataRow(_requestDto.ChargeExtractDateTime, formattedVendorInfo));
                _output.Append("\r\n");

                //Detail Row Header Data
                _output.Append(GetDetailHeaderDataRow(grandTotal));
                _output.Append("\r\n");

                // Get All break down rows
                foreach (var item in _groupedRows)
                {
                    _output.Append(GetDetailItemDataRow(item.id.InvoiceNumber, item.total, item.id.CostCenter, item.id.InternalOrder, item.id.Fund, vendor, documentOrVoucherId));
                    _output.Append("\r\n");
                }

                // Create file in memory
                byte[] byteArray = Encoding.ASCII.GetBytes(_output.ToString());
                MemoryStream stream = new MemoryStream(byteArray);

                // Write it to Azure
                string fileName = vendor + "." + DateTime.UtcNow.ToString("dd.MM.yyyy hh.mm.ss.ffff") + ".csv";
                AzureStorageController azureStorageController = new AzureStorageController(_logger, _mapper);
                bool success = azureStorageController.CheckFileExistsAsync(fileName).GetAwaiter().GetResult();
                if (success)
                {
                    _logger.LogInformation(string.Format("File already exists: {0}", fileName));
                    return null;
                }
                else
                {
                    success = azureStorageController.UploadFileAsync(fileName, byteArray.ToString()).GetAwaiter().GetResult();
                    if (success)
                    {
                        _logger.LogInformation(string.Format("File Uploaded: {0}", fileName));
                        _filesPutInAzureStorage.Add(fileName);
                    }
                }

                // Create transactions in database
                chargeExtractDto = new ChargeExtractDto();
                ChargeExtractDetailDto chargeExtractDetailDto;
                List<ChargeExtractDetailDto> chargeExtractDetailDtos = new List<ChargeExtractDetailDto>();

                chargeExtractDto.ChargeExtractDateTime = _requestDto.ChargeExtractDateTime;
                chargeExtractDto.AuditCreationDateTime = DateTime.UtcNow;
                chargeExtractDto.RequestedBy = _requestDto.RequestedBy;
                chargeExtractDto.ChargeExtractFileName = fileName;
                chargeExtractDto.VendorId = vendor;
                chargeExtractDto.AuditLastUpdatedDateTime = DateTime.UtcNow;
                chargeExtractDto.ParentChargeExtractId = parentChargeExtractId;

                chargeExtractDetailDto = new ChargeExtractDetailDto();
                chargeExtractDetailDto.InvoiceId = invoice.InvoiceId;
                chargeExtractDetailDto.AuditCreationDateTime = DateTime.UtcNow;
                chargeExtractDetailDto.AuditLastUpdatedBy = _requestDto.RequestedBy;
                chargeExtractDetailDto.AuditLastUpdatedDateTime = DateTime.UtcNow;
                chargeExtractDetailDtos.Add(chargeExtractDetailDto);

                chargeExtractDto.ChargeExtractDetail = chargeExtractDetailDtos;
                ChargeExtract CEEntity = _mapper.Map<ChargeExtract>(chargeExtractDto);
                _dbContext.ChargeExtract.Add(CEEntity);
                _dbContext.SaveChanges();

                _newChargeExtracts.Add(CEEntity);

                invoice.ChargeExtractId = CEEntity.ChargeExtractId;
                _dbContext.Invoice.Update(invoice);
                _dbContext.SaveChanges();

                chargeExtractDto = _mapper.Map<ChargeExtractDto>(CEEntity);
                chargeExtractDto.ExtractFile = JsonConvert.SerializeObject(Convert.ToBase64String(byteArray));

                transaction.Commit();
            }
            catch (Exception ex)
            {
                chargeExtractDto = null;
                _logger.LogError(string.Format("CreateChargeExtract:ProcessInvoiceForMultiFileExtract - An error has occured while creating extract for Vendor: {0} - Invoice {1}, " +
                                               "ErrorMessage: {2}, InnerException: {3}", vendor, invoice.InvoiceNumber, ex.Message, ex.InnerException));
            }


            return chargeExtractDto;
        }

        /// <summary>
        /// Charge Extract - Single File Scenario
        /// </summary>
        /// <param name="vendor"></param>
        /// <param name="invoices"></param>
        /// <returns></returns>
        private bool ProcessSingleFileExtract(string vendor, List<Invoice> invoices)
        {
            var _groupedRows = _unGroupedRows.GroupBy(x => new { x.InvoiceNumber, x.CostCenter, x.InternalOrder, x.Fund })
                                                        .Select(y => new { id = y.Key, total = y.Sum(x => x.InvoiceAmount) });

            var itemForDetail = invoices.FirstOrDefault();
            var vendorName = itemForDetail.VendorName;
            var contractNumber = itemForDetail.ContractNumber;
            decimal grandTotal = _groupedRows.Sum(x => x.total);
            bool result = true;

            _output = new StringBuilder();
            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();

            try
            {
                //First Row
                _output.Append(GetFirstRow());
                _output.Append("\r\n");

                // Main Header Row
                _output.Append(GetHeaderRow());
                _output.Append("\r\n");

                // Detail Row Header
                _output.Append(GetDetailHeaderRow());
                _output.Append("\r\n");

                // Main Header Row Data
                var formattedVendorInfo = vendorName + " " + vendor;
                _output.Append(GetHeaderDataRow(_requestDto.ChargeExtractDateTime, formattedVendorInfo));
                _output.Append("\r\n");

                //Detail Row Header Data
                _output.Append(GetDetailHeaderDataRow(grandTotal));
                _output.Append("\r\n");

                // Get All break down rows
                var prevInvoiceNumber = _groupedRows.FirstOrDefault().id.InvoiceNumber;
                var previousInvoice = invoices.Where(p => p.InvoiceNumber == prevInvoiceNumber).FirstOrDefault();
                var resp = _wildfireFinanceService.GetFinanceDocuments(new Core.Model.FinanceDocument.FinanceDocumentRequestDto
                {
                    InvoiceNumber = prevInvoiceNumber,
                    InvoiceAmount = previousInvoice.InvoiceAmount.Value,
                    VendorBusinessId = previousInvoice.VendorBusinessId
                }).Result?.Data?.FirstOrDefault();
                string documentOrVoucherId = resp == null ? string.Empty : resp.AccountingDocument;
                foreach (var item in _groupedRows)
                {
                    if (item.id.InvoiceNumber != prevInvoiceNumber)
                    {
                        //_output.Append(GetBlankRow());
                        //_output.Append("\r\n");
                        prevInvoiceNumber = item.id.InvoiceNumber;
                        previousInvoice = invoices.Where(p => p.InvoiceNumber == prevInvoiceNumber).FirstOrDefault();
                        resp = _wildfireFinanceService.GetFinanceDocuments(new Core.Model.FinanceDocument.FinanceDocumentRequestDto
                        {
                            InvoiceNumber = prevInvoiceNumber,
                            InvoiceAmount = previousInvoice.InvoiceAmount.Value,
                            VendorBusinessId = previousInvoice.VendorBusinessId
                        }).Result?.Data?.FirstOrDefault();
                        documentOrVoucherId = resp == null ? string.Empty : resp.AccountingDocument;
                    }
                    _output.Append(GetDetailItemDataRow(item.id.InvoiceNumber, item.total, item.id.CostCenter, item.id.InternalOrder, item.id.Fund, contractNumber, documentOrVoucherId));
                    _output.Append("\r\n");
                }

                // Create file in memory
                byte[] byteArray = Encoding.ASCII.GetBytes(_output.ToString());
                MemoryStream stream = new MemoryStream(byteArray);

                // Write it to Azure
                string fileName = vendor + "." + DateTime.UtcNow.ToString("dd.MM.yyyy hh.mm.ss.ffff") + ".csv";
                AzureStorageController azureStorageController = new AzureStorageController(_logger, _mapper);
                bool success = azureStorageController.CheckFileExistsAsync(fileName).GetAwaiter().GetResult();
                if (success)
                {
                    _logger.LogInformation(string.Format("File already exists: {0}", fileName));
                    result = false;
                    return result;
                }
                else
                {
                    success = azureStorageController.UploadFileAsync(fileName, byteArray.ToString()).GetAwaiter().GetResult();
                    if (success)
                    {
                        _logger.LogInformation(string.Format("File Uploaded: {0}", fileName));
                        _filesPutInAzureStorage.Add(fileName);
                    }
                }

                // Create transactions in database
                ChargeExtractDto chargeExtractDto = new ChargeExtractDto();
                ChargeExtractDetailDto chargeExtractDetailDto;
                List<ChargeExtractDetailDto> chargeExtractDetailDtos = new List<ChargeExtractDetailDto>();

                chargeExtractDto.ChargeExtractDateTime = _requestDto.ChargeExtractDateTime;
                chargeExtractDto.AuditCreationDateTime = DateTime.UtcNow;
                chargeExtractDto.RequestedBy = _requestDto.RequestedBy;
                chargeExtractDto.ChargeExtractFileName = fileName;
                chargeExtractDto.VendorId = vendor;
                chargeExtractDto.AuditLastUpdatedDateTime = DateTime.UtcNow;
                chargeExtractDto.ParentChargeExtractId = null;

                invoices.ForEach(p =>
                {
                    chargeExtractDetailDto = new ChargeExtractDetailDto();
                    chargeExtractDetailDto.InvoiceId = p.InvoiceId;
                    chargeExtractDetailDto.AuditCreationDateTime = DateTime.UtcNow;
                    chargeExtractDetailDto.AuditLastUpdatedBy = _requestDto.RequestedBy;
                    chargeExtractDetailDto.AuditLastUpdatedDateTime = DateTime.UtcNow;
                    chargeExtractDetailDtos.Add(chargeExtractDetailDto);
                });

                chargeExtractDto.ChargeExtractDetail = chargeExtractDetailDtos;
                ChargeExtract CEEntity = _mapper.Map<ChargeExtract>(chargeExtractDto);
                _dbContext.ChargeExtract.Add(CEEntity);
                _dbContext.SaveChanges();

                _newChargeExtracts.Add(CEEntity);

                invoices.ForEach(p =>
                {
                    p.ChargeExtractId = CEEntity.ChargeExtractId;
                    _dbContext.Invoice.Update(p);
                    _updatedInvoices.Add(p);
                });

                _dbContext.SaveChanges();

                _responseDto.ChargeExtract = _mapper.Map<ChargeExtractDto>(CEEntity);
                _responseDto.ChargeExtract.ExtractFile = JsonConvert.SerializeObject(Convert.ToBase64String(byteArray));
                _extractFiles.Add(new ChargeExtractFileDto()
                {
                    ExtractFile = _responseDto.ChargeExtract.ExtractFile,
                    ExtractFileName = _responseDto.ChargeExtract.ChargeExtractFileName
                });
                _responseDto.ChargeExtract.ExtractFiles = _extractFiles;

                // if all is good then create response object and commit chages
                transaction.Commit();
            }
            catch (Exception ex)
            {
                result = false;
                _logger.LogError(string.Format("CreateChargeExtract:ProcessSingleFileExtract An error has occured while creating extract for Vendor: {0}, ErrorMessage: {1}, InnerException: {2}", vendor, ex.Message, ex.InnerException));
                transaction.Rollback();
            }

            return result;
        }

        public ChargeExtractResponseDto GetChargeExtract(string chargeExtractId)
        {
            ChargeExtractResponseDto _response = new ChargeExtractResponseDto();

            return _response;
        }

        public bool InvoiceAlreadyExtracted(string invoiceId, string contractNumber)
        {
            bool bResult = false;
            try
            {
                Invoice invoice = _dbContext.Invoice.Where(x => x.InvoiceId.ToString() == invoiceId && x.ContractNumber == contractNumber).FirstOrDefault();
                if (invoice != null)
                {
                    if (invoice.ChargeExtractId != null)
                    {
                        bResult = true;
                    }
                }
            }
            catch
            {
                _logger.LogError("An error has occured while looking up invoice: " + invoiceId);
                throw;
            }

            return bResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="contractNumber"></param>
        /// <returns></returns>
        public bool InvoiceHasSESNumber(string invoiceId, string contractNumber)
        {
            bool bResult = false;
            
            try
            {
                Invoice invoice = _dbContext.Invoice.Where(x => x.InvoiceId.ToString() == invoiceId && x.ContractNumber == contractNumber).FirstOrDefault();
                if (invoice != null)
                {
                    if (invoice.UniqueServiceSheetName.Trim().Length > 0)
                    {
                        bResult = true;
                    }
                }
            }
            catch
            {
                _logger.LogError("An error has occured while looking up invoice: " + invoiceId);
                throw;
            }
            return bResult;
        }

        #region create csv rows
        private StringBuilder GetBlankRow()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + ""
                + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + ""
                + "," + "" + "," + "" + "," + "" + "," + "" + ",");
            return sb;
        }
        private StringBuilder GetFirstRow()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Journal Entry upload" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," +
                      "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," +
                      "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + ",");
            return sb;
        }
        private StringBuilder GetHeaderRow()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("HEADER (BH)" + ","  // Column A
                + "Company Code" + ","  // Column B
                + "Document Type" + ","  // Column C
                + "Document Date" + ","  // Column D
                + "Posting Date" + ","  // Column E
                + "Period" + ","  // Column F
                + "Fiscal Year" + ","  // Column G
                + "Ledger Group" + ","  // Column H
                + "Currency Code" + ","  // Column I
                + "Exchange Rate" + ","  // Column J
                + "Reference" + ","  // Column K
                + "Document Header Text" + ","  // Column L
                + "Document Number" + ","  // Column M
                + "" + ","  // Column N
                + "" + ","  // Column O
                + "" + ","  // Column P
                + "" + ","  // Column Q
                + "" + ","  // Column R
                + "" + ","  // Column S
                + "" + ","  // Column T
                + "" + ","  // Column U
                + "" + ","  // Column V
                + "" + ","  // Column W
                + "" + ","  // Column X
                + "" + ","  // Column Y
                + "" + ","  // Column Z
                + "" + ","  // Column AA
                + "" + ","  // Column AB
                + "" + ","  // Column AC
                + "" + ","); // Column AD
            return sb;
        }
        private StringBuilder GetHeaderDataRow(DateTime documentDate, string documentHeaderText)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BH" + ","  // Column A
                + "1000" + ","  // Column B
                + "SA" + ","  // Column C
                + documentDate.Day.ToString("00") + "." + documentDate.Month.ToString("00") + "." + documentDate.Year + ","  // Column D
                + "" + ","  // Column E
                + "" + ","  // Column F
                + "" + ","  // Column G
                + "" + ","  // Column H
                + "CAD" + ","  // Column I
                + "" + ","  // Column J
                + "" + ","  // Column K
                + documentHeaderText + ","  // Column L
                + "" + ","  // Column M
                + "" + ","  // Column N
                + "" + ","  // Column O
                + "" + ","  // Column P
                + "" + ","  // Column Q
                + "" + ","  // Column R
                + "" + ","  // Column S
                + "" + ","  // Column T
                + "" + ","  // Column U
                + "" + ","  // Column V
                + "" + ","  // Column W
                + "" + ","  // Column X
                + "" + ","  // Column Y
                + "" + ","  // Column Z
                + "" + ","  // Column AA
                + "" + ","  // Column AB
                + "" + ","  // Column AC
                + "" + ","); // Column AD
            return sb;
        }
        private StringBuilder GetDetailHeaderRow()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DETAIL(BD)" + "," // Column A
                + "Posting Key" + "," // Column B
                + "Account" + "," // Column C
                + "Special GL Indiactor" + "," // Column D
                + "Amount" + "," // Column E
                + "Amount in LC" + "," // Column F
                + "Tax Code" + "," // Column G
                + "Tax Jurisdiction" + "," // Column H
                + "Transaction Type" + "," // Column I
                + "Cost Centre" + "," // Column J
                + "Internal Order" + "," // Column K
                + "Profit Centre" + "," // Column L
                + "WBS" + "," // Column M
                + "Fund" + "," // Column N
                + "Fund Centre" + "," // Column O
                + "Commitment Item" + "," // Column P
                + "Earmarked Funds" + "," // Column Q
                + "Earmarked Funds :Document Item" + "," // Column R
                + "Asset" + "," // Column S
                + "Asset Sub Number" + "," // Column T
                + "Quantity" + "," // Column U
                + "Personnel Number" + "," // Column V
                + "Assignment" + "," // Column W
                + "Text" + "," // Column X
                + "Reference Key 1" + "," // Column Y
                + "Reference Key 2" + "," // Column Z
                + "Reference Key 3" + "," // Column AA
                + "House Bank" + "," // Column AB
                + "Partner Profit Center" + "," // Column AC
                + "Functional Area" + ","); // Column AD
            return sb;
        }
        private StringBuilder GetDetailHeaderDataRow(decimal grandTotal)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BD" + "," + "50" + "," + "6020700210" + "," + "" + "," + grandTotal.ToString() + "," + "" + ","
                    + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + ","
                    + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + "," + "" + ",");
            return sb;
        }

        private StringBuilder GetDetailItemDataRow(string invoiceNumber, decimal amount, string cc, string io, string fund, string contractNumber, string documentOrVoucherId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BD" + ","  // Column A
                + "40" + "," // Column B
                + "6020700210" + "," // Column C
                + "" + "," // Column D
                + amount.ToString() + "," // Column E
                + "" + "," // Column F
                + "" + "," // Column G
                + "" + "," // Column H
                + "" + "," // Column I
                + cc + "," // Column J
                + io + "," // Column K
                + "" + "," // Column L
                + "" + "," // Column M
                + fund + "," // Column N
                + "" + "," // Column O
                + "" + "," // Column P
                + "" + "," // Column Q
                + "" + "," // Column R
                + "" + "," // Column S
                + "" + "," // Column T
                + "" + "," // Column U
                + "" + "," // Column V
                + "" + "," // Column W
                + invoiceNumber + "," // Column X
                + contractNumber + "," // Column Y
                + documentOrVoucherId + "," // Column Z
                + "Professional Service" + "," // Column AA
                + "" + "," // Column AB
                + "" + "," // Column AC
                + "" + ","); // Column AD
            return sb;
        }

        #endregion

    }
}
