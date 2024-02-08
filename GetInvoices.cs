using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;
using AutoMapper;
using WCDS.WebFuncions.Controller;

namespace WCDS.WebFuncions
{
    public class GetInvoices
    {
        
        private readonly IMapper _mapper;

        public GetInvoices( IMapper mapper)
        {
            _mapper = mapper;
        }

        [FunctionName("GetInvoices")]
        public async Task<ActionResult<InvoiceResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Trigger function (GetInvoices) received a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<InvoiceRequestDto>(requestBody);

            if (data != null)
            {
                var responseDto = new InvoiceController(log,_mapper).GetInvoices(data);
                return new JsonResult(responseDto);
            }
            log.LogError("Either invalid request, or an error retrieving invoices");
            throw new Exception("Either invalid request, or an error retrieving invoices");
        }
    }
}
