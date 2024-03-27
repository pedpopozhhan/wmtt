using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    public class DoesInvoiceNumberExist
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        OkObjectResult okResult = null;
        BadRequestObjectResult badRequestResult = null;

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
                    okResult = new OkObjectResult(exists);
                    okResult.ContentTypes.Add("application/json");
                    return okResult;
                }
                else
                {
                    badRequestResult = new BadRequestObjectResult("invoiceNumber missing from query params");
                    badRequestResult.ContentTypes.Add("application/json");
                    return badRequestResult;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                var result = new ObjectResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.ContentTypes.Add("application/json");
                return result;
            }
        }
    }
}
