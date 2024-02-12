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


namespace WCDS.WebFuncions
{
    public class GetInvoiceDetails
    {
        private readonly IMapper _mapper;

        public GetInvoiceDetails(IMapper mapper)
        {
            _mapper = mapper;
        }
        [FunctionName("GetInvoiceDetails")]
        public async Task<ActionResult<InvoiceDetailResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Trigger function (GetInvoiceDetails) received a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<InvoiceDetailRequestDto>(requestBody);

            if (data != null)
            {
                var responseDto = new InvoiceController(log, _mapper).GetInvoiceDetails(data);
                return new JsonResult(responseDto);
            }

            log.LogError("Either invalid request, or an error retrieving details of invoice");
            throw new Exception("Either invalid request, or an error retrieving details of invoice");
        }
    }
}