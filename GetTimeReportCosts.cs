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
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.Services;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    /// <summary>
    /// This service pulls cost details from aviation for a flight report[s] based on the flight report id[s] passed.
    /// </summary>
    public class GetTimeReportCosts
    {
        private readonly ITimeReportingService TimeReportingService;
        private readonly IMapper Mapper;
        string errorMessage = "Error : {0}, InnerException: {1}";

        public GetTimeReportCosts(ITimeReportingService timeReportingService, IMapper mapper)
        {

            TimeReportingService = timeReportingService;
            Mapper = mapper;
        }


        [FunctionName("GetTimeReportCosts")]
        public async Task<ActionResult<TimeReportDetailsResponse[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("Trigger function (GetTimeReportCosts) received a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<TimeReportCostsRequest>(requestBody);

                if (data != null)
                {
                    var costs = await TimeReportingService.GetTimeReportCosts(data.ContractNumber, data.Status);
                    if (!string.IsNullOrEmpty(costs.ErrorMessage))
                    {
                        throw new Exception(costs.ErrorMessage);
                    }

                    var response = new TimeReportCostsResponse
                    {
                        Rows = costs.Data
                    };
                    return new JsonResult(response);
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
