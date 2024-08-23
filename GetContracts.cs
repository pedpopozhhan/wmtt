using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Enums;

namespace WCDS.WebFuncions
{
    /// <summary>
    /// This service pulls contracts from aviation.
    /// </summary>
    public class GetContracts
    {
        private readonly ITimeReportingService _timeReportingService;
        private readonly IMapper mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ApplicationDBContext _dbContext;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetContracts(ITimeReportingService timeReportingService, IMapper mapper, IAuditLogService auditLogService, ApplicationDBContext dbContext)
        {
            _timeReportingService = timeReportingService;
            this.mapper = mapper;
            this._auditLogService = auditLogService;
            _dbContext = dbContext;
        }

        [FunctionName("GetContracts")]
        public async Task<ActionResult<ContractsResponse[]>> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            await _auditLogService.Audit("GetContracts");
            try
            {
                log.LogInformation("Trigger function (GetContracts) received a request.");

                var contracts = await _timeReportingService.GetContracts();
                if (!string.IsNullOrEmpty(contracts.ErrorMessage))
                {
                    jsonResult = new JsonResult(contracts.ErrorMessage)
                    {
                        StatusCode = StatusCodes.Status424FailedDependency
                    };
                    return jsonResult;
                }

                // Pulling information from local cache and aviation service to populate data points related to
                // time reports available for Extract and still pending approval
                var invoiceController = new InvoiceController(log, mapper, _dbContext);
                var signedOffReports = "signed off";
                foreach (var contract in contracts.Data)
                {
                    var invoices = invoiceController.GetInvoices(new GetInvoiceRequestDto { ContractNumber = contract.ContractNumber });
                    contract.DownloadsAvailable = invoices.Invoices.Where(p => p.InvoiceStatus == InvoiceStatus.Processed.ToString()
                                                               && !p.DocumentDate.HasValue
                                                               && !string.IsNullOrEmpty(p.UniqueServiceSheetName)).Count();

                    log.LogInformation("GetContracts - pulling time reports for contract {0} with status {1}", contract.ContractNumber, signedOffReports);
                    var costs = await _timeReportingService.GetTimeReportCosts(contract.ContractNumber, signedOffReports);
                    contract.PendingApprovals = costs != null && costs.Data != null ? costs.Data.Count() : 0;
                }

                var response = new ContractsResponse
                {
                    Rows = contracts.Data.ToArray()
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
