using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WCDS.WebFuncions.Core.Common;
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
        public string GetInvoiceByKey(int invoiceKey);
        public string GetInvoiceByID(string invoiceID);
    }

    public class InvoiceController: IInvoiceController
    {
        private Mapper mapper;
        ApplicationDBContext dbContext;
        ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        public InvoiceController(ILogger log)
        {
            mapper = MapperConfig.InitializeAutomapper();
            dbContext = new ApplicationDBContext();
            _logger = log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public int CreateInvoice(InvoiceDto invoice)
        {
            int result = 0;            
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Invoice invoiceEntity = mapper.Map<Invoice>(invoice);
                    invoiceEntity.AuditCreationDateTime = DateTime.Now;
                    invoiceEntity.AuditLastUpdateDateTime = invoiceEntity.AuditCreationDateTime;
                    dbContext.Invoice.Add(invoiceEntity);
                    dbContext.SaveChanges();
                    invoice.InvoiceKey = invoiceEntity.InvoiceKey;
                    result = invoice.InvoiceKey;
                    if (invoice.TimeReports != null)
                    {
                        foreach (var report in invoice.TimeReports)
                        {
                            InvoiceTimeReport tr = mapper.Map<InvoiceTimeReport>(report);
                            tr.InvoiceKey = invoice.InvoiceKey;
                            tr.AuditCreationDateTime = DateTime.Now;
                            tr.AuditLastUpdateDateTime = tr.AuditCreationDateTime;
                            dbContext.InvoiceTimeReport.Add(tr);
                            dbContext.SaveChanges();
                            if (report.InvoiceDetails != null)
                            {
                                foreach (var detail in report.InvoiceDetails)
                                {
                                    InvoiceDetail invoiceDetail = mapper.Map<InvoiceDetail>(detail);
                                    invoiceDetail.InvoiceTimeReportKey = tr.InvoiceTimeReportKey;
                                    invoiceDetail.AuditCreationDateTime = DateTime.Now;
                                    invoiceDetail.AuditLastUpdateDateTime = invoiceDetail.AuditCreationDateTime;
                                    dbContext.InvoiceDetail.Add(invoiceDetail);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch
                {
                    _logger.LogError("An error has occured while Saving Invoice: " + invoice.InvoiceID);
                    transaction.Rollback();
                    throw;
                }
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public int UpdateInvoice(InvoiceDto invoice)
        {
            int result = invoice.InvoiceKey;            
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Invoice preInvoice = dbContext.Invoice.Where(x => x.InvoiceKey == invoice.InvoiceKey).FirstOrDefault();
                    preInvoice.InvoiceID = invoice.InvoiceID;
                    preInvoice.DateOnInvoice = invoice.DateOnInvoice.Value;
                    preInvoice.InvoiceAmount = invoice.InvoiceAmount.Value;
                    preInvoice.PeriodEndDate = invoice.PeriodEndDate.Value;
                    preInvoice.InvoiceReceivedDate = invoice.InvoiceReceivedDate;
                    preInvoice.AuditLastUpdateDateTime = DateTime.Now;
                    dbContext.Invoice.Update(preInvoice);
                    dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    _logger.LogError("An error has occured while Saving Invoice: " + invoice.InvoiceID);
                    transaction.Rollback();
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <returns></returns>
        public bool InvoiceExists(string invoiceID)
        {
            bool bResult = false;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Invoice invoice = dbContext.Invoice.Where(x => x.InvoiceID == invoiceID).FirstOrDefault();
                    if (invoice != null)
                        bResult = true;
                }
                catch
                {
                    _logger.LogError("An error has occured while Saving Invoice: " + invoiceID);
                    transaction.Rollback();
                    throw;
                }
            }
            return bResult;
        }


        public string GetInvoiceByKey(int invoiceKey)
        {
            InvoiceDto invoiceDto = null;
            string result = string.Empty;
            try
            {
                Invoice invoice = dbContext.Invoice.Include(p => p.TimeReports).Where(x => x.InvoiceKey == invoiceKey).FirstOrDefault();
                invoiceDto = mapper.Map<InvoiceDto>(invoice);

                var opt = new JsonSerializerOptions() { WriteIndented = true };
                result = JsonSerializer.Serialize<InvoiceDto>(invoiceDto, opt);
            }
            catch
            {
                _logger.LogError("An error has occured while Retrieving Invoice: " + invoiceKey);
                throw;
            }
            return result;
        }

        public string GetInvoiceByID(string invoiceID)
        {
            InvoiceDto invoiceDto = null;
            string result = string.Empty;
            try
            {
                Invoice invoice = dbContext.Invoice.Include(p => p.TimeReports).Where(x => x.InvoiceID == invoiceID).FirstOrDefault();
                invoiceDto = mapper.Map<InvoiceDto>(invoice);

                var opt = new JsonSerializerOptions() { WriteIndented = true };
                result = JsonSerializer.Serialize<InvoiceDto>(invoiceDto, opt);
            }
            catch
            {
                _logger.LogError("An error has occured while Retrieving Invoice: " + invoiceID);
                throw;
            }
            return result;
        }
    }
}
