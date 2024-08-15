using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Services;
using System.Collections.Generic;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model.Services;
using WCDS.WebFuncions.Core.Model.ContractManagement;

namespace WCDS.WebFuncions
{
    public class GetOneGxContracts
    {
        private readonly IWildfireFinanceService _wildfireFinanceService;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetOneGxContracts(IWildfireFinanceService wildfireFinanceService, IAuditLogService auditLogService)
        {   
            _wildfireFinanceService = wildfireFinanceService;
            _auditLogService = auditLogService;
        }

        [FunctionName("GetOneGxContracts")]
        public async Task<ActionResult<CWSContractsResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _auditLogService.Audit("GetOneGxContracts");
            try
            {
                log.LogInformation("Trigger function (GetOneGxContracts) received a request.");
                log.LogInformation("Reading OneGx contracts from WildFireFinanceApi");
                var response = await _wildfireFinanceService.GetCWSContracts();
                log.LogInformation("contracts returned from WildFireFinanceApi are: {0} ", response == null ? 0 : response.Count);

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
