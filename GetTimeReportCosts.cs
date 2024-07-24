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
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    /// <summary>
    /// This service pulls cost details from aviation for a flight report[s] based on the flight report id[s] passed.
    /// </summary>
    public class GetTimeReportCosts
    {
        private readonly ITimeReportingService _timeReportingService;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetTimeReportCosts(ITimeReportingService timeReportingService, IMapper mapper, IAuditLogService auditLogService)
        {
            _timeReportingService = timeReportingService;
            _mapper = mapper;
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
                if (data == null || string.IsNullOrEmpty(data.ContractNumber) || string.IsNullOrEmpty(data.Status))
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }
                if (data != null)
                { // Retrieves for the tabs tab
                    if (data.Status.ToLower() == "approved")
                    {
                        var responseDto = await new TimeReportController(_timeReportingService, log, _mapper).GetApprovedTimeReports(data);
                        var response = new TimeReportCostsResponse
                        {
                            Rows = responseDto.Data.ToArray()
                        };
                        jsonResult = new JsonResult(response);
                        jsonResult.StatusCode = StatusCodes.Status200OK;
                        return jsonResult;
                    }
                    else
                    {
                        var costs = await _timeReportingService.GetTimeReportCosts(data.ContractNumber, data.Status);
                        if (!string.IsNullOrEmpty(costs.ErrorMessage))
                        {
                            jsonResult = new JsonResult(costs.ErrorMessage);
                            jsonResult.StatusCode = StatusCodes.Status424FailedDependency;
                            return jsonResult;
                        }

                        var response = new TimeReportCostsResponse
                        {
                            Rows = costs.Data.ToArray()
                        };

                        jsonResult = new JsonResult(response);
                        jsonResult.StatusCode = StatusCodes.Status200OK;
                        return jsonResult;
                    }
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
