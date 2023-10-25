using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.ContractUtilization.Repositories;
using WCDS.ContractUtilization.Models;

namespace WCDS.ContractUtilization
{
    public class FlightReportAll
    {
        public ISearchRepository SearchRepository { get; }
        public FlightReportAll(ISearchRepository searchRepository)
        {
            SearchRepository = searchRepository;
        }

        [FunctionName("FlightReportAll")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var response = new SearchResponse
            {

                SearchResults = SearchRepository.Query()
            };

            return new OkObjectResult(response);
        }
    }
}

