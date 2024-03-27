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
using AutoMapper;
using WCDS.WebFuncions.Core.Services;
using System.Linq;

namespace WCDS.WebFuncions
{
    public class CreateInvoice
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        OkObjectResult okResult = null;
        BadRequestObjectResult badRequestResult = null;

        public CreateInvoice(IMapper mapper, IAuditLogService auditLogService)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        [FunctionName("CreateInvoice")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Put), Route = null)] HttpRequest req, ILogger _logger)
        {
            await _auditLogService.Audit("CreateInvoice");
            _logger.LogInformation("Trigger function (CreateInvoice) received a request.");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var invoiceObj = JsonConvert.DeserializeObject<InvoiceDto>(requestBody);
                if (invoiceObj != null)
                {
                    IInvoiceController iController = new InvoiceController(_logger, _mapper);
                    InvoiceValidator validationRules = new InvoiceValidator(iController);

                    var validationResult = validationRules.Validate(invoiceObj);
                    if (!validationResult.IsValid)
                    {
                        badRequestResult = new BadRequestObjectResult(validationResult.Errors.Select(i => i.ErrorMessage).ToList());
                        badRequestResult.ContentTypes.Add("application/json");
                        return badRequestResult;
                    }

                    var result = await iController.CreateInvoice(invoiceObj);
                    okResult = new OkObjectResult(result);
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
