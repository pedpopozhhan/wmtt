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


namespace WCDS.WebFuncions
{
    public static class GetInvoiceDetails
    {
        [FunctionName("GetInvoiceDetails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Trigger function (GetInvoiceDetails) received a request.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<InvoiceDetailRowDataRequestDto>(requestBody);
            log.LogDebug("Request payload", data);
            Console.WriteLine(JsonConvert.SerializeObject(data));
            return new JsonResult(GetSampleResults());
        }
        private static List<InvoiceDetailRowDataDto> GetSampleResults()
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
/*export class SampleData {
  static GetSampleResults() IDetailsTableRowData[] {
    const results: IDetailsTableRowData[] = [];
    const date = Date.now();
    for (let i = 1; i <= 50; i++) {
      results.push({
        date: new Date(date + 86400000 * i),
        registrationNumber: `${i}`,
        reportNumber: i,
        aO02Number: `${i}`,
        rateType: `${i}`,
        numberOfUnits: i,
        rateUnit: `${i}`,
        ratePerUnit: i, //with $0.00
        cost: i * 1000.25, //with $0.00
        glAccountNumber: i,
        profitCentre: `${i}`,
        costCentre: `${i}`,
        fireNumber: `${i}`,
        internalOrder: `${i}`,
        fund: i,
      });
    }

    return results;
  }
}*/