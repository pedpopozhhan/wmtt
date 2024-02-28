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
using WCDS.WebFuncions.Core.Common;

namespace WCDS.WebFuncions
{
    public class GetCustomLists
    {
        private readonly IDomainService _domainService;
        private readonly IWildfireFinanceService _wildfireFinanceService;
        string errorMessage = "Error : {0}, InnerException: {1}";

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
            try
            {
                log.LogInformation("Trigger function (GetCustomLists) received a request.");

                log.LogInformation("Reading rateunits from DomainService");
                var rateUnits = await _domainService.GetRateUnits();
                log.LogInformation("rateunits returned from DomainService are: " + rateUnits.Data.Count());

                log.LogInformation("Reading ratetypes from DomainService");
                var rateTypes = await _domainService.GetRateTypes();
                log.LogInformation("ratetypes returned from DomainService are: " + rateTypes.Data.Count());

                log.LogInformation("Reading costCenter from WildFireFinanceApi");
                var costCenter = await _wildfireFinanceService.GetCostCenterForDDL(log);
                log.LogInformation("costCenters returned from WildFireFinanceApi are: {0} ", costCenter == null ? 0 : costCenter.Count);

                log.LogInformation("Reading glAccount from WildFireFinanceApi");
                var glAccount = await _wildfireFinanceService.GetGLAccountForDDL(log);
                log.LogInformation("glAccounts returned from WildFireFinanceApi are: {0} ", glAccount == null ? 0 : glAccount.Count);

                log.LogInformation("Reading internalOrder from WildFireFinanceApi");
                var internalOrder = await _wildfireFinanceService.GetInternalOrderForDDL(log);
                log.LogInformation("internalOrders returned from WildFireFinanceApi are: {0} ", internalOrder == null ? 0 : internalOrder.Count);

                log.LogInformation("Reading Fund from WildFireFinanceApi");
                var fund = await _wildfireFinanceService.GetFundForDDL(log);
                log.LogInformation("Funds returned from WildFireFinanceApi are: {0} ", fund == null ? 0 : fund.Count);

                log.LogInformation("C# HTTP trigger function processed a request.");

                var response = new CustomlistsResponse
                {
                    RateTypes = rateTypes.Data.Where(p => Common.filteredRateTypes.Contains(p.Type)).Select(x => x.Type).ToArray(),
                    RateUnits = rateUnits.Data.Where(p => Common.filteredRateUnits.Contains(p.Type)).Select(x => x.Type).ToArray(),
                    CostCenterList = costCenter.Select(x => x.Value).ToArray(),
                    GLAccountList = glAccount.Select(x => x.Value).ToArray(),
                    InternalOrderList = internalOrder.Select(x => x.Value).ToArray(),
                    FundList = fund.Select(x => x.Value).ToArray()
                };

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                var result = new ObjectResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
           
        }
    }
}
