using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    public class DoesInvoiceNumberExist
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ApplicationDBContext _dbContext;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public DoesInvoiceNumberExist(IMapper mapper, IAuditLogService auditLogService, ApplicationDBContext dbContext)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            _dbContext = dbContext;
        }

        [FunctionName("DoesInvoiceNumberExist")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Get), Route = null)] HttpRequest req, ILogger _logger)
        {
            await _auditLogService.Audit("DoesInvoiceNumberExist");
            _logger.LogInformation("Trigger function (DoesInvoiceNumberExist) received a request.");
            try
            {

                if (!Guid.TryParse(req.Query["invoiceId"], out Guid invoiceId))
                {
                    jsonResult = new JsonResult("invoiceId must be a Guid")
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    return jsonResult;
                }
                var invoiceNumber = req.Query["invoiceNumber"];
                var contractNumber = req.Query["contractNumber"];

                if (!string.IsNullOrEmpty(invoiceNumber) && !string.IsNullOrEmpty(contractNumber))
                {
                    IInvoiceController iController = new InvoiceController(_logger, _mapper, _dbContext);
                    var exists = iController.InvoiceExistsForContract(invoiceId, invoiceNumber, contractNumber);

                    jsonResult = new JsonResult(exists);
                    jsonResult.StatusCode = StatusCodes.Status200OK;
                    return jsonResult;
                }
                else
                {
                    jsonResult = new JsonResult("invoiceNumber and/or contractNumber missing from query params");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult = new JsonResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult.StatusCode = StatusCodes.Status500InternalServerError;
                return jsonResult;
            }
        }
    }
}
