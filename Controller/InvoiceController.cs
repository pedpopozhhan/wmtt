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
        public Guid CreateInvoice(InvoiceDto invoice);
        public int UpdateInvoice(InvoiceDto invoice);
        public bool InvoiceExists(string invoiceNumber);
        public InvoiceResponseDto GetInvoices(InvoiceRequestDto invoiceRequest);
        public string UpdateProcessedInvoice(InvoiceDto invoice);
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


        public Guid CreateInvoice(InvoiceDto invoice)
        {
            Guid result = Guid.Empty;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Invoice invoiceEntity = _mapper.Map<Invoice>(invoice);
                    invoiceEntity.CreatedBy = DEFAULT_USER;
                    invoiceEntity.CreatedByDateTime = DateTime.Now;
                    if (invoiceEntity.InvoiceTimeReportCostDetails != null && invoiceEntity.InvoiceTimeReportCostDetails.Count() > 0)
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
                    if (!string.IsNullOrEmpty(invoiceEntity.UniqueServiceSheetName))
                    {
                        invoiceEntity.PaymentStatus = Enums.PaymentStatus.Submitted.ToString();
                        invoiceEntity.InvoiceStatusLogs = new List<InvoiceStatusLog> { new InvoiceStatusLog()
                                        {
                                            InvoiceId = invoiceEntity.InvoiceId,
                                            CurrentStatus = invoiceEntity.PaymentStatus,
                                            PreviousStatus = string.Empty,
                                            User = DEFAULT_USER,
                                            Timestamp = DateTime.Now
                                        }};

                    }
                    invoiceEntity.CreatedBy = DEFAULT_USER;
                    invoiceEntity.CreatedByDateTime = DateTime.Now;
                    dbContext.Invoice.Add(invoiceEntity);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    result = invoiceEntity.InvoiceId;
                }
                catch
                {
                    _logger.LogError("An error has occured while Saving Invoice: " + invoice.InvoiceNumber);
                    transaction.Rollback();
                    throw;
                }
            }
            return result;
        }

        public string UpdateProcessedInvoice(InvoiceDto invoice)
        {
            string result = string.Empty;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var invoiceRecord = dbContext.Invoice.FirstOrDefault(ss => ss.InvoiceId == invoice.InvoiceId);
                    if (invoiceRecord == null)
                    {
                        throw new System.Exception($"No Invoice found for InvoiceId - {invoice.InvoiceId} in the Database.");
                    }
                    invoiceRecord.UniqueServiceSheetName = invoice.UniqueServiceSheetName;
                    string previousStatus = invoiceRecord.PaymentStatus;
                    invoiceRecord.PaymentStatus = Enums.PaymentStatus.Submitted.ToString();
                    invoiceRecord.UpdatedBy = DEFAULT_USER;
                    invoiceRecord.UpdatedByDateTime = DateTime.Now;

                    if(string.IsNullOrEmpty(previousStatus) || previousStatus != invoiceRecord.PaymentStatus)
                    {
                        invoiceRecord.InvoiceStatusLogs = new List<InvoiceStatusLog> { new InvoiceStatusLog()
                                    {
                                        InvoiceId = invoiceRecord.InvoiceId,
                                        CurrentStatus = invoiceRecord.PaymentStatus,
                                        PreviousStatus = previousStatus,
                                        User = DEFAULT_USER,
                                        Timestamp = DateTime.Now
                                    }};
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    result = invoiceRecord.UniqueServiceSheetName;
                }
                catch
                {
                    _logger.LogError("UpdateProcessedInvoice: An error has occured while Updating Invoice for Invoice Number: " + invoice.InvoiceNumber);
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

        public bool InvoiceExists(string invoiceNumber)
        {
            bool bResult = false;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Invoice invoice = dbContext.Invoice.Where(x => x.InvoiceNumber == invoiceNumber).FirstOrDefault();
                    if (invoice != null)
                        bResult = true;
                }
                catch
                {
                    _logger.LogError("An error has occured while Saving Invoice: " + invoiceNumber);
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
                    List<Invoice> items = dbContext.Invoice.Where(x => x.ContractNumber == invoiceRequest.ContractNumber).ToList();
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
                    List<Invoice> items = dbContext.Invoice.Where(x => x.InvoiceId == invoiceDetailRequest.InvoiceId)
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
                            if (!dbContext.InvoiceTimeReportCostDetails.Any(c => c.FlightReportCostDetailsId == item && c.FlightReportId == request.FlightReportId))
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
                                var result = (from trc in dbContext.InvoiceTimeReportCostDetails
                                              join i in dbContext.Invoice.DefaultIfEmpty()
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
                    transaction.Rollback();
                    throw;
                }
            }
            return response;
        }
    }
}
