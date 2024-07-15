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
        public Task<Guid> DeleteDraft(Guid invoiceId, string user);
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

        /*
        If we are here, there is no existing invoice
        */
        public async Task<Guid> CreateInvoice(InvoiceDto invoice)
        {
            Guid result = Guid.Empty;
            int numberOfCostDetails = 0, numberOfOtherCostDetails = 0;

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
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation("Ivoice Insert Completed at: {0} for invoice: {1}", DateTime.UtcNow, invoiceEntity.InvoiceNumber);

                    var entity = await dbContext.Invoice.Where(x => x.InvoiceId == invoice.InvoiceId)
                    .Include(i => i.InvoiceTimeReportCostDetails)
                    .Include(i => i.InvoiceOtherCostDetails)
                    .Include(i => i.InvoiceStatusLogs)
                    .Include(i => i.InvoiceTimeReports).FirstOrDefaultAsync();

                    if (entity == null)
                    {
                        throw new System.Exception($"No Invoice found for InvoiceId - {invoice.InvoiceId} in the Database.");
                    }

                    // update-invoice to data team

                    // all the child records are inserts
                    // TODO waiting on clarification
                    // var dataPayload = CreateDataSyncInsertPayload(entity, entity.InvoiceTimeReportCostDetails, entity.InvoiceOtherCostDetails, entity.InvoiceTimeReports);
                    // await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(dataPayload, entity.InvoiceNumber, "insert-invoice");

                    var toInsert = entity.InvoiceTimeReportCostDetails;
                    var payload = CreateStatusSyncPayload(entity, "update-invoice", new List<InvoiceTimeReportCostDetails>(), new List<InvoiceTimeReportCostDetails>(), entity.InvoiceTimeReportCostDetails);
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
            }
            return result;
        }

        public async Task<string> UpdateProcessedInvoice(InvoiceDto invoice)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var newInvoice = new Invoice
                    {
                        InvoiceId = invoice.InvoiceId.Value,
                        UniqueServiceSheetName = invoice.UniqueServiceSheetName,
                        UpdatedBy = invoice.UpdatedBy,
                        UpdatedByDateTime = DateTime.UtcNow,
                        InvoiceStatus = InvoiceStatus.Processed.ToString()
                    };

                    dbContext.Attach(newInvoice);

                    // Mark specific properties as modified
                    dbContext.Entry(newInvoice).Property(x => x.UniqueServiceSheetName).IsModified = true;
                    dbContext.Entry(newInvoice).Property(x => x.UpdatedBy).IsModified = true;
                    dbContext.Entry(newInvoice).Property(x => x.UpdatedByDateTime).IsModified = true;
                    dbContext.Entry(newInvoice).Property(x => x.InvoiceStatus).IsModified = true;
                    // Save changes to the database
                    await dbContext.SaveChangesAsync();
                    transaction.Commit();
                    var entity = dbContext.Invoice.FirstOrDefault(ss => ss.InvoiceId == invoice.InvoiceId);
                    if (entity == null)
                    {
                        throw new System.Exception($"No Invoice found for InvoiceId - {invoice.InvoiceId} in the Database.");
                    }
                    // TODO waiting on clarification
                    // var dataPayload = CreateDataSyncUpdatePayload(entity, new List<InvoiceTimeReportCostDetails>(), new List<InvoiceOtherCostDetails>(), new List<InvoiceTimeReports>());
                    // await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(dataPayload, entity.InvoiceNumber, "update-invoice");

                    return entity.UniqueServiceSheetName;
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("UpdateProcessedInvoice: An error has occured while Updating Invoice for Invoice Number:  {0}, ErrorMessage: {1}, InnerException: {2}", invoice.InvoiceNumber, ex.Message, ex.InnerException));
                    transaction.Rollback();
                    throw;
                }
            }
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

                    //// insert-invoice to data team
                    // TODO waiting on clarification
                    // var dataPayload = CreateDataSyncInsertPayload(entity, entity.InvoiceTimeReportCostDetails, entity.InvoiceOtherCostDetails, entity.InvoiceTimeReports);
                    // await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(dataPayload, entity.InvoiceNumber, "insert-invoice");

                    var toInsert = entity.InvoiceTimeReportCostDetails;
                    var payload = CreateStatusSyncPayload(entity, "update-invoice", toInsert, new List<InvoiceTimeReportCostDetails>(), new List<InvoiceTimeReportCostDetails>());
                    await new InvoiceStatusSyncMessageHandler(_logger).SendInvoiceStatusSyncMessage(payload, entity.InvoiceNumber);

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
            Invoice entity = null;
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

                    var costDetailsToUpdate = new List<InvoiceTimeReportCostDetails>();
                    var costDetailsToRemove = new List<InvoiceTimeReportCostDetails>();
                    var costDetailsToAdd = new List<InvoiceTimeReportCostDetails>();
                    IList<InvoiceTimeReportCostDetails> costDetailsUnChanged = ProcessTimeReportCostDetails(invoice, entity, ref costDetailsToAdd, ref costDetailsToUpdate, ref costDetailsToRemove);
                    dbContext.InvoiceTimeReportCostDetails.RemoveRange(costDetailsToRemove);

                    var timeReportsToUpdate = new List<InvoiceTimeReports>();
                    var timeReportsToRemove = new List<InvoiceTimeReports>();
                    var timeReportsToAdd = new List<InvoiceTimeReports>();
                    IEnumerable<InvoiceTimeReports> timeReportsUnChanged = ProcessTimeReports(invoice, entity, ref timeReportsToAdd, ref timeReportsToUpdate, ref timeReportsToRemove);
                    dbContext.InvoiceTimeReports.RemoveRange(timeReportsToRemove);
                    // savechanges?

                    var otherCostDetailsToUpdate = new List<InvoiceOtherCostDetails>();
                    var otherCostDetailsToRemove = new List<InvoiceOtherCostDetails>();
                    var otherCostDetailsToAdd = new List<InvoiceOtherCostDetails>();
                    IList<InvoiceOtherCostDetails> otherCostDetailsUnChanged = ProcessOtherCostDetails(invoice, entity, ref otherCostDetailsToAdd, ref otherCostDetailsToUpdate, ref otherCostDetailsToRemove);
                    dbContext.InvoiceOtherCostDetails.RemoveRange(otherCostDetailsToRemove);

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

                    entity.InvoiceTimeReportCostDetails = costDetailsToUpdate.Concat(costDetailsToAdd).ToList();
                    entity.InvoiceOtherCostDetails = otherCostDetailsToUpdate.Concat(otherCostDetailsToAdd).ToList();
                    entity.InvoiceTimeReports = timeReportsToUpdate.Concat(timeReportsToAdd).ToList();

                    // save the modified entities
                    foreach (var timeReportCostDetail in costDetailsToUpdate)
                    {
                        timeReportCostDetail.UpdatedBy = user;
                        timeReportCostDetail.UpdatedByDateTime = dt;
                        dbContext.Entry(timeReportCostDetail).State = EntityState.Modified;
                    }

                    foreach (var timeReportCostDetail in costDetailsToAdd)
                    {
                        timeReportCostDetail.CreatedBy = user;
                        timeReportCostDetail.CreatedByDateTime = dt;
                        timeReportCostDetail.UpdatedBy = user;
                        timeReportCostDetail.UpdatedByDateTime = dt;
                        dbContext.Entry(timeReportCostDetail).State = EntityState.Added;
                    }

                    foreach (var otherCostDetail in otherCostDetailsToUpdate)
                    {
                        otherCostDetail.UpdatedBy = user;
                        otherCostDetail.UpdatedByDateTime = dt;
                        dbContext.Entry(otherCostDetail).State = EntityState.Modified;
                    }

                    foreach (var otherCostDetail in costDetailsToAdd)
                    {
                        otherCostDetail.CreatedBy = user;
                        otherCostDetail.CreatedByDateTime = dt;
                        otherCostDetail.UpdatedBy = user;
                        otherCostDetail.UpdatedByDateTime = dt;
                        dbContext.Entry(otherCostDetail).State = EntityState.Added;
                    }

                    foreach (var timeReport in timeReportsToUpdate)
                    {
                        timeReport.AuditLastUpdatedBy = user;
                        timeReport.AuditLastUpdatedDateTime = dt;
                        dbContext.Entry(timeReport).State = EntityState.Modified;
                    }

                    foreach (var timeReport in timeReportsToAdd)
                    {
                        timeReport.AuditCreationDateTime = dt;
                        timeReport.AuditLastUpdatedBy = user;
                        timeReport.AuditLastUpdatedDateTime = dt;
                        dbContext.Entry(timeReport).State = EntityState.Added;
                    }

                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    // update-invoice to data team
                    // send messages
                    var statusPayload = CreateStatusSyncPayload(entity, "update-invoice", costDetailsToAdd, costDetailsToRemove, costDetailsUnChanged);
                    await new InvoiceStatusSyncMessageHandler(_logger).SendInvoiceStatusSyncMessage(statusPayload, entity.InvoiceNumber);

                    // TODO waiting on clarification
                    // var dataPayload = CreateDataSyncUpdatePayload(entity, entity.InvoiceTimeReportCostDetails, entity.InvoiceOtherCostDetails, entity.InvoiceTimeReports);
                    // await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(dataPayload, entity.InvoiceNumber, "update-invoice");

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

        private IList<InvoiceTimeReportCostDetails> ProcessTimeReportCostDetails(InvoiceRequestDto invoice, Invoice entity, ref List<InvoiceTimeReportCostDetails> costDetailsToAdd, ref List<InvoiceTimeReportCostDetails> costDetailsToUpdate, ref List<InvoiceTimeReportCostDetails> costDetailsToRemove)
        {
            // get the list from the database
            var usedIds = new List<Guid>();
            foreach (var e in entity.InvoiceTimeReportCostDetails)
            {
                if (invoice.InvoiceTimeReportCostDetails.Any(x => x.FlightReportCostDetailsId == e.FlightReportCostDetailsId))
                {
                    usedIds.Add(e.FlightReportCostDetailsId);
                    costDetailsToUpdate.Add(e);
                }
                else
                {
                    usedIds.Add(e.FlightReportCostDetailsId);
                    costDetailsToRemove.Add(e);
                }
            }
            var toAdd = usedIds.Count == 0 ? invoice.InvoiceTimeReportCostDetails : invoice.InvoiceTimeReportCostDetails.Where(x => !usedIds.Contains(x.FlightReportCostDetailsId)).ToList();
            costDetailsToAdd = _mapper.Map<IList<InvoiceTimeReportCostDetails>>(toAdd).ToList();
            var costDetailsUnChanged = entity.InvoiceTimeReportCostDetails.Where(x => !usedIds.Contains(x.FlightReportCostDetailsId)).ToList();

            return costDetailsUnChanged;
        }

        private IList<InvoiceOtherCostDetails> ProcessOtherCostDetails(InvoiceRequestDto invoice, Invoice entity, ref List<InvoiceOtherCostDetails> toAdd, ref List<InvoiceOtherCostDetails> toUpdate, ref List<InvoiceOtherCostDetails> toRemove)
        {
            var usedIds = new List<Guid>();
            foreach (var e in entity.InvoiceOtherCostDetails)
            {
                if (invoice.InvoiceOtherCostDetails.Any(x => x.InvoiceOtherCostDetailId == e.InvoiceOtherCostDetailId))
                {
                    usedIds.Add(e.InvoiceOtherCostDetailId);
                    toUpdate.Add(e);
                }
                else
                {
                    usedIds.Add(e.InvoiceOtherCostDetailId);
                    toRemove.Add(e);
                }
            }
            var dtoToAdd = usedIds.Count == 0 ? invoice.InvoiceOtherCostDetails : invoice.InvoiceOtherCostDetails.Where(x => !usedIds.Contains(x.InvoiceOtherCostDetailId)).ToList();
            toAdd = _mapper.Map<IList<InvoiceOtherCostDetails>>(dtoToAdd).ToList();
            var unChanged = entity.InvoiceOtherCostDetails.Where(x => !usedIds.Contains(x.InvoiceOtherCostDetailId)).ToList();

            return unChanged;
        }
        private static IEnumerable<InvoiceTimeReports> ProcessTimeReports(InvoiceRequestDto invoice, Invoice entity, ref List<InvoiceTimeReports> toAdd, ref List<InvoiceTimeReports> toUpdate, ref List<InvoiceTimeReports> toRemove)
        {
            var usedIds = new List<int>();
            foreach (var e in entity.InvoiceTimeReports)
            {
                if (invoice.FlightReportIds.Any(x => x == e.FlightReportId))
                {
                    usedIds.Add(e.FlightReportId);
                    toUpdate.Add(e);
                }
                else
                {
                    usedIds.Add(e.FlightReportId);
                    toRemove.Add(e);
                }
            }
            var reportsToAdd = usedIds.Count == 0 ? invoice.FlightReportIds : invoice.FlightReportIds.Where(x => !usedIds.Contains(x));
            toAdd = reportsToAdd.Select(x =>
                    {
                        return new InvoiceTimeReports
                        {
                            FlightReportId = x,
                            InvoiceId = entity.InvoiceId,

                        };
                    }).ToList();

            var unChanged = entity.InvoiceTimeReports.Where(x => !usedIds.Contains(x.FlightReportId));
            return unChanged;
        }

        public async Task<Guid> DeleteDraft(Guid invoiceId, string user)
        {
            using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var newInvoice = new Invoice
                {
                    InvoiceId = invoiceId,
                    InvoiceStatus = InvoiceStatus.DraftDeleted.ToString(),
                    UpdatedBy = user,
                    UpdatedByDateTime = DateTime.UtcNow,
                };

                dbContext.Attach(newInvoice);

                dbContext.Entry(newInvoice).Property(x => x.InvoiceStatus).IsModified = true;
                dbContext.Entry(newInvoice).Property(x => x.UpdatedBy).IsModified = true;
                dbContext.Entry(newInvoice).Property(x => x.UpdatedByDateTime).IsModified = true;
                // Save changes to the database
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                var entity = await dbContext.Invoice.FirstOrDefaultAsync(ss => ss.InvoiceId == invoiceId) ?? throw new System.Exception($"No Invoice found for InvoiceId - {invoiceId} in the Database.");
                // TODO waiting on clarification
                // var dataPayload = CreateDataSyncDeletePayload(entity, entity.InvoiceTimeReportCostDetails, entity.InvoiceOtherCostDetails, entity.InvoiceTimeReports);
                // await new InvoiceDataSyncMessageHandler(_logger).SendInvoiceDataSyncMessage(dataPayload, entity.InvoiceNumber, "delete-invoice");

                // delete-invoice to data team
                var payload = CreateStatusSyncPayload(entity, "update-invoice", new List<InvoiceTimeReportCostDetails>(), new List<InvoiceTimeReportCostDetails>(), new List<InvoiceTimeReportCostDetails>());
                await new InvoiceStatusSyncMessageHandler(_logger).SendInvoiceStatusSyncMessage(payload, entity.InvoiceNumber);
                return entity.InvoiceId;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("DeleteDraft: An error has occured while Deleting Invoice for InvoiceId:  {0}, ErrorMessage: {1}, InnerException: {2}", invoiceId, ex.Message, ex.InnerException));
                await transaction.RollbackAsync();
                throw;
            }
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

                        // TODO waiting on clarification
                        // await SendUpdateInvoiceMessage(invoiceRecord);
                        // await new InvoiceDataSyncMessageHandler(_logger).SendUpdateInvoiceMessage();

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
            var messageDetailInvoice = new InvoiceDataSyncMessageDetailInvoiceDto
            {
                Tables = new InvoiceDataSyncMessageDetailCostDto
                {
                    InvoiceOtherCostDetails = _mapper.Map<List<InvoiceOtherCostDetailDto>>(other),
                    InvoiceTimeReportCostDetails = _mapper.Map<List<InvoiceTimeReportCostDetailDto>>(costDetails),
                    InvoiceTimeReports = _mapper.Map<List<InvoiceTimeReportsDto>>(timeReports)
                }
            };
            // only updated records
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
        private InvoiceDataSyncMessageDto CreateDataSyncDeletePayload(Invoice invoiceEntity,
        IList<InvoiceTimeReportCostDetails> costDetails, IList<InvoiceOtherCostDetails> other, IList<InvoiceTimeReports> timeReports)
        {
            var messageDetailInvoice = new InvoiceDataSyncMessageDetailInvoiceDto
            {
                Tables = new InvoiceDataSyncMessageDetailCostDto
                {
                    InvoiceOtherCostDetails = _mapper.Map<List<InvoiceOtherCostDetailDto>>(other),
                    InvoiceTimeReportCostDetails = _mapper.Map<List<InvoiceTimeReportCostDetailDto>>(costDetails),
                    InvoiceTimeReports = _mapper.Map<List<InvoiceTimeReportsDto>>(timeReports)
                }
            };
            // only updated records
            var payload = new InvoiceDataSyncMessageDto
            {
                Action = "delete-invoice",
                TimeStamp = DateTime.UtcNow,
                Tables = new InvoiceDataSyncMessageDetailDto
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

    }
}
