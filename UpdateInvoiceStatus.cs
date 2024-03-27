using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    public class UpdateInvoiceStatus
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        OkObjectResult okResult = null;
        BadRequestObjectResult badRequestResult = null;

        string errorMessage = "Error : {0}, InnerException: {1}";

        public UpdateInvoiceStatus(IMapper mapper, IAuditLogService auditLogService)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        [FunctionName("UpdateInvoiceStatus")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post), Route = null)] HttpRequest req, ILogger _logger)
        {
           // await _auditLogService.Audit("UpdateInvoiceStatus");
            _logger.LogInformation("Trigger function (UpdateInvoiceStatus) received a request.");
            try
           {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var invoiceObj = JsonConvert.DeserializeObject<UpdateInvoiceStatusRequestDto>(requestBody);

               
                if (invoiceObj != null)
                {
                    List<string> validationErrors = new List<string>();
                    if (invoiceObj.InvoiceId == null || (invoiceObj.InvoiceId != null  && invoiceObj.InvoiceId == Guid.Empty))
                    {
                        validationErrors.Add("Invalid Request: InvoiceId can not be empty or null.");
                    }
                    if (string.IsNullOrEmpty(invoiceObj.PaymentStatus))
                    {
                        validationErrors.Add("Invalid Request: PaymentStatus can not be empty or null.");
                    }
                    else
                    {
                        if(invoiceObj.PaymentStatus != Enums.PaymentStatus.Submitted.ToString() && invoiceObj.PaymentStatus != Enums.PaymentStatus.Posted.ToString() && invoiceObj.PaymentStatus != Enums.PaymentStatus.Cleared.ToString())
                        {
                            validationErrors.Add("Invalid Request: PaymentStatus can not be other than Submitted, Posted or Cleared.");
                        }
                    }
                    if (!invoiceObj.UpdatedDateTime.HasValue)
                    {
                        validationErrors.Add("Invalid Request: UpdateDateTime can not be empty or null.");
                    }

                    if (validationErrors.Count > 0)
                    {
                        badRequestResult = new BadRequestObjectResult(validationErrors);
                        badRequestResult.ContentTypes.Add("application/json");
                        return badRequestResult;
                    }
                    IInvoiceController iController = new InvoiceController(_logger, _mapper);
                    okResult = new OkObjectResult(iController.UpdateInvoiceStatus(invoiceObj));
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
