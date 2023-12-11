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

namespace WCDS.WebFuncions
{
    public static class UpdateInvoice
    {
        [FunctionName("UpdateInvoice")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Put), Route = null)] HttpRequest req, ILogger _logger)
        {
            _logger.LogInformation("Trigger function (UpdateInvoice) received a request.");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var invoiceObj = JsonConvert.DeserializeObject<InvoiceDto>(requestBody);

                IInvoiceController iController = new InvoiceController(_logger);
                InvoiceValidator validationRules = new InvoiceValidator(iController);
                validationRules.ValidateAndThrow(invoiceObj);

                int result = iController.UpdateInvoice(invoiceObj);
                return new OkObjectResult(result.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new BadRequestObjectResult(ex.ToString() + ex.StackTrace);
            }
        }
    }
}
