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
        private readonly IMapper mapper;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetContracts(ITimeReportingService timeReportingService, IMapper mapper, IAuditLogService auditLogService)
        {
            _timeReportingService = timeReportingService;
            this.mapper = mapper;
            this._auditLogService = auditLogService;
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

                // get all the time reports for each contract
                //The number of transfers pending are all invoices of a contract for a vendor that does not contain  a ‘transfer date’.

                var invoiceController = new InvoiceController(log, mapper);
                foreach (var contract in contracts.Data)
                {
                    var invoices = invoiceController.GetInvoices(new InvoiceRequestDto { ContractNumber = contract.ContractNumber });
                    contract.DownloadsAvailable = invoices.Invoices.Count(invoice => !invoice.DocumentDate.HasValue);
                }

                var response = new ContractsResponse
                {
                    Rows = contracts.Data
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
