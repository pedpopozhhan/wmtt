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
using WCDS.ContractUtilization.Repositories;

namespace WCDS.ContractUtilization
{
    public class FlightReportSearch
    {
        public ISearchRepository SearchRepository { get; }
        public FlightReportSearch(ISearchRepository searchRepository)
        {
            SearchRepository = searchRepository;
        }

        [FunctionName("FlightReportSearch")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var searchRequest = JsonConvert.DeserializeObject<SearchRequest>(requestBody);


            var response = new SearchResponse
            {
                PageNumber = searchRequest.PageNumber,
                PageSize = searchRequest.PageSize,
                Ascending = searchRequest.Ascending,
                SortColumn = searchRequest.SortColumn,
                SearchResults = SearchRepository.Query(searchRequest)
            };

            return new OkObjectResult(response);
        }
    }
}
