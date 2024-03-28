using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions
{
    /// <summary>
    /// This service pulls contracts from aviation.
    /// </summary>
    public class GetContracts
    {
        private readonly ITimeReportingService _timeReportingService;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";

        public GetContracts(ITimeReportingService timeReportingService, IMapper mapper, IAuditLogService auditLogService)
        {

            _timeReportingService = timeReportingService;
            _mapper = mapper;
            this._auditLogService = auditLogService;
        }


        [FunctionName("GetContracts")]
        public async Task<ActionResult<ContractsResponse[]>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _auditLogService.Audit("GetContracts");
            try
            {
                log.LogInformation("Trigger function (GetContracts) received a request.");

                var contracts = await _timeReportingService.GetContracts();
                if (!string.IsNullOrEmpty(contracts.ErrorMessage))
                {
                    throw new Exception(contracts.ErrorMessage);
                }

                var response = new ContractsResponse
                {
                    Rows = contracts.Data
                };
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                var result = new ObjectResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.ContentTypes.Add("application/json");
                return result;
            }
        }
    }
}
