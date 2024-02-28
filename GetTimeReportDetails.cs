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
        private readonly IDomainService DomainService;
        private readonly ITimeReportingService TimeReportingService;
        private readonly IMapper Mapper;
        string errorMessage = "Error : {0}, InnerException: {1}";

        public GetTimeReportDetails(IDomainService domainService, ITimeReportingService timeReportingService, IMapper mapper)
        {
            DomainService = domainService;
            TimeReportingService = timeReportingService;
            Mapper = mapper;
        }


        [FunctionName("GetTimeReportDetails")]
        public async Task<ActionResult<TimeReportDetailsResponse[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("Trigger function (GetTimeReportDetails) received a request.");

                log.LogInformation("Reading rateunits from DomainService");
                var rateUnits = await DomainService.GetRateUnits();
                log.LogInformation("Reading ratetypes from DomainService");
                var rateTypes = await DomainService.GetRateTypes();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<TimeReportDetailsRequest>(requestBody);

                if (data == null)
                {
                    return new BadRequestObjectResult("Invalid Request");
                }

                if (string.IsNullOrEmpty(rateTypes.ErrorMessage) && string.IsNullOrEmpty(rateTypes.ErrorMessage))
                {
                    var details = await TimeReportingService.GetTimeReportByIds(data.TimeReportIds);
                    if (!string.IsNullOrEmpty(details.ErrorMessage))
                    {
                        throw new Exception(details.ErrorMessage);
                    }
                    var mapped = details.Data?.Select(detail =>
                    {
                        var mapped = Mapper.Map<TimeReportCostDetailDto, TimeReportCostDetail>(detail);
                        // set rate unit and rate type
                        mapped.RateType = rateTypes.Data.SingleOrDefault(x => x.RateTypeId == detail.RateTypeId)?.Type;
                        mapped.RateUnit = rateUnits.Data.SingleOrDefault(x => x.RateUnitId == detail.RateUnitId)?.Type;
                        return mapped;
                    });
                    var response = new TimeReportDetailsResponse
                    {
                        Rows = mapped?.ToArray(),
                        RateTypes = rateTypes.Data.Select(x => x.Type).ToArray()
                    };
                    return new JsonResult(response);
                }

                throw new Exception("Error retrieving rateTypes or rateUnits");

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                var result = new ObjectResult(ex.Message);
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }
    }
}
