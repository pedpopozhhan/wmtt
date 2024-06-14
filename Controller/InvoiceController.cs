using AutoMapper;
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
using WCDS.WebFuncions.Enums;

namespace WCDS.WebFuncions.Controller
{
    public interface IInvoiceController
    {
        public Task<Guid> CreateInvoice(InvoiceDto invoice);
        public int UpdateInvoice(InvoiceDto invoice);
        public bool InvoiceExistsForContract(string invoiceNumber, string contractNumber);
        public InvoiceResponseDto GetInvoices(GetInvoiceRequestDto invoiceRequest);
        public InvoiceResponseDto GetInvoicesWithDetails(GetInvoiceRequestDto invoiceRequest);
        public Task<string> UpdateProcessedInvoice(InvoiceDto invoice);
        public Task<bool> UpdateInvoiceStatus(UpdateInvoiceStatusRequestDto request);
        public CostDetailsResponseDto GetCostDetails(CostDetailsRequestDto request);
        public Task<Guid> UpdateDraft(InvoiceRequestDto invoice, string user);
        public Task<Guid> CreateDraft(InvoiceRequestDto invoice, string user);
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


        public async Task<Guid> CreateInvoice(InvoiceDto invoice)
        {
            Guid result = Guid.Empty;
            int numberOfCostDetails = 0, numberOfOtherCostDetails = 0;
            invoice.InvoiceStatus = InvoiceStatus.Processed.ToString();
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
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

                    dbContext.Invoice.Add(invoiceEntity);
                    dbContext.SaveChanges();

                    _logger.LogInformation("Ivoice Insert Completed at: {0} for invoice: {1}", DateTime.UtcNow, invoiceEntity.InvoiceNumber);

                    await SendCreateInvoiceMessage(invoiceEntity);

                    if (invoiceEntity.InvoiceTimeReportCostDetails != null && invoiceEntity.InvoiceTimeReportCostDetails.Count > 0)
                    {
                        await SendInvoiceStatusSyncMessage(invoiceEntity, "update-invoice");
                    }

                    result = invoiceEntity.InvoiceId;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("CreateInvoice: An error has occured while Saving Invoice: {0}, ErrorMessage: {1}, InnerException: {2}", invoice.InvoiceNumber, ex.Message, ex.InnerException));
                    transaction.Rollback();
                    throw;
                }
            }
            return result;
        }



        public async Task<string> UpdateProcessedInvoice(InvoiceDto invoice)
        {
            string result = string.Empty;
            invoice.InvoiceStatus = InvoiceStatus.Processed.ToString();
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
                    invoiceRecord.UpdatedBy = invoice.UpdatedBy;
                    invoiceRecord.UpdatedByDateTime = DateTime.UtcNow;
                    dbContext.SaveChanges();
                    await SendUpdateInvoiceMessage(invoiceRecord);

                    result = invoiceRecord.UniqueServiceSheetName;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("UpdateProcessedInvoice: An error has occured while Updating Invoice for Invoice Number:  {0}, ErrorMessage: {1}, InnerException: {2}", invoice.InvoiceNumber, ex.Message, ex.InnerException));
                    transaction.Rollback();
                    throw;
                }
            }
            return result;
        }



        public async Task<Guid> CreateDraft(InvoiceRequestDto invoice, string user)
        {

            var dt = DateTime.UtcNow;

            using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();
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
                    dbContext.Invoice.Add(entity);
                    SetCreatedFields(entity.InvoiceTimeReportCostDetails, user, dt);
                    SetCreatedFields(entity.InvoiceOtherCostDetails, user, dt);


                    await dbContext.SaveChangesAsync();

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
                    await dbContext.InvoiceTimeReports.AddRangeAsync(invoiceTimeReports);
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // send messages
                    await SendCreateInvoiceMessage(entity);

                    if (entity.InvoiceTimeReportCostDetails != null && entity.InvoiceTimeReportCostDetails.Count > 0)
                    {
                        await SendInvoiceStatusSyncMessage(entity, "update-invoice");
                    }
                    return entity.InvoiceId;
                }
                return Guid.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("CreateDraft: An error has occured while Creating draft for Invoice Number:  {0}, ErrorMessage: {1}, InnerException: {2}", invoice.InvoiceNumber, ex.Message, ex.InnerException));
                await transaction.RollbackAsync();
                throw;
            }

        }


        public async Task<Guid> UpdateDraft(InvoiceRequestDto invoice, string user)
        {
            var dt = DateTime.UtcNow;
            Invoice entity;
            using (IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    // update
                    entity = await dbContext.Invoice.Where(x => x.InvoiceId == invoice.InvoiceId)
                    .Include(i => i.InvoiceTimeReportCostDetails)
                    .Include(i => i.InvoiceOtherCostDetails)
                    .Include(i => i.InvoiceStatusLogs)
                    .Include(i => i.InvoiceTimeReports).FirstOrDefaultAsync();

                    if (entity == null)
                    {
                        throw new System.Exception($"No Invoice found for InvoiceId - {invoice.InvoiceId} in the Database.");
                    }

                    dbContext.InvoiceTimeReports.RemoveRange(entity.InvoiceTimeReports);
                    dbContext.InvoiceOtherCostDetails.RemoveRange(entity.InvoiceOtherCostDetails);
                    dbContext.InvoiceTimeReportCostDetails.RemoveRange(entity.InvoiceTimeReportCostDetails);
                    await dbContext.SaveChangesAsync();
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
                    entity.InvoiceTimeReports = invoiceTimeReports.ToList();
                    entity.InvoiceOtherCostDetails = _mapper.Map<List<InvoiceOtherCostDetails>>(invoice.InvoiceOtherCostDetails);

                    entity.InvoiceTimeReportCostDetails = _mapper.Map<List<InvoiceTimeReportCostDetails>>(invoice.InvoiceTimeReportCostDetails);

                    foreach (var timeReport in entity.InvoiceTimeReports)
                    {
                        dbContext.Entry(timeReport).State = EntityState.Added;
                    }

                    foreach (var otherCostDetail in entity.InvoiceOtherCostDetails)
                    {
                        dbContext.Entry(otherCostDetail).State = EntityState.Added;
                    }

                    foreach (var timeReportCostDetail in entity.InvoiceTimeReportCostDetails)
                    {
                        dbContext.Entry(timeReportCostDetail).State = EntityState.Added;
                    }
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // send messages
                    await SendCreateInvoiceMessage(entity);

                    if (entity.InvoiceTimeReportCostDetails != null && entity.InvoiceTimeReportCostDetails.Count > 0)
                    {
                        await SendInvoiceStatusSyncMessage(entity, "update-invoice");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("UpdateDraft: An error has occured while updating draft for Invoice Number:  {0}, ErrorMessage: {1}, InnerException: {2}", invoice.InvoiceNumber, ex.Message, ex.InnerException));
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            return entity.InvoiceId;
        }
        public async Task<bool> UpdateInvoiceStatus(UpdateInvoiceStatusRequestDto request)
        {
            bool result = false;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var invoiceRecord = dbContext.Invoice.FirstOrDefault(ss => ss.InvoiceId == request.InvoiceId);
                    if (invoiceRecord == null)
                    {
                        throw new System.Exception($"No Invoice found for InvoiceId - {request.InvoiceId} in the Database.");
                    }

                    if (request.PaymentStatus != invoiceRecord.PaymentStatus)
                    {
                        invoiceRecord.InvoiceStatusLogs = new List<InvoiceStatusLog> { new()
                                    {
                                        InvoiceId = invoiceRecord.InvoiceId,
                                        CurrentStatus = request.PaymentStatus,
                                        PreviousStatus = invoiceRecord.PaymentStatus,
                                        User = request.UpdatedBy,
                                        Timestamp = request.UpdatedDateTime.Value
                                    }};
                        invoiceRecord.PaymentStatus = request.PaymentStatus;
                        invoiceRecord.UpdatedBy = request.UpdatedBy;
                        invoiceRecord.UpdatedByDateTime = DateTime.UtcNow;
                        dbContext.SaveChanges();

                        await SendUpdateInvoiceMessage(invoiceRecord);

                        result = true;
                        transaction.Commit();
                    }
                }
                catch
                {
                    _logger.LogError("UpdateInvoiceStatus: An error has occured while Updating Invoice for Invoice Number: " + request.InvoiceId);
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

        public bool InvoiceExistsForContract(string invoiceNumber, string contractNumber)
        {
            bool bResult = false;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Invoice invoice = dbContext.Invoice.Where(x => string.Compare(x.InvoiceStatus, InvoiceStatus.Draft.ToString()) != 0 && x.InvoiceNumber == invoiceNumber && x.ContractNumber == contractNumber).FirstOrDefault();
                    if (invoice != null)
                        bResult = true;
                }
                catch
                {
                    _logger.LogError("An error has occured while looking up invoice: " + invoiceNumber);
                    transaction.Rollback();
                    throw;
                }
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
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (invoiceRequest.ContractNumber.Trim().Length == 0)
                        items = dbContext.Invoice.ToList();
                    else
                        items = dbContext.Invoice.Include(p => p.ChargeExtract).Where(x => x.ContractNumber == invoiceRequest.ContractNumber).ToList();

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

        public InvoiceResponseDto GetInvoicesWithDetails(GetInvoiceRequestDto invoiceRequest)
        {
            InvoiceResponseDto response = new();
            List<Invoice> items = dbContext.Invoice
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

        private static void SetCreatedFields<T>(ICollection<T> entities, string user, DateTime dt) where T : EntityBase
        {
            if (entities == null || !entities.Any()) return;

            foreach (var item in entities)
            {
                item.CreatedBy = user;
                item.CreatedByDateTime = dt;
            }
        }

        private async Task SendInvoiceStatusSyncMessage(Invoice invoiceEntity, string action)
        {
            await new InvoiceStatusSyncMessageHandler(_logger).SendInvoiceStatusSyncMessage(new InvoiceStatusSyncMessageDto()
            {
                Action = action,
                TimeStamp = DateTime.UtcNow,
                InvoiceId = invoiceEntity.InvoiceId,
                InvoiceNumber = invoiceEntity.InvoiceNumber,
                PaymentStatus = invoiceEntity.PaymentStatus,
                Details = invoiceEntity.InvoiceTimeReportCostDetails.Select(i => new InvoiceStatusSyncMessageDto.CostDetails()
                {
                    FlightReportCostDetailsId = i.FlightReportCostDetailsId,
                    FlightReportId = i.FlightReportId
                }).ToList()
            }, invoiceEntity.InvoiceNumber);
        }

        private async Task SendCreateInvoiceMessage(Invoice invoiceEntity)
        {
            var messageDetailInvoice = _mapper.Map<InvoiceDataSyncMessageDetailInvoiceDto>(invoiceEntity);
            messageDetailInvoice.Tables = new InvoiceDataSyncMessageDetailCostDto()
            {
                InvoiceTimeReportCostDetails = _mapper.Map<List<InvoiceTimeReportCostDetailDto>>(invoiceEntity.InvoiceTimeReportCostDetails),
                InvoiceOtherCostDetails = _mapper.Map<List<InvoiceOtherCostDetailDto>>(invoiceEntity.InvoiceOtherCostDetails)
            };
            await new InvoiceDataSyncMessageHandler(_logger).SendCreateInvoiceMessage(new InvoiceDataSyncMessageDto()
            {
                Action = "create-invoice",
                TimeStamp = DateTime.UtcNow,
                Tables = new InvoiceDataSyncMessageDetailDto() { Invoice = messageDetailInvoice }
            }, invoiceEntity.InvoiceNumber);
        }

        private async Task SendUpdateInvoiceMessage(Invoice invoiceRecord)
        {
            var messageDetailInvoice = _mapper.Map<InvoiceDataSyncMessageDetailInvoiceDto>(invoiceRecord);
            messageDetailInvoice.Tables = new InvoiceDataSyncMessageDetailCostDto()
            {
                InvoiceTimeReportCostDetails = _mapper.Map<List<InvoiceTimeReportCostDetailDto>>(invoiceRecord.InvoiceTimeReportCostDetails),
                InvoiceOtherCostDetails = _mapper.Map<List<InvoiceOtherCostDetailDto>>(invoiceRecord.InvoiceOtherCostDetails)
            };

            await new InvoiceDataSyncMessageHandler(_logger).SendUpdateInvoiceMessage(new InvoiceDataSyncMessageDto()
            {
                Action = "update-invoice",
                TimeStamp = DateTime.UtcNow,
                Tables = new InvoiceDataSyncMessageDetailDto() { Invoice = messageDetailInvoice }
            }, invoiceRecord.InvoiceNumber);
        }


    }
}
