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
    public class GetCostDetails
    {
        private readonly IMapper _mapper;

        public GetCostDetails(IMapper mapper)
        {
            _mapper = mapper;
        }


        [FunctionName("GetCostDetails")]
        public async Task<ActionResult<CostDetailsResponseDto[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("Trigger function (GetCostDetails) received a request.");
                List<string> validationErrors = new List<string>();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<CostDetailsRequestDto>(requestBody);

                if (data == null)
                {
                    validationErrors.Add("Invalid Request: Request can not be null or empty.");
                    return new BadRequestObjectResult(validationErrors);
                }
                else
                {
                    if (data.FlightReportId == null || data.FlightReportId == 0)
                    {
                        validationErrors.Add("Invalid Request: FlightReportId can not be null or 0");
                    }
                    if (data.FlightReportCostDetailIds == null || data.FlightReportCostDetailIds.Count() == 0)
                    {
                        validationErrors.Add("Invalid Request: FlightReportCostDetailIds can not be empty or null");
                    }
                }

                if(validationErrors.Count > 0)
                {
                    return new BadRequestObjectResult(validationErrors);
                }
               
                var responseDto = new InvoiceController(log, _mapper).GetCostDetails(data);
                return new OkObjectResult(responseDto);

            }
            catch
            {
                log.LogError("GetCostDetails: Error retrieving processed cost details.");
                return new InternalServerErrorResult();
            }

        }
    }
}
