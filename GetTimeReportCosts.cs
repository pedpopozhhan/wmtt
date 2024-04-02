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
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    /// <summary>
    /// This service pulls cost details from aviation for a flight report[s] based on the flight report id[s] passed.
    /// </summary>
    public class GetTimeReportCosts
    {
        private readonly ITimeReportingService _timeReportingService;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetTimeReportCosts(ITimeReportingService timeReportingService, IMapper mapper, IAuditLogService auditLogService)
        {
            _timeReportingService = timeReportingService;
            _auditLogService = auditLogService;
        }


        [FunctionName("GetTimeReportCosts")]
        public async Task<ActionResult<TimeReportDetailsResponse[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _auditLogService.Audit("GetTimeReportCosts");
            try
            {
                log.LogInformation("Trigger function (GetTimeReportCosts) received a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<TimeReportCostsRequest>(requestBody);

                if (data != null)
                {
                    var costs = await _timeReportingService.GetTimeReportCosts(data.ContractNumber, data.Status);
                    if (!string.IsNullOrEmpty(costs.ErrorMessage))
                    {
                        throw new Exception(costs.ErrorMessage);
                    }

                    var response = new TimeReportCostsResponse
                    {
                        Rows = costs.Data
                    };

                    jsonResult = new JsonResult(response);
                    jsonResult.StatusCode = StatusCodes.Status200OK;
                    return jsonResult;
                }
                else
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }

            }
            catch (Exception ex)
            {
                log.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult = new JsonResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult.StatusCode = StatusCodes.Status500InternalServerError;
                return jsonResult;
            }
        }
    }
}
