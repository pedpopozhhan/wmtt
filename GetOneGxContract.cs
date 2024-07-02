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
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions
{
    public class GetOneGxContract
    {
        private readonly IWildfireFinanceService _wildfireFinanceService;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetOneGxContract(IWildfireFinanceService wildfireFinanceService, IAuditLogService auditLogService)
        {
            _wildfireFinanceService = wildfireFinanceService;
            _auditLogService = auditLogService;
        }

        [FunctionName("GetOneGxContract")]
        public async Task<ActionResult<CWSContractDetailResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _auditLogService.Audit("GetOneGxContract");
            try
            {
                log.LogInformation("Trigger function GetOneGxContract received a request.");
                log.LogInformation("Reading OneGx contract from WildFireFinanceApi");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<CWSContractDetailRequestDto>(requestBody);
                if (data == null || data.ContractID <= 0)
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }

                var response = await _wildfireFinanceService.GetCWSContract(data);
                log.LogInformation("contract returned from WildFireFinanceApi is: {0} ", response == null ? -1 : response.ContractNumber);

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
