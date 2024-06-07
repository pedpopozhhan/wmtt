using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Common;
using WCDS.WebFuncions.Core.Model.Services;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    public class GetCustomLists
    {
        private readonly IDomainService _domainService;
        private readonly IWildfireFinanceService _wildfireFinanceService;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetCustomLists(IDomainService domainService, IWildfireFinanceService wildfireFinanceService, IAuditLogService auditLogService)
        {
            _domainService = domainService;
            _wildfireFinanceService = wildfireFinanceService;
            _auditLogService = auditLogService;
        }

        [FunctionName("GetCustomLists")]
        public async Task<ActionResult<CustomlistsResponse>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _auditLogService.Audit("GetCustomLists");
            try
            {
                log.LogInformation("Trigger function (GetCustomLists) received a request.");

                log.LogInformation("Reading rateunits from DomainService");
                var rateUnits = await _domainService.GetRateUnits();
                log.LogInformation("rateunits returned from DomainService are: " + rateUnits.Data.Count());

                log.LogInformation("Reading ratetypes from DomainService");
                var rateTypes = await _domainService.GetRateTypes();
                log.LogInformation("ratetypes returned from DomainService are: " + rateTypes.Data.Count());

                log.LogInformation("Reading ratetypes for aviation reporting expenses from DomainService");
                var aviationReportingExpensesRateTypes = await _domainService.GetRateTypesByService("aviation reporting expenses");
                log.LogInformation("ratetypes for aviation reporting expenses returned from DomainService are: " + aviationReportingExpensesRateTypes.Data.Count());

                log.LogInformation("Reading ratetypes for aviation reporting flying hours from DomainService");
                var aviationreportingflyinghoursRateType = await _domainService.GetRateTypesByService("aviation reporting flying hours");
                log.LogInformation("ratetypes for aviation reporting flying hours returned from DomainService are: " + aviationreportingflyinghoursRateType.Data.Count());

                List<RateType> payableRateTypes = aviationReportingExpensesRateTypes.Data.Union(aviationreportingflyinghoursRateType.Data, new RateTypeComparer()).ToList();
                
                log.LogInformation("Reading costCenter from WildFireFinanceApi");
                var costCenter = await _wildfireFinanceService.GetCostCenterForDDL();
                log.LogInformation("costCenters returned from WildFireFinanceApi are: {0} ", costCenter == null ? 0 : costCenter.Count);

                log.LogInformation("Reading glAccount from WildFireFinanceApi");
                var glAccount = await _wildfireFinanceService.GetGLAccountForDDL();
                log.LogInformation("glAccounts returned from WildFireFinanceApi are: {0} ", glAccount == null ? 0 : glAccount.Count);

                log.LogInformation("Reading internalOrder from WildFireFinanceApi");
                var internalOrder = await _wildfireFinanceService.GetInternalOrderForDDL();
                log.LogInformation("internalOrders returned from WildFireFinanceApi are: {0} ", internalOrder == null ? 0 : internalOrder.Count);

                log.LogInformation("Reading Fund from WildFireFinanceApi");
                var fund = await _wildfireFinanceService.GetFundForDDL();
                log.LogInformation("Funds returned from WildFireFinanceApi are: {0} ", fund == null ? 0 : fund.Count);

                log.LogInformation("C# HTTP trigger function processed a request.");

                var response = new CustomlistsResponse
                {
                    RateTypes = rateTypes.Data.Where(p => Common.filteredRateTypes.Contains(p.Type)).Select(x => x.Type).ToArray(),
                    RateUnits = rateUnits.Data.Where(p => Common.filteredRateUnits.Contains(p.Type)).Select(x => x.Type).ToArray(),
                    PayableRateTypes = payableRateTypes.Select(x => x.Type).ToArray(),
                    CostCenterList = costCenter.ToArray(),
                    GLAccountList = glAccount.ToArray(),
                    InternalOrderList = internalOrder.ToArray(),
                    FundList = fund.ToArray()
                };

                jsonResult = new JsonResult(response);
                jsonResult.StatusCode = StatusCodes.Status200OK;
                return jsonResult;
            }
            catch (Exception ex)
            {
                log.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult = new JsonResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult.StatusCode = StatusCodes.Status500InternalServerError;
                return jsonResult;
            }
        }
    }
}
