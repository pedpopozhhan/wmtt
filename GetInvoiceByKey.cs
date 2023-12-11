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
    public static class GetInvoiceByKey
    {
        [FunctionName("GetInvoiceByKey")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post), Route = null)] HttpRequest req, ILogger _logger)
        {
            _logger.LogInformation("Trigger function (GetInvoiceByKey) received a request.");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var requestObj = JsonConvert.DeserializeObject<InvoiceByKeyRequestDto>(requestBody);

                IInvoiceController iController = new InvoiceController(_logger);
                InvoiceByKeyValidator validationRules = new InvoiceByKeyValidator();
                validationRules.ValidateAndThrow(requestObj);

                string responseMessage = iController.GetInvoiceByKey(requestObj.InvoiceKey);
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new BadRequestObjectResult(ex.ToString() + ex.StackTrace);
            }
        }
    }
}
