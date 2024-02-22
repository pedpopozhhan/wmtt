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
    /// This service pulls contracts from aviation.
    /// </summary>
    public class GetContracts
    {
        private readonly ITimeReportingService TimeReportingService;
        private readonly IMapper Mapper;

        public GetContracts(ITimeReportingService timeReportingService, IMapper mapper)
        {

            TimeReportingService = timeReportingService;
            Mapper = mapper;
        }


        [FunctionName("GetContracts")]
        public async Task<ActionResult<ContractsResponse[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Trigger function (GetContracts) received a request.");

            var contracts = await TimeReportingService.GetContracts();
            if (!string.IsNullOrEmpty(contracts.ErrorMessage))
            {
                log.LogError(contracts.ErrorMessage);
                throw new Exception(contracts.ErrorMessage);
            }

            var response = new ContractsResponse
            {
                Rows = contracts.Data
            };
            return new JsonResult(response);


        }
    }
}
