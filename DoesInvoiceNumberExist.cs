using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Validator;
using WCDS.WebFuncions.Core.Services;
using AutoMapper;
using System.Linq;

namespace WCDS.WebFuncions
{
    public class DoesInvoiceNumberExist
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        public DoesInvoiceNumberExist(IMapper mapper, IAuditLogService auditLogService)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        [FunctionName("DoesInvoiceNumberExist")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Get), Route = null)] HttpRequest req, ILogger _logger)
        {
            await _auditLogService.Audit("DoesInvoiceNumberExist");
            _logger.LogInformation("Trigger function (DoesInvoiceNumberExist) received a request.");
            try
            {

                var invoiceNumber = req.Query["invoiceNumber"];

                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    IInvoiceController iController = new InvoiceController(_logger, _mapper);

                    var exists = iController.InvoiceExists(invoiceNumber);
                    return new OkObjectResult(exists);
                }
                else
                {
                    return new BadRequestObjectResult("invoiceNumber missing from query params");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                var result = new ObjectResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }
    }
}