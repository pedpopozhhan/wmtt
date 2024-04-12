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
        JsonResult jsonResult = null;

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


                log.LogInformation("Reading rateunits from DomainService");
                var rateUnits = await _domainService.GetRateUnits();
                log.LogInformation("Reading ratetypes from DomainService");
                var rateTypes = await _domainService.GetRateTypes();

                if (!string.IsNullOrEmpty(rateTypes.ErrorMessage))
                {
                    jsonResult = new JsonResult(rateTypes.ErrorMessage);
                    jsonResult.StatusCode = StatusCodes.Status424FailedDependency;
                    return jsonResult;
                }
                if (!string.IsNullOrEmpty(rateUnits.ErrorMessage))
                {
                    jsonResult = new JsonResult(rateUnits.ErrorMessage);
                    jsonResult.StatusCode = StatusCodes.Status424FailedDependency;
                    return jsonResult;

                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<TimeReportDetailsRequest>(requestBody);

                if (data == null)
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }

                var details = await _timeReportingService.GetTimeReportByIds(data.TimeReportIds);
                if (!string.IsNullOrEmpty(details.ErrorMessage))
                {
                    jsonResult = new JsonResult(details.ErrorMessage)
                    {
                        StatusCode = StatusCodes.Status424FailedDependency
                    };
                    return jsonResult;
                }
                var mapped = details.Data?.Select(detail =>
                {
                    var mapped = _mapper.Map<TimeReportCostDetailDto, TimeReportCostDetail>(detail);
                    mapped.RateType = rateTypes.Data.SingleOrDefault(x => x.RateTypeId == detail.RateTypeId)?.Type;
                    mapped.RateUnit = rateUnits.Data.SingleOrDefault(x => x.RateUnitId == detail.RateUnitId)?.Type;
                    return mapped;
                });
                var response = new TimeReportDetailsResponse
                {
                    Rows = mapped?.ToArray(),
                };

                jsonResult = new JsonResult(response);
                jsonResult.StatusCode = StatusCodes.Status200OK;
                return jsonResult;
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                jsonResult = new JsonResult(ex.Message);
                jsonResult.StatusCode = StatusCodes.Status500InternalServerError;
                return jsonResult;
            }
        }
    }
}
