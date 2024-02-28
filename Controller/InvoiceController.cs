using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Controller
{
    public interface IInvoiceController
    {
        public int CreateInvoice(InvoiceDto invoice);
        public int UpdateInvoice(InvoiceDto invoice);
        public bool InvoiceExists(string invoiceID);
        public InvoiceResponseDto GetInvoices(InvoiceRequestDto invoiceRequest);
        public string UpdateProcessedInvoice(InvoiceServiceSheetDto invoiceServiceSheet);
        public CostDetailsResponseDto GetCostDetails(CostDetailsRequestDto request);
    }

    public class InvoiceController : IInvoiceController
    {
        ApplicationDBContext dbContext;
        ILogger _logger;
        IMapper _mapper;
        private const string DEFAULT_USER = "System";
        private const string CONTRACTS_API_PATH_PROCESSEDINVOICE = "/ProcessedInvoice/{0}";

        public InvoiceController(ILogger log, IMapper mapper)
        {

            dbContext = new ApplicationDBContext();
            _logger = log;
            _mapper = mapper;
        }


        public int CreateInvoice(InvoiceDto invoice)
        {
            int result = 0;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Invoice invoiceEntity = _mapper.Map<Invoice>(invoice);
                    invoiceEntity.CreatedBy = DEFAULT_USER;
                    invoiceEntity.CreatedByDateTime = DateTime.Now;
                    if(invoiceEntity.InvoiceTimeReportCostDetails  != null && invoiceEntity.InvoiceTimeReportCostDetails.Count() > 0)
                    {
                        foreach (var item in invoiceEntity.InvoiceTimeReportCostDetails)
                        {
                            item.CreatedBy = DEFAULT_USER;
                            item.CreatedByDateTime = DateTime.Now;
                        }
                    }
                    if (invoiceEntity.InvoiceOtherCostDetails != null && invoiceEntity.InvoiceOtherCostDetails.Count() > 0)
                    {
                        foreach (var item in invoiceEntity.InvoiceOtherCostDetails)
                        {
                            item.CreatedBy = DEFAULT_USER;
                            item.CreatedByDateTime = DateTime.Now;
                        }
                    }
                    if(invoiceEntity.InvoiceServiceSheet != null)
                    {
                        if (!string.IsNullOrEmpty(invoiceEntity.InvoiceServiceSheet.UniqueServiceSheetName))
                        {
                            invoiceEntity.PaymentStatus = Enums.PaymentStatus.Submitted.ToString();
                        }
                        invoiceEntity.InvoiceServiceSheet.CreatedBy = DEFAULT_USER;
                        invoiceEntity.InvoiceServiceSheet.CreatedByDateTime = DateTime.Now;
                    }
                    dbContext.Invoice.Add(invoiceEntity);
                    dbContext.SaveChanges();
                    invoice.InvoiceKey = invoiceEntity.InvoiceKey;
                    result = invoice.InvoiceKey;
                    transaction.Commit();
                }
                catch
                {
                    _logger.LogError("An error has occured while Saving Invoice: " + invoice.InvoiceKey);
                    transaction.Rollback();
                    throw;
                }
            }
            return result;
        }

        public string UpdateProcessedInvoice(InvoiceServiceSheetDto invoiceServiceSheet)
        {
            string result = string.Empty;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var invoiceServiceSheetRecord = dbContext.InvoiceServiceSheet.FirstOrDefault(ss => ss.InvoiceKey == invoiceServiceSheet.InvoiceKey);
                    if (invoiceServiceSheetRecord != null)
                    {
                        invoiceServiceSheetRecord.UniqueServiceSheetName = invoiceServiceSheet.UniqueServiceSheetName;
                        var invoiceRecord = dbContext.Invoice.FirstOrDefault(i => i.InvoiceKey == invoiceServiceSheetRecord.InvoiceKey);
                        if (invoiceRecord != null)
                        {
                            invoiceRecord.PaymentStatus = Enums.PaymentStatus.Submitted.ToString();
                        }
                        invoiceServiceSheetRecord.UpdatedBy = DEFAULT_USER;
                        invoiceServiceSheetRecord.UpdatedByDateTime = DateTime.Now;
                        dbContext.SaveChanges();
                        transaction.Commit();
                        result = invoiceServiceSheetRecord.UniqueServiceSheetName;
                    }
                    else
                    {
                        throw new System.Exception($"No Service Sheet Record found for InvoiceKey - {invoiceServiceSheet.InvoiceKey} in Database.");
                    }
                }
                catch
                {
                    _logger.LogError("An error has occured while Updating Invoice Service Sheet for Invoice Id: " + invoiceServiceSheet.InvoiceKey);
                    transaction.Rollback();
                    throw;
                }
            }
            return result;
        }

        public int UpdateInvoice(InvoiceDto invoice)
        {
            return 0;
        }

        public bool InvoiceExists(string invoiceId)
        {
            bool bResult = false;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Invoice invoice = dbContext.Invoice.Where(x => x.InvoiceId == invoiceId).FirstOrDefault();
                    if (invoice != null)
                        bResult = true;
                }
                catch
                {
                    _logger.LogError("An error has occured while Saving Invoice: " + invoiceId);
                    transaction.Rollback();
                    throw;
                }
            }
            return bResult;
        }

        public InvoiceResponseDto GetInvoices(InvoiceRequestDto invoiceRequest)
        {
            InvoiceResponseDto response = new InvoiceResponseDto();
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<Invoice> items = dbContext.Invoice.Where(x => x.ContractNumber == invoiceRequest.ContractNumber).Include(i => i.InvoiceServiceSheet).ToList();
                    var mapped = items.Select(item =>
                    {
                        return _mapper.Map<Invoice, InvoiceDto>(item);
                    });
                    response.Invoices = mapped.ToArray();
                }
                catch
                {
                    _logger.LogError("An error has occured while retrieving Invoices for: " + invoiceRequest.ContractNumber);
                    transaction.Rollback();
                    throw;
                }
            }
            return response;
        }

        public InvoiceDetailResponseDto GetInvoiceDetails(InvoiceDetailRequestDto invoiceDetailRequest)
        {
            InvoiceDetailResponseDto response = new InvoiceDetailResponseDto();
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<Invoice> items = dbContext.Invoice.Where(x => x.InvoiceKey == invoiceDetailRequest.InvoiceKey)
                        .Include(p => p.InvoiceOtherCostDetails)
                        .Include(q => q.InvoiceTimeReportCostDetails)
                        .Include(r => r.InvoiceServiceSheet)
                        .ToList();
                    var mapped = items.Select(item =>
                    {
                        return _mapper.Map<Invoice, InvoiceDto>(item);
                    });
                    response.Invoice = mapped.FirstOrDefault();
                }
                catch
                {
                    _logger.LogError("An error has occured while retrieving Invoices for: " + invoiceDetailRequest.InvoiceKey);
                    transaction.Rollback();
                    throw;
                }
            }
            return response;
        }

        public CostDetailsResponseDto GetCostDetails(CostDetailsRequestDto request)
        {
            CostDetailsResponseDto response = new CostDetailsResponseDto();
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (request != null && request.FlightReportCostDetailIds != null && request.FlightReportCostDetailIds.Count() > 0)
                    {
                        request.FlightReportCostDetailIds.ForEach(item =>
                        {
                            if(!dbContext.InvoiceTimeReportCostDetails.Any(c => c.FlightReportCostDetailsId == item && c.FlightReportId == request.FlightReportId))
                            {
                                response.CostDetails.Add(new CostDetailsResponseDto.CostDetailsResult()
                                {
                                    FlightReportId = request.FlightReportId.Value,
                                    FlightReportCostDetailsId = item,
                                    InvoiceId = string.Empty,
                                    PaymentStatus = string.Empty,
                                    RedirectionURL = string.Empty
                                });
                            }
                            else
                            {
                                var result = (from trc in dbContext.InvoiceTimeReportCostDetails
                                              join i in dbContext.Invoice.DefaultIfEmpty()
                                              on trc.InvoiceKey equals i.InvoiceKey
                                                where trc.FlightReportCostDetailsId == item
                                              select new
                                              {
                                                  FlightReportCostDetailsId = item,
                                                  Invoicekey = i.InvoiceKey,
                                                  InvoiceId = i.InvoiceId,
                                                  PaymentStatus = i.PaymentStatus
                                              }).FirstOrDefault();

                                if (result != null)
                                {
                                    response.CostDetails.Add(new CostDetailsResponseDto.CostDetailsResult()
                                    {
                                        FlightReportId = request.FlightReportId.Value,
                                        FlightReportCostDetailsId = result.FlightReportCostDetailsId,
                                        InvoiceId = result.InvoiceId,
                                        PaymentStatus = !string.IsNullOrEmpty(result.PaymentStatus) ? result.PaymentStatus : string.Empty,
                                        RedirectionURL = string.Format(Environment.GetEnvironmentVariable("ContractAppUrl") + CONTRACTS_API_PATH_PROCESSEDINVOICE, result.Invoicekey)
                                    }) ;
                                }
                            }
                        });
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError("GetCostDetails: Error retrieving processed cost details - Message:{0}, StackTrace:{1}, InnerException:{2}", ex.Message, ex.StackTrace, ex.InnerException);
                    transaction.Rollback();
                    throw;
                }
            }
            return response;
        }
    }
}
