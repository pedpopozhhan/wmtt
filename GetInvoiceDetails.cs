using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;
using System.Linq;
using WCDS.WebFuncions.Core.Model.Services;
using AutoMapper;
using System.Web.Http;


namespace WCDS.WebFuncions
{
    public class GetInvoiceDetails
    {
        private readonly IDomainService DomainService;
        private readonly ITimeReportingService TimeReportingService;
        private readonly IMapper Mapper;

        public GetInvoiceDetails(IDomainService domainService, ITimeReportingService timeReportingService, IMapper mapper)
        {
            DomainService = domainService;
            TimeReportingService = timeReportingService;
            Mapper = mapper;
        }
        [FunctionName("GetInvoiceDetails")]
        public async Task<ActionResult<InvoiceDetailsResponse[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Trigger function (GetInvoiceDetails) received a request.");

            log.LogInformation("Reading rateunits from DomainService");
            var rateUnits = await DomainService.GetRateUnits();
            log.LogInformation("Reading ratetypes from DomainService");
            var rateTypes = await DomainService.GetRateTypes();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(requestBody);

            if (data != null && string.IsNullOrEmpty(rateTypes.ErrorMessage) && string.IsNullOrEmpty(rateTypes.ErrorMessage))
            {

                var details = await TimeReportingService.GetTimeReportByIds(data.TimeReportIds);
                if (!string.IsNullOrEmpty(details.ErrorMessage))
                {
                    log.LogError(details.ErrorMessage);
                    throw new Exception(details.ErrorMessage);
                }
                var mapped = details.Data?.Select(detail =>
                {
                    var mapped = Mapper.Map<CostDetailDto, CostDetail>(detail);
                    // set rate unit and rate type
                    mapped.RateType = rateTypes.Data.SingleOrDefault(x => x.RateTypeId == detail.RateTypeId)?.Type;
                    mapped.RateUnit = rateUnits.Data.SingleOrDefault(x => x.RateUnitId == detail.RateUnitId)?.Type;
                    return mapped;
                });
                var response = new InvoiceDetailsResponse
                {
                    Rows = mapped?.ToArray(),
                    RateTypes = rateTypes.Data.Select(x => x.Type).ToArray()
                };
                return new JsonResult(response);
            }
            log.LogError("Either invalid request, or an error retrieving rateTypes or rateUnits");
            throw new Exception("Either invalid request, or an error retrieving rateTypes or rateUnits");
        }

    }
}