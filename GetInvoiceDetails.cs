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


namespace WCDS.WebFuncions
{
    public class GetInvoiceDetails
    {
        private readonly IDomainService DomainService;
        private readonly ITimeReportingService TimeReportingService;

        public GetInvoiceDetails(IDomainService domainService, ITimeReportingService timeReportingService)
        {
            DomainService = domainService;
            TimeReportingService = timeReportingService;
        }
        [FunctionName("GetInvoiceDetails")]
        public async Task<ActionResult<InvoiceDetailRowDataRequestDto[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Trigger function (GetInvoiceDetails) received a request.");

            log.LogInformation("Reading rateunits from DomainService");
            var rateUnits = await DomainService.GetRateUnits();
            log.LogInformation("Reading ratetypes from DomainService");
            var rateTypes = await DomainService.GetRateTypes();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<InvoiceDetailRowDataRequestDto>(requestBody);

            if (data != null)
            {
                // call time reports api to get cost details           

            }
            log.LogDebug("Request payload", data);
            Console.WriteLine(JsonConvert.SerializeObject(data));
            return new JsonResult(GetSampleResults());
        }
        private List<InvoiceDetailRowDataDto> GetSampleResults()
        {
            var rows = new List<InvoiceDetailRowDataDto>();
            var date = DateTime.Now;
            for (int i = 1; i <= 50; i++)
            {
                rows.Add(new InvoiceDetailRowDataDto
                {
                    Date = date.AddDays(i),
                    RegistrationNumber = i.ToString(),
                    ReportNumber = i,
                    AO02Number = i.ToString(),
                    RateType = i.ToString(),
                    NumberOfUnits = i,
                    RateUnit = i.ToString(),
                    RatePerUnit = i,
                    Cost = i * 1000.25,
                    GlAccountNumber = i,
                    ProfitCentre = i.ToString(),
                    CostCentre = i.ToString(),
                    FireNumber = i.ToString(),
                    InternalOrder = i.ToString(),
                    Fund = i,
                });
            }

            return rows;
        }
    }
}
/*{
  "search": "",
  "sortBy": "",
  "sortOrder": "",
  "filterBy": {
    "columnName": "",
    "columnValue": ""
  },
  "paginationInfo": {
    "perPage": 2000,
    "page": 1
  }
}*/

/*{
    "status": "true",
    "errorCodeId": "0",
    "errorMessage": "",
    "paginationInfo": {
        "perPage": 2000,
        "page": 1,
        "totalPages": 1,
        "total": 33
    },
    "data": [
        {
            "rateUnitId": "f33834b9-5b4a-433a-a25f-4038806d2601",
            "oracleId": 44,
            "type": "CFU/g",
            "createTimestamp": "1998-04-02T16:35:31",
            "createUserId": "FIRE_PROD",
            "updateTimestamp": null,
            "updateUserId": null,
            "effectiveDate": "2023-06-05T14:30:42",
            "terminationDate": "2024-06-05T14:30:42"
        },
    ]
}*/