using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WCDS.WebFuncions.Core.Common;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WCDS.WebFuncions.Controller
{
    public interface IInvoiceController
    {
        public int CreateInvoice(InvoiceDto invoice);
        public int UpdateInvoice(InvoiceDto invoice);
        public bool InvoiceExists(string invoiceID);
    }

    public class InvoiceController: IInvoiceController
    {
        private Mapper mapper;
        ApplicationDBContext dbContext;
        ILogger _logger;

        public InvoiceController(ILogger log)
        {
            mapper = MapperConfig.InitializeAutomapper();
            dbContext = new ApplicationDBContext();
            _logger = log;
        }


        public int CreateInvoice(InvoiceDto invoice)
        {
            int result = 0;
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Invoice invoiceEntity = mapper.Map<Invoice>(invoice);
                    dbContext.Invoice.Add(invoiceEntity);
                    dbContext.SaveChanges();
                    invoice.InvoiceId = invoiceEntity.InvoiceId;
                    result = invoice.InvoiceId;
                    transaction.Commit();
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

    }
}
