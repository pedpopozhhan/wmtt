using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.Services;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    /// <summary>
    /// This service pulls cost details from aviation for a flight report[s] based on the flight report id[s] passed.
    /// </summary>
    public class GetProcessedCostDetails
    {
        private readonly IMapper _mapper;

        public GetProcessedCostDetails(IMapper mapper)
        {
            _mapper = mapper;
        }


        [FunctionName("GetProcessedCostDetails")]
        public async Task<ActionResult<ProcessedCostDetailsResponseDto[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("Trigger function (GetProcessedCostDetails) received a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ProcessedCostDetailsRequestDto>(requestBody);

                if (data == null || data.FlightReportCostDetailIds == null || data.FlightReportCostDetailIds.Count() == 0)
                {
                    return new BadRequestObjectResult("Invalid Input: FlightReportCostDetailIds not found.");
                }
                if (data.FlightReportId == 0)
                {
                    return new BadRequestObjectResult("Invalid Input: FlightReportId is missing or not valid.");
                }

                var responseDto = new InvoiceController(log, _mapper).GetProcessedCostDetails(data);
                return new OkObjectResult(responseDto);

            }
            catch (Exception ex)
            {
                log.LogError("GetProcessedCostDetails: Error retrieving processed cost details - Message:{0}, StackTrace:{1}, InnerException:{2}", ex.Message, ex.StackTrace, ex.InnerException);
                return new InternalServerErrorResult();
            }

        }
    }
}
