using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.ContractUtilization.Models;
using System.Collections.Generic;

namespace WCDS.ContractUtilization
{
    public static class FlightReportSearch
    {
        [FunctionName("FlightReportSearch")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SearchRequest>(requestBody);


            var response = new SearchResponse
            {
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                Ascending = data.Ascending,
                SortColumn = data.SortColumn
            };
            var searchResults = new List<SearchResult>{
                new SearchResult{}
            };
            return new OkObjectResult(response);
        }
    }
}
