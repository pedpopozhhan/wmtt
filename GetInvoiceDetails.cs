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
        string errorMessage = "Error : {0}, InnerException: {1}";

        public GetInvoiceDetails(IMapper mapper)
        {
            _mapper = mapper;
        }
        [FunctionName("GetInvoiceDetails")]
        public async Task<ActionResult<InvoiceDetailResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("Trigger function (GetInvoiceDetails) received a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<InvoiceDetailRequestDto>(requestBody);

                if (data != null)
                {
                    var responseDto = new InvoiceController(log, _mapper).GetInvoiceDetails(data);
                    return new JsonResult(responseDto);
                }
                else
                {
                    return new BadRequestObjectResult("Invalid Request");
                }

            }

            catch (Exception ex)
            {
                log.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                var result = new ObjectResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }
    }
}