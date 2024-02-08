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
    public class UpdateInvoiceServiceSheet
    {
        private readonly IMapper _mapper;

        public UpdateInvoiceServiceSheet(IMapper mapper)
        {
            _mapper = mapper;
        }

        [FunctionName("UpdateInvoiceServiceSheet")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post), Route = null)] HttpRequest req, ILogger _logger)
        {
            _logger.LogInformation("Trigger function (UpdateInvoiceServiceSheet) received a request.");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var invoiceObj = JsonConvert.DeserializeObject<UpdateServiceSheetDto>(requestBody);

                IInvoiceController iController = new InvoiceController(_logger, _mapper);

                string result = iController.UpdateInvoiceServiceSheet(invoiceObj);
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
