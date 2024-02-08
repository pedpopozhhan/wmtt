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
using Microsoft.AspNetCore.Mvc;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Controller
{
    public interface IInvoiceController
    {
        public int CreateInvoice(InvoiceDto invoice);
        public int UpdateInvoice(InvoiceDto invoice);
        public bool InvoiceExists(string invoiceID);
        public InvoiceResponseDto GetInvoices(InvoiceRequestDto invoiceRequest);
    }

    public class InvoiceController : IInvoiceController
    {
        ApplicationDBContext dbContext;
        ILogger _logger;
        IMapper _mapper;

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
    }
}
