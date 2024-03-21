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
using System.Collections.Generic;

namespace WCDS.WebFuncions
{
    public class UpdateInvoiceStatus
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        string errorMessage = "Error : {0}, InnerException: {1}";

        public UpdateInvoiceStatus(IMapper mapper, IAuditLogService auditLogService)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        [FunctionName("UpdateInvoiceStatus")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post), Route = null)] HttpRequest req, ILogger _logger)
        {
           string userName = await _auditLogService.Audit("UpdateInvoiceStatus");
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
                        return new BadRequestObjectResult(validationErrors);
                    }
                    IInvoiceController iController = new InvoiceController(_logger, _mapper);
                    invoiceObj.UpdatedBy = userName;
                    return new OkObjectResult(iController.UpdateInvoiceStatus(invoiceObj));
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
