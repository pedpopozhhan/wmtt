using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WCDS.WebFuncions.Core.Services;
using System.Linq;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions
{
    public class GetCustomLists
    {
        private readonly IDomainService _domainService;
        private readonly IWildfireFinanceService _wildfireFinanceService;

        public GetCustomLists(IDomainService domainService, IWildfireFinanceService wildfireFinanceService)
        {
            _domainService = domainService;
            _wildfireFinanceService = wildfireFinanceService;
        }

        [FunctionName("GetCustomLists")]
        public async Task<ActionResult<CustomlistsResponse>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Trigger function (GetCustomLists) received a request.");

            log.LogInformation("Reading rateunits from DomainService");
            var rateUnits = await _domainService.GetRateUnits();

            log.LogInformation("Reading ratetypes from DomainService");
            var rateTypes = await _domainService.GetRateTypes();

            log.LogInformation("Reading costCenter from WildFireFinanceApi");
            var costCenter = await _wildfireFinanceService.GetCostCenterForDDL(log);

            log.LogInformation("Reading glAccount from WildFireFinanceApi");
            var glAccount = await _wildfireFinanceService.GetGLAccountForDDL(log);

            log.LogInformation("Reading internalOrder from WildFireFinanceApi");
            var internalOrder = await _wildfireFinanceService.GetInternalOrderForDDL(log);

            log.LogInformation("Reading Fund from WildFireFinanceApi");
            var fund = await _wildfireFinanceService.GetFundForDDL(log);

            log.LogInformation("C# HTTP trigger function processed a request.");

            var response = new CustomlistsResponse
            {
                RateTypes = rateTypes.Data.Select(x => x.Type).ToArray(),
                RateUnits = rateUnits.Data.Select(x => x.Type).ToArray(),
                CostCenterList = costCenter.Select(x => x.Value).ToArray(),
                GLAccountList = glAccount.Select(x => x.Value).ToArray(),
                InternalOrderList = internalOrder.Select(x => x.Value).ToArray(),
                FundList = fund.Select(x => x.Value).ToArray()
            };
            
            return new JsonResult(response);
        }
    }
}
