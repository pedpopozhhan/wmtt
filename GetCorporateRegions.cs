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
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model.Services;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Core.Model;
using System.Linq;

namespace WCDS.WebFuncions
{
    public class GetCorporateRegions
    {
        private readonly IDomainService _domainService;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetCorporateRegions(IDomainService domainService, IAuditLogService auditLogService)
        {
            _domainService = domainService;
            _auditLogService = auditLogService;
        }

        [FunctionName("GetCorporateRegions")]
        public async Task<ActionResult<CorporateRegionResponse>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _auditLogService.Audit("GetCorporateRegions");
            try
            {
                log.LogInformation("Trigger function (GetCustomLists) received a request.");

                log.LogInformation("Reading corporate region from DomainService");
                var corporateRegions = await _domainService.GetCorporateRegion();
                log.LogInformation("corporate regions returned from DomainService are: " + corporateRegions.Data.Count);

                var response = new CorporateRegionResponse
                {
                    CorporateRegions = corporateRegions.Data.Select
                    (p => new CorporateRegionDto() { CorporateRegionId = p.CorporateRegionId, RegionName = p.Name }).ToArray()
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
