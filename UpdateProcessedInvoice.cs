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
using FluentValidation;
using AutoMapper;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    public class UpdateProcessedInvoice
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

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
                    if (string.IsNullOrEmpty(invoiceObj.UniqueServiceSheetName))
                    {
                        return new BadRequestObjectResult("Invalid Request: UniqueServiceSheetName can not be null or empty");
                    }
                    IInvoiceController iController = new InvoiceController(_logger, _mapper);
                    string result = await iController.UpdateProcessedInvoice(invoiceObj);
                    return new OkObjectResult(result.ToString());
                }
                else
                {
                    return new BadRequestObjectResult("Invalid Request");
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
