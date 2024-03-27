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
    public class GetTimeReportDetails
    {
        private readonly IDomainService _domainService;
        private readonly ITimeReportingService _timeReportingService;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        BadRequestObjectResult badRequestResult = null;

        public GetTimeReportDetails(IDomainService domainService, ITimeReportingService timeReportingService, IMapper mapper, IAuditLogService auditLogService)
        {
            _domainService = domainService;
            _timeReportingService = timeReportingService;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }


        [FunctionName("GetTimeReportDetails")]
        public async Task<ActionResult<TimeReportDetailsResponse[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _auditLogService.Audit("GetTimeReportDetails");
            try
            {
                log.LogInformation("Trigger function (GetTimeReportDetails) received a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<TimeReportDetailsRequest>(requestBody);

                if (data == null)
                {
                    badRequestResult = new BadRequestObjectResult("Invalid Request");
                    badRequestResult.ContentTypes.Add("application/json");
                    return badRequestResult;
                }

                var details = await _timeReportingService.GetTimeReportByIds(data.TimeReportIds);
                if (!string.IsNullOrEmpty(details.ErrorMessage))
                {
                    throw new Exception(details.ErrorMessage);
                }
                var mapped = details.Data?.Select(detail =>
                {
                    var mapped = _mapper.Map<TimeReportCostDetailDto, TimeReportCostDetail>(detail);

                    return mapped;
                });
                var response = new TimeReportDetailsResponse
                {
                    Rows = mapped?.ToArray(),
                };
                
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                var result = new ObjectResult(ex.Message);
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.ContentTypes.Add("application/json");
                return result;
            }
        }
    }
}
