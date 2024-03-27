using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    public class UpdateProcessedInvoice
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        OkObjectResult okResult = null;
        BadRequestObjectResult badRequestResult = null;

        string errorMessage = "Error : {0}, InnerException: {1}";

        public UpdateProcessedInvoice(IMapper mapper, IAuditLogService auditLogService)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        [FunctionName("UpdateProcessedInvoice")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post), Route = null)] HttpRequest req, ILogger _logger)
        {
            await _auditLogService.Audit("UpdateProcessedInvoice");
            _logger.LogInformation("Trigger function (UpdateProcessedInvoice) received a request.");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var invoiceObj = JsonConvert.DeserializeObject<InvoiceDto>(requestBody);
                if (invoiceObj != null)
                {
                    if (invoiceObj.InvoiceId == null || (invoiceObj.InvoiceId != null && invoiceObj.InvoiceId == Guid.Empty))
                    {
                        badRequestResult = new BadRequestObjectResult("Invalid Request: InvoiceId can not be null or empty");
                        badRequestResult.ContentTypes.Add("application/json");
                        return badRequestResult;
                    }
                    if (string.IsNullOrEmpty(invoiceObj.UniqueServiceSheetName))
                    {
                        badRequestResult = new BadRequestObjectResult("Invalid Request: UniqueServiceSheetName can not be null or empty");
                        badRequestResult.ContentTypes.Add("application/json");
                        return badRequestResult;
                    }
                    IInvoiceController iController = new InvoiceController(_logger, _mapper);
                    string result = await iController.UpdateProcessedInvoice(invoiceObj);
                    okResult = new OkObjectResult(result.ToString());
                    okResult.ContentTypes.Add("application/json");
                    return okResult;
                }
                else
                {
                    badRequestResult = new BadRequestObjectResult("Invalid Request");
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
