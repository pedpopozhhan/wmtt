﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Validator;
using WCDS.WebFuncions.Enums;

namespace WCDS.WebFuncions.Controller
{
    public interface IInvoiceController
    {
        public Task<Guid> CreateInvoice(InvoiceDto invoice);
        public int UpdateInvoice(InvoiceDto invoice);
        public bool InvoiceExistsForContract(Guid? _invoiceId, string invoiceNumber, string contractNumber);
        public InvoiceResponseDto GetInvoices(GetInvoiceRequestDto invoiceRequest);
        public InvoiceResponseDto GetInvoicesWithDetails(GetInvoiceRequestDto invoiceRequest);
        public InvoiceDetailResponseDto GetInvoiceDetails(InvoiceDetailRequestDto invoiceDetailRequest);
        public Task<string> UpdateProcessedInvoice(InvoiceDto invoice);
        public Task<bool> UpdateInvoiceStatus(UpdateInvoiceStatusRequestDto request);
        public CostDetailsResponseDto GetCostDetails(CostDetailsRequestDto request);
        public Task<InvoiceDto> UpdateDraft(InvoiceRequestDto invoice, string user);
        public Task<InvoiceDto> CreateDraft(InvoiceRequestDto invoice, string user);
        public Task<Guid> DeleteDraft(Guid invoiceId, string user);
    }

    public class InvoiceController : IInvoiceController
    {
        ApplicationDBContext _dbContext;
        ILogger _logger;
        IMapper _mapper;
        private const string DEFAULT_USER = "System";
        private const string CONTRACTS_API_PATH_PROCESSEDINVOICE = "/ProcessedInvoice/{0}";

        public InvoiceController(ILogger log, IMapper mapper, ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            _logger = log;
            _mapper = mapper;
        }

        /*
        If we are here, there is no existing invoice
        */
        public async Task<Guid> CreateInvoice(InvoiceDto invoice)
        {
            Guid result = Guid.Empty;
            int numberOfCostDetails = 0, numberOfOtherCostDetails = 0;

            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();

            try
            {
                Invoice invoiceEntity = _mapper.Map<Invoice>(invoice);
                invoiceEntity.InvoiceStatus = InvoiceStatus.Processed.ToString();
                if (invoiceEntity.InvoiceTimeReportCostDetails != null && invoiceEntity.InvoiceTimeReportCostDetails.Count() > 0)
                {
                    foreach (var item in invoiceEntity.InvoiceTimeReportCostDetails)
                    {
                        item.CreatedBy = invoice.CreatedBy;
                        item.CreatedByDateTime = DateTime.UtcNow;
                    }
                    numberOfCostDetails = invoiceEntity.InvoiceTimeReportCostDetails.Count();
                }
                if (invoiceEntity.InvoiceOtherCostDetails != null && invoiceEntity.InvoiceOtherCostDetails.Count() > 0)
                {
                    foreach (var item in invoiceEntity.InvoiceOtherCostDetails)
                    {
                        item.CreatedBy = invoice.CreatedBy;
                        item.CreatedByDateTime = DateTime.UtcNow;
                    }
                    numberOfOtherCostDetails = invoiceEntity.InvoiceOtherCostDetails.Count();
                }
                invoiceEntity.InvoiceStatusLogs = new List<InvoiceStatusLog> { new InvoiceStatusLog()
                                        {
                                            InvoiceId = invoiceEntity.InvoiceId,
                                            User = invoice.CreatedBy,
                                            Timestamp = DateTime.UtcNow
                                        }};

                invoiceEntity.CreatedBy = invoice.CreatedBy;
                invoiceEntity.CreatedByDateTime = DateTime.UtcNow;

                _logger.LogInformation("Ivoice sent to DB for Insert at: {0} for invoice : {1}. Number of detail records in this invoice are: {2}",
                    DateTime.UtcNow, invoiceEntity.InvoiceNumber, numberOfCostDetails + numberOfOtherCostDetails);

                _dbContext.Invoice.Add(invoiceEntity);
                _dbContext.SaveChanges();

                _logger.LogInformation("Ivoice Insert Completed at: {0} for invoice: {1}", DateTime.UtcNow, invoiceEntity.InvoiceNumber);

                var entity = _dbContext.Invoice.Where(x => x.InvoiceId == invoice.InvoiceId)
                .Include(i => i.InvoiceTimeReportCostDetails)
                .Include(i => i.InvoiceOtherCostDetails)
                .Include(i => i.InvoiceStatusLogs)
                .Include(i => i.InvoiceTimeReports).FirstOrDefault();

                if (entity == null)
                {
                    throw new System.Exception($"No Invoice found for InvoiceId - {invoice.InvoiceId} in the Database.");
                }

                // update-invoice to data team

                // all the child records are inserts
                var dataPayload = CreateDataSyncInsertPayload(entity, entity.InvoiceTimeReportCostDetails, entity.InvoiceOtherCostDetails, entity.InvoiceTimeReports);
                await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(dataPayload, entity.InvoiceNumber, "insert");

                var toInsert = entity.InvoiceTimeReportCostDetails;
                var payload = CreateStatusSyncPayload(entity, "update-invoice", entity.InvoiceTimeReportCostDetails, new List<InvoiceTimeReportCostDetails>(), new List<InvoiceTimeReportCostDetails>());
                await new InvoiceStatusSyncMessageHandler(_logger).SendInvoiceStatusSyncMessage(payload, entity.InvoiceNumber);

                result = invoiceEntity.InvoiceId;
                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("CreateInvoice: An error has occured while Saving Invoice: {0}, ErrorMessage: {1}, InnerException: {2}", invoice.InvoiceNumber, ex.Message, ex.InnerException));
                transaction.Rollback();
                throw;
            }

            return result;
        }

        // Update this to properly update the entire invoice
        public async Task<string> UpdateProcessedInvoice(InvoiceDto invoice)
        {
            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var entity = _dbContext.Invoice.Where(x => x.InvoiceId == invoice.InvoiceId.Value).FirstOrDefault();
                if (entity == null)
                {
                    throw new System.Exception($"No Invoice found for InvoiceId - {invoice.InvoiceId} in the Database.");
                }
                entity.UniqueServiceSheetName = invoice.UniqueServiceSheetName;
                entity.UpdatedByDateTime = DateTime.UtcNow;
                entity.InvoiceStatus = InvoiceStatus.Processed.ToString();

                _dbContext.SaveChanges();
                transaction.Commit();

                var dataPayload = CreateDataSyncUpdatePayload(entity, new List<InvoiceTimeReportCostDetails>(), new List<InvoiceOtherCostDetails>(), new List<InvoiceTimeReports>());
                await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(dataPayload, entity.InvoiceNumber, "update");

                return entity.UniqueServiceSheetName;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("UpdateProcessedInvoice: An error has occured while Updating Invoice for Invoice Number:  {0}, ErrorMessage: {1}, InnerException: {2}", invoice.InvoiceNumber, ex.Message, ex.InnerException));
                transaction.Rollback();
                throw;
            }

        }


        public async Task<InvoiceDto> CreateDraft(InvoiceRequestDto invoice, string user)
        {

            var dt = DateTime.UtcNow;

            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
            try
            {
                if (!invoice.InvoiceId.HasValue || invoice.InvoiceId == Guid.Empty)
                {
                    // create
                    var entity = _mapper.Map<Invoice>(invoice);
                    entity.InvoiceStatus = InvoiceStatus.Draft.ToString();
                    entity.CreatedByDateTime = dt;
                    entity.CreatedBy = user;
                    entity.UpdatedByDateTime = dt;
                    entity.UpdatedBy = user;
                    _dbContext.Invoice.Add(entity);
                    SetCreatedFields(entity.InvoiceTimeReportCostDetails, user, dt);
                    SetCreatedFields(entity.InvoiceOtherCostDetails, user, dt);


                    _dbContext.SaveChanges();

                    entity.InvoiceStatusLogs = new List<InvoiceStatusLog> { new()
                    {
                        InvoiceId = entity.InvoiceId,
                        User = user,
                        Timestamp = dt
                    }};


                    var invoiceTimeReports = invoice.FlightReportIds.Select(x =>
                    {
                        return new InvoiceTimeReports
                        {
                            FlightReportId = x,
                            InvoiceId = entity.InvoiceId,
                            AuditCreationDateTime = dt,
                            AuditLastUpdatedDateTime = dt,
                            AuditLastUpdatedBy = user
                        };
                    });
                    _dbContext.InvoiceTimeReports.AddRange(invoiceTimeReports);
                    _dbContext.SaveChanges();
                    transaction.Commit();

                    //// insert-invoice to data team
                    var dataPayload = CreateDataSyncInsertPayload(entity, entity.InvoiceTimeReportCostDetails, entity.InvoiceOtherCostDetails, entity.InvoiceTimeReports);
                    await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(dataPayload, entity.InvoiceNumber, "insert");

                    var toInsert = entity.InvoiceTimeReportCostDetails;
                    var payload = CreateStatusSyncPayload(entity, "update-invoice", toInsert, new List<InvoiceTimeReportCostDetails>(), new List<InvoiceTimeReportCostDetails>());
                    await new InvoiceStatusSyncMessageHandler(_logger).SendInvoiceStatusSyncMessage(payload, entity.InvoiceNumber);

                    return _mapper.Map<Invoice, InvoiceDto>(entity);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("CreateDraft: An error has occured while Creating draft for Invoice Number:  {0}, ErrorMessage: {1}, InnerException: {2}", invoice.InvoiceNumber, ex.Message, ex.InnerException));
                transaction.Rollback();
                throw;
            }

        }


        public async Task<InvoiceDto> UpdateDraft(InvoiceRequestDto invoice, string user)
        {
            var dt = DateTime.UtcNow;
            Invoice entity = null;
            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();

            try
            {
                // update
                entity = _dbContext.Invoice.Where(x => x.InvoiceId == invoice.InvoiceId)
                .Include(i => i.InvoiceTimeReportCostDetails)
                .Include(i => i.InvoiceOtherCostDetails)
                .Include(i => i.InvoiceStatusLogs)
                .Include(i => i.InvoiceTimeReports).FirstOrDefault();

                if (entity == null)
                {
                    throw new System.Exception($"No Invoice found for InvoiceId - {invoice.InvoiceId} in the Database.");
                }

                var timeReportsToUpdate = new List<InvoiceTimeReports>();
                var timeReportsToRemove = new List<InvoiceTimeReports>();
                var timeReportsToAdd = new List<InvoiceTimeReports>();
                IEnumerable<InvoiceTimeReports> timeReportsUnChanged = ProcessTimeReports(invoice, entity, ref timeReportsToAdd, ref timeReportsToUpdate, ref timeReportsToRemove);
                _dbContext.InvoiceTimeReports.RemoveRange(timeReportsToRemove);
                _dbContext.InvoiceTimeReports.AddRange(timeReportsToAdd);

                var costDetailsToRemove = new List<InvoiceTimeReportCostDetails>();
                var costDetailsToAdd = new List<InvoiceTimeReportCostDetails>();
                // TODO clean up this method because we dont update anything
                IList<InvoiceTimeReportCostDetails> costDetailsUnChanged = ProcessTimeReportCostDetails(invoice, entity, ref costDetailsToAdd, ref costDetailsToRemove);
                _dbContext.InvoiceTimeReportCostDetails.RemoveRange(costDetailsToRemove);
                _dbContext.InvoiceTimeReportCostDetails.AddRange(costDetailsToAdd);

                var otherCostDetailsToUpdate = new List<InvoiceOtherCostDetails>();
                var otherCostDetailsToRemove = new List<InvoiceOtherCostDetails>();
                var otherCostDetailsToAdd = new List<InvoiceOtherCostDetails>();
                IList<InvoiceOtherCostDetails> otherCostDetailsUnChanged = ProcessOtherCostDetails(invoice, entity, ref otherCostDetailsToAdd, ref otherCostDetailsToUpdate, ref otherCostDetailsToRemove);
                _dbContext.InvoiceOtherCostDetails.RemoveRange(otherCostDetailsToRemove);
                _dbContext.InvoiceOtherCostDetails.AddRange(otherCostDetailsToAdd);

                _dbContext.SaveChanges();
                entity.InvoiceStatus = InvoiceStatus.Draft.ToString();
                entity.UpdatedByDateTime = dt;
                entity.UpdatedBy = user;

                entity.InvoiceAmount = invoice.InvoiceAmount;
                entity.InvoiceDate = invoice.InvoiceDate;
                entity.InvoiceNumber = invoice.InvoiceNumber;
                entity.InvoiceReceivedDate = invoice.InvoiceReceivedDate;
                entity.PeriodEndDate = invoice.PeriodEndDate;
                entity.ServiceDescription = invoice.ServiceDescription;
                entity.UniqueServiceSheetName = invoice.UniqueServiceSheetName;

                entity.InvoiceTimeReportCostDetails = costDetailsUnChanged.Concat(costDetailsToAdd).ToList();
                entity.InvoiceOtherCostDetails = otherCostDetailsToUpdate.Concat(otherCostDetailsToAdd).Concat(otherCostDetailsUnChanged).ToList();

                foreach (var timeReportCostDetail in costDetailsToAdd)
                {
                    timeReportCostDetail.CreatedBy = user;
                    timeReportCostDetail.CreatedByDateTime = dt;
                }
                foreach (var otherCostDetail in otherCostDetailsToUpdate)
                {
                    otherCostDetail.UpdatedBy = user;
                    otherCostDetail.UpdatedByDateTime = dt;
                }

                foreach (var otherCostDetail in otherCostDetailsToAdd)
                {
                    otherCostDetail.CreatedBy = user;
                    otherCostDetail.CreatedByDateTime = dt;
                }

                foreach (var timeReport in timeReportsToUpdate)
                {
                    timeReport.AuditLastUpdatedBy = user;
                    timeReport.AuditLastUpdatedDateTime = dt;
                }

                foreach (var timeReport in timeReportsToAdd)
                {
                    timeReport.AuditCreationDateTime = dt;
                    timeReport.AuditLastUpdatedBy = user;
                    timeReport.AuditLastUpdatedDateTime = dt;
                }

                _dbContext.SaveChanges();
                transaction.Commit();

                // send messages
                var statusPayload = CreateStatusSyncPayload(entity, "update-invoice", costDetailsToAdd, costDetailsToRemove, costDetailsUnChanged);
                await new InvoiceStatusSyncMessageHandler(_logger).SendInvoiceStatusSyncMessage(statusPayload, entity.InvoiceNumber);

                // send 3 messages, one for insert, one for update , one for delete
                var updateDataPayload = CreateDataSyncUpdatePayload(entity, new List<InvoiceTimeReportCostDetails>(), otherCostDetailsToUpdate, new List<InvoiceTimeReports>());
                await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(updateDataPayload, entity.InvoiceNumber, "update");

                var insertDataPayload = CreateDataSyncInsertPayload(null, costDetailsToAdd, otherCostDetailsToAdd, timeReportsToAdd);
                await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(insertDataPayload, entity.InvoiceNumber, "insert");

                var deleteDataPayload = CreateDataSyncDeletePayload(costDetailsToRemove, otherCostDetailsToRemove, timeReportsToRemove);
                await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(deleteDataPayload, entity.InvoiceNumber, "delete");

            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("UpdateDraft: An error has occured while updating draft for Invoice Number:  {0}, ErrorMessage: {1}, InnerException: {2}", invoice.InvoiceNumber, ex.Message, ex.InnerException));
                transaction.Rollback();
                throw;
            }

            return _mapper.Map<Invoice, InvoiceDto>(entity);
        }


        public async Task<Guid> DeleteDraft(Guid invoiceId, string user)
        {
            var dt = DateTime.UtcNow;
            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var entity = _dbContext.Invoice.Where(x => x.InvoiceId == invoiceId)
                    .Include(i => i.InvoiceTimeReportCostDetails)
                    .Include(i => i.InvoiceOtherCostDetails)
                    .Include(i => i.InvoiceStatusLogs)
                    .Include(i => i.InvoiceTimeReports).FirstOrDefault();

                entity.InvoiceStatus = InvoiceStatus.DraftDeleted.ToString();
                entity.UpdatedBy = user;
                entity.UpdatedByDateTime = DateTime.UtcNow;

                // Save changes to the database
                _dbContext.SaveChanges();
                transaction.Commit();

                var updatedEntity = _dbContext.Invoice.Where(x => x.InvoiceId == invoiceId)
                    .Include(i => i.InvoiceTimeReportCostDetails)
                    .Include(i => i.InvoiceOtherCostDetails)
                    .Include(i => i.InvoiceStatusLogs)
                    .Include(i => i.InvoiceTimeReports).FirstOrDefault();

                var updatePayload = CreateDataSyncUpdatePayload(updatedEntity, new List<InvoiceTimeReportCostDetails>(), new List<InvoiceOtherCostDetails>(), new List<InvoiceTimeReports>());
                await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(updatePayload, entity.InvoiceNumber, "update");

                var payload = CreateStatusSyncPayload(updatedEntity, "update-invoice", new List<InvoiceTimeReportCostDetails>(), updatedEntity.InvoiceTimeReportCostDetails, new List<InvoiceTimeReportCostDetails>());
                await new InvoiceStatusSyncMessageHandler(_logger).SendInvoiceStatusSyncMessage(payload, entity.InvoiceNumber);
                return entity.InvoiceId;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("DeleteDraft: An error has occured while Deleting Invoice for InvoiceId:  {0}, ErrorMessage: {1}, InnerException: {2}", invoiceId, ex.Message, ex.InnerException));
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateInvoiceStatus(UpdateInvoiceStatusRequestDto request)
        {
            bool result = false;

            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var entity = await _dbContext.Invoice.Where(x => x.InvoiceId == request.InvoiceId)
                .Include(i => i.InvoiceTimeReportCostDetails)
                .Include(i => i.InvoiceOtherCostDetails)
                .Include(i => i.InvoiceStatusLogs)
                .Include(i => i.InvoiceTimeReports).FirstOrDefaultAsync();
                if (entity == null)
                {
                    throw new System.Exception($"No Invoice found for InvoiceId - {request.InvoiceId} in the Database.");
                }

                if (request.PaymentStatus != entity.PaymentStatus)
                {
                    entity.InvoiceStatusLogs = new List<InvoiceStatusLog> { new()
                                    {
                                        InvoiceId = entity.InvoiceId,
                                        CurrentStatus = request.PaymentStatus,
                                        PreviousStatus = entity.PaymentStatus,
                                        User = request.UpdatedBy,
                                        Timestamp = request.UpdatedDateTime.Value
                                    }};
                    entity.PaymentStatus = request.PaymentStatus;
                    entity.UpdatedBy = request.UpdatedBy;
                    entity.UpdatedByDateTime = DateTime.UtcNow;


                    _dbContext.SaveChanges();
                    transaction.Commit();
                    var updatedEntity = _dbContext.Invoice.Where(x => x.InvoiceId == request.InvoiceId)
                                        .Include(i => i.InvoiceTimeReportCostDetails)
                                        .Include(i => i.InvoiceOtherCostDetails)
                                        .Include(i => i.InvoiceStatusLogs)
                                        .Include(i => i.InvoiceTimeReports).FirstOrDefault();

                    var dataPayload = CreateDataSyncUpdatePayload(updatedEntity, updatedEntity.InvoiceTimeReportCostDetails, updatedEntity.InvoiceOtherCostDetails, updatedEntity.InvoiceTimeReports);
                    await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(dataPayload, entity.InvoiceNumber, "update");

                    var statusPayload = CreateStatusSyncPayload(entity, "update-invoice", new List<InvoiceTimeReportCostDetails>(), new List<InvoiceTimeReportCostDetails>(), updatedEntity.InvoiceTimeReportCostDetails);
                    await new InvoiceStatusSyncMessageHandler(_logger).SendInvoiceStatusSyncMessage(statusPayload, entity.InvoiceNumber);
                    result = true;

                }
            }
            catch
            {
                _logger.LogError("UpdateInvoiceStatus: An error has occured while Updating Invoice for Invoice Number: " + request.InvoiceId);
                transaction.Rollback();
                throw;
            }

            return result;
        }

        public int UpdateInvoice(InvoiceDto invoice)
        {
            return 0;
        }

        public bool InvoiceExistsForContract(Guid? _invoiceId, string invoiceNumber, string contractNumber)
        {
            var invoiceId = _invoiceId ?? Guid.Empty;

            bool bResult = false;
            // this will still fail....for updates, should not call this

            try
            {
                Invoice invoice = _dbContext.Invoice.Where(x =>
                    x.InvoiceNumber == invoiceNumber && x.ContractNumber == contractNumber && invoiceId != x.InvoiceId).FirstOrDefault();
                if (invoice != null
                    && string.Compare(invoice.InvoiceStatus, InvoiceStatus.DraftDeleted.ToString()) != 0)
                    bResult = true;
            }
            catch
            {
                _logger.LogError("An error has occured while looking up invoice: " + invoiceNumber);
                throw;
            }

            return bResult;
        }

        /// <summary>
        /// returns ivoices for a specific contract if passed in request object 
        /// and if contract number is an empty string it returns all invoices
        /// </summary>
        /// <param name="invoiceRequest"></param>
        /// <returns></returns>
        public InvoiceResponseDto GetInvoices(GetInvoiceRequestDto invoiceRequest)
        {
            InvoiceResponseDto response = new InvoiceResponseDto();
            List<Invoice> items;

            try
            {
                if (invoiceRequest.ContractNumber.Trim().Length == 0)
                    items = _dbContext.Invoice.ToList();
                else
                    items = _dbContext.Invoice.Include(p => p.ChargeExtract).Where(x => x.ContractNumber == invoiceRequest.ContractNumber).ToList();

                var mapped = items.Select(item =>
                {
                    return _mapper.Map<Invoice, InvoiceDto>(item);
                });
                response.Invoices = mapped.ToArray();
            }
            catch
            {
                _logger.LogError("An error has occured while retrieving Invoices for: " + invoiceRequest.ContractNumber);
                throw;
            }

            return response;
        }


        public InvoiceListResponseDto GetInvoiceList()
        {
            InvoiceListResponseDto response = new InvoiceListResponseDto();
            List<Invoice> items;

            try
            {
                items = _dbContext.Invoice.ToList();

                var mapped = items.Select(item =>
                {
                    return _mapper.Map<Invoice, InvoiceListDto>(item);
                });
                response.InvoiceList = mapped.ToArray();
            }
            catch
            {
                _logger.LogError("An error has occured while retrieving InvoiceList.");
                throw;
            }

            return response;
        }
        public InvoiceDetailResponseDto GetInvoiceDetails(InvoiceDetailRequestDto invoiceDetailRequest)
        {
            InvoiceDetailResponseDto response = new InvoiceDetailResponseDto();

            try
            {
                List<Invoice> items = _dbContext.Invoice.Where(x => x.InvoiceId == invoiceDetailRequest.InvoiceId)
                    .Include(p => p.InvoiceOtherCostDetails)
                    .Include(q => q.InvoiceTimeReportCostDetails)
                    .ToList();
                var mapped = items.Select(item =>
                {
                    return _mapper.Map<Invoice, InvoiceDto>(item);
                });
                response.Invoice = mapped.FirstOrDefault();
            }
            catch
            {
                _logger.LogError("An error has occured while retrieving Invoices for: " + invoiceDetailRequest.InvoiceId);
                throw;
            }

            return response;
        }

        public InvoiceResponseDto GetInvoicesWithDetails(GetInvoiceRequestDto invoiceRequest)
        {
            InvoiceResponseDto response = new();
            List<Invoice> items = _dbContext.Invoice
                .Include(i => i.InvoiceTimeReportCostDetails)
                .Include(i => i.InvoiceOtherCostDetails)
                .Include(i => i.InvoiceTimeReports)
                .Where(x => x.ContractNumber == invoiceRequest.ContractNumber)
                .ToList();

            response.Invoices = items.Select(item =>
                {
                    return _mapper.Map<Invoice, InvoiceDto>(item);
                }).ToArray();


            return response;
        }
        public CostDetailsResponseDto GetCostDetails(CostDetailsRequestDto request)
        {
            CostDetailsResponseDto response = new CostDetailsResponseDto();

            try
            {
                if (request != null && request.FlightReportCostDetailIds != null && request.FlightReportCostDetailIds.Count() > 0)
                {
                    request.FlightReportCostDetailIds.ForEach(item =>
                    {
                        if (!_dbContext.InvoiceTimeReportCostDetails.Any(c => c.FlightReportCostDetailsId == item && c.FlightReportId == request.FlightReportId))
                        {
                            response.CostDetails.Add(new CostDetailsResponseDto.CostDetailsResult()
                            {
                                FlightReportId = request.FlightReportId.Value,
                                FlightReportCostDetailsId = item,
                                InvoiceNumber = string.Empty,
                                PaymentStatus = string.Empty,
                                RedirectionURL = string.Empty
                            });
                        }
                        else
                        {
                            var result = (from trc in _dbContext.InvoiceTimeReportCostDetails
                                          join i in _dbContext.Invoice.DefaultIfEmpty()
                                          on trc.InvoiceId equals i.InvoiceId
                                          where trc.FlightReportCostDetailsId == item
                                          select new
                                          {
                                              FlightReportCostDetailsId = item,
                                              InvoiceNumber = i.InvoiceNumber,
                                              PaymentStatus = i.PaymentStatus
                                          }).FirstOrDefault();

                            if (result != null)
                            {
                                response.CostDetails.Add(new CostDetailsResponseDto.CostDetailsResult()
                                {
                                    FlightReportId = request.FlightReportId.Value,
                                    FlightReportCostDetailsId = result.FlightReportCostDetailsId,
                                    InvoiceNumber = result.InvoiceNumber,
                                    PaymentStatus = !string.IsNullOrEmpty(result.PaymentStatus) ? result.PaymentStatus : string.Empty,
                                    RedirectionURL = string.Format(Environment.GetEnvironmentVariable("ContractAppUrl") + CONTRACTS_API_PATH_PROCESSEDINVOICE, result.InvoiceNumber)
                                });
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCostDetails: Error retrieving processed cost details - Message:{0}, StackTrace:{1}, InnerException:{2}", ex.Message, ex.StackTrace, ex.InnerException);
                throw;
            }

            return response;
        }

        private static void SetCreatedFields<T>(ICollection<T> entities, string user, DateTime dt) where T : EntityBase
        {
            if (entities == null || !entities.Any()) return;

            foreach (var item in entities)
            {
                item.CreatedBy = user;
                item.CreatedByDateTime = dt;
            }
        }

        private static InvoiceStatusSyncMessageDto CreateStatusSyncPayload(Invoice invoiceEntity, string action, IList<InvoiceTimeReportCostDetails> inserted, IList<InvoiceTimeReportCostDetails> removed, IList<InvoiceTimeReportCostDetails> notChanged)
        {
            var payload = new InvoiceStatusSyncMessageDto
            {
                Action = action,
                TimeStamp = DateTime.UtcNow,
                InvoiceId = invoiceEntity.InvoiceId,
                InvoiceNumber = invoiceEntity.InvoiceNumber,
                PaymentStatus = invoiceEntity.PaymentStatus,
                InvoiceStatus = invoiceEntity.InvoiceStatus.ToString(),
                Details = new Details
                {
                    Inserted = inserted.Select(x => { return new CostDetails { FlightReportCostDetailsId = x.FlightReportCostDetailsId, FlightReportId = x.FlightReportId }; }).ToList(),
                    Removed = removed.Select(x => { return new CostDetails { FlightReportCostDetailsId = x.FlightReportCostDetailsId, FlightReportId = x.FlightReportId }; }).ToList(),
                    NotChanged = notChanged.Select(x => { return new CostDetails { FlightReportCostDetailsId = x.FlightReportCostDetailsId, FlightReportId = x.FlightReportId }; }).ToList(),
                }
            };
            return payload;

        }


        private InvoiceDataSyncMessageDto CreateDataSyncInsertPayload(Invoice invoiceEntity,
                IList<InvoiceTimeReportCostDetails> costDetails, IList<InvoiceOtherCostDetails> other, IList<InvoiceTimeReports> timeReports)
        {

            var messageDetailInvoice = (invoiceEntity == null) ? new InvoiceDataSyncMessageDetailInvoiceDto() : _mapper.Map<InvoiceDataSyncMessageDetailInvoiceDto>(invoiceEntity);
            messageDetailInvoice.Tables = new InvoiceDataSyncMessageDetailCostDto
            {
                InvoiceOtherCostDetails = _mapper.Map<List<InvoiceOtherCostDetailDto>>(other),
                InvoiceTimeReportCostDetails = _mapper.Map<List<InvoiceTimeReportCostDetailDto>>(costDetails),
                InvoiceTimeReports = _mapper.Map<List<InvoiceTimeReportsDto>>(timeReports)
            };

            var payload = new InvoiceDataSyncMessageDto
            {
                Action = "insert-invoice",
                TimeStamp = DateTime.UtcNow,
                Tables = new InvoiceDataSyncMessageDetailDto
                {
                    Invoice = messageDetailInvoice,
                }

            };
            return payload;

        }
        private static InvoiceDataSyncDeleteInvoiceMessageDto CreateDataSyncDeletePayload(
        IList<InvoiceTimeReportCostDetails> costDetails, IList<InvoiceOtherCostDetails> other, IList<InvoiceTimeReports> timeReports)
        {

            var messageDetailInvoice = new InvoiceDataSyncDeleteInvoiceMessageDetailInvoiceDto
            {
                Tables = new InvoiceDataSyncDeleteInvoiceMessageDetailCostDto
                {
                    InvoiceOtherCostDetails = other.Select(x => x.InvoiceOtherCostDetailId).ToList(),
                    InvoiceTimeReportCostDetails = costDetails.Select(x =>
                    {
                        return new InvoiceTimeReportCostDetailsDeleteRowDto { InvoiceId = x.InvoiceId, FlightReportCostDetailsId = x.FlightReportCostDetailsId };
                    }).ToList(),
                    InvoiceTimeReports = timeReports.Select(x => x.InvoiceTimeReportId).ToList()
                }
            };
            // only updated records
            var payload = new InvoiceDataSyncDeleteInvoiceMessageDto
            {
                Action = "delete-invoice",
                TimeStamp = DateTime.UtcNow,
                Tables = new InvoiceDataSyncDeleteInvoiceMessageDetailDto
                {
                    Invoice = messageDetailInvoice,
                }

            };
            return payload;

        }

        private InvoiceDataSyncMessageDto CreateDataSyncUpdatePayload(Invoice invoiceEntity,
        IList<InvoiceTimeReportCostDetails> updatedCostDetails, IList<InvoiceOtherCostDetails> updatedOther, IList<InvoiceTimeReports> updatedTimeReports)
        {
            var messageDetailInvoice = _mapper.Map<InvoiceDataSyncMessageDetailInvoiceDto>(invoiceEntity);
            messageDetailInvoice.Tables = new InvoiceDataSyncMessageDetailCostDto
            {
                InvoiceOtherCostDetails = _mapper.Map<List<InvoiceOtherCostDetailDto>>(updatedOther),
                InvoiceTimeReportCostDetails = _mapper.Map<List<InvoiceTimeReportCostDetailDto>>(updatedCostDetails),
                InvoiceTimeReports = _mapper.Map<List<InvoiceTimeReportsDto>>(updatedTimeReports)
            };
            // only updated records
            var payload = new InvoiceDataSyncMessageDto
            {
                Action = "update-invoice",
                TimeStamp = DateTime.UtcNow,
                Tables = new InvoiceDataSyncMessageDetailDto
                {
                    Invoice = messageDetailInvoice,
                }

            };
            return payload;

        }

        private IList<InvoiceTimeReportCostDetails> ProcessTimeReportCostDetails(InvoiceRequestDto invoice, Invoice entity, ref List<InvoiceTimeReportCostDetails> costDetailsToAdd, ref List<InvoiceTimeReportCostDetails> costDetailsToRemove)
        {
            var incoming = _mapper.Map<IList<InvoiceTimeReportCostDetails>>(invoice.InvoiceTimeReportCostDetails);
            incoming = incoming.Select(x =>
            {
                x.InvoiceId = invoice.InvoiceId.Value;
                return x;
            }).ToList();

            var costDetailsUnChanged = new List<InvoiceTimeReportCostDetails>();
            if (entity.InvoiceTimeReportCostDetails.Count == 0)
            {
                costDetailsToAdd.AddRange(incoming);
            }
            else
            {
                // we have a list of ids, 
                // get all the existing ids
                // if the new id is in the existing list, it is unchanged                
                // if the new id is not in the list, it is added
                var ids = incoming.Select(x => x.FlightReportCostDetailsId);
                foreach (var i in incoming)
                {

                    var existing = entity.InvoiceTimeReportCostDetails.Where(x => x.FlightReportCostDetailsId == i.FlightReportCostDetailsId).FirstOrDefault();
                    if (existing != null)
                    {
                        i.CreatedBy = existing.CreatedBy;
                        i.CreatedByDateTime = existing.CreatedByDateTime;
                        i.UpdatedBy = existing.UpdatedBy;
                        i.UpdatedByDateTime = existing.UpdatedByDateTime;
                        costDetailsUnChanged.Add(i);
                    }
                    else
                    {
                        costDetailsToAdd.Add(i);
                    }
                }
                // if there are ids in the existing that are not in the new, they are being removed
                costDetailsToRemove = entity.InvoiceTimeReportCostDetails.Where(x => !ids.Contains(x.FlightReportCostDetailsId)).ToList();
            }

            return costDetailsUnChanged;
        }
        private IList<InvoiceOtherCostDetails> ProcessOtherCostDetails(InvoiceRequestDto invoice, Invoice entity, ref List<InvoiceOtherCostDetails> toAdd, ref List<InvoiceOtherCostDetails> toUpdate, ref List<InvoiceOtherCostDetails> toRemove)
        {

            var incoming = _mapper.Map<IList<InvoiceOtherCostDetails>>(invoice.InvoiceOtherCostDetails);
            incoming = incoming.Select(x =>
            {
                x.InvoiceId = invoice.InvoiceId.Value;
                return x;
            }).ToList();
            var unChanged = new List<InvoiceOtherCostDetails>();
            if (entity.InvoiceOtherCostDetails.Count == 0)
            {
                toAdd.AddRange(incoming);
            }
            else
            {
                // we have a list of ids, 
                // get all the existing ids
                // if the new id is in the existing list, it is unchanged                
                // if the new id is not in the list, it is added
                var ids = incoming.Select(x => x.InvoiceOtherCostDetailId);
                foreach (var i in incoming)
                {

                    var existing = entity.InvoiceOtherCostDetails.Where(x => x.InvoiceOtherCostDetailId == i.InvoiceOtherCostDetailId).FirstOrDefault();
                    if (existing != null)
                    {
                        i.CreatedBy = existing.CreatedBy;
                        i.CreatedByDateTime = existing.CreatedByDateTime;
                        //does entity match
                        if (existing.Account == i.Account
                            && existing.Cost == i.Cost
                            && existing.CostCentre == i.CostCentre
                            && existing.FireNumber == i.FireNumber
                            && existing.From == i.From
                            && existing.Fund == i.Fund
                            && existing.InternalOrder == i.InternalOrder
                            && existing.NoOfUnits == i.NoOfUnits
                            && existing.ProfitCentre == i.ProfitCentre
                            && existing.RatePerUnit == i.RatePerUnit
                            && existing.RateType == i.RateType
                            && existing.RateUnit == i.RateUnit
                            && existing.Remarks == i.Remarks
                         )
                        {
                            unChanged.Add(i);
                        }
                        else
                        {
                            toUpdate.Add(i);
                        }
                    }
                    else
                    {
                        toAdd.Add(i);
                    }
                }
                // if there are ids in the existing that are not in the new, they are being removed
                toRemove = entity.InvoiceOtherCostDetails.Where(x => !ids.Contains(x.InvoiceOtherCostDetailId)).ToList();
            }

            return unChanged;
        }

        private IEnumerable<InvoiceTimeReports> ProcessTimeReports(InvoiceRequestDto invoice, Invoice entity, ref List<InvoiceTimeReports> toAdd, ref List<InvoiceTimeReports> toUpdate, ref List<InvoiceTimeReports> toRemove)
        {
            var unChanged = new List<InvoiceTimeReports>();
            if (entity.InvoiceTimeReports.Count == 0)
            {
                var incoming = invoice.FlightReportIds.Select(x =>
                {
                    return new InvoiceTimeReports
                    {
                        FlightReportId = x,
                        InvoiceId = entity.InvoiceId,

                    };
                });
                toAdd.AddRange(incoming);
            }
            else
            {
                // we have a list of ids, 
                // get all the existing ids
                // if the new id is in the existing list, it is unchanged                
                // if the new id is not in the list, it is added
                foreach (var i in invoice.FlightReportIds)
                {
                    var newEntity = new InvoiceTimeReports
                    {
                        FlightReportId = i,
                        InvoiceId = entity.InvoiceId,
                    };
                    if (entity.InvoiceTimeReports.Any(x => x.FlightReportId == i))
                    {
                        foreach (var item in invoice.InvoiceTimeReportCostDetails.Where(p => p.FlightReportId == i))
                        {
                            if (entity.InvoiceTimeReportCostDetails
                                           .Where(p => p.FlightReportCostDetailsId == item.FlightReportCostDetailsId).Count() == 0 && toUpdate.Where(p => p.FlightReportId == i).Count() == 0)
                            {
                                toUpdate.Add(newEntity);
                            }
                        }
                    }
                    else
                    {
                        toAdd.Add(newEntity);
                    }

                    if (toUpdate.Where(p => p.FlightReportId == i).Count() == 0 && toAdd.Where(p => p.FlightReportId == i).Count() == 0)
                        unChanged.Add(newEntity);
                }
                // if there are ids in the existing that are not in the new, they are being removed
                toRemove = entity.InvoiceTimeReports.Where(x => !invoice.FlightReportIds.Contains(x.FlightReportId)).ToList();
            }
            return unChanged;
        }
    }
}
