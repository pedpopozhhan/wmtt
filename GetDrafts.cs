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
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Enums;
namespace WCDS.WebFuncions
{
    public class GetDrafts
    {

        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetDrafts(IMapper mapper, IAuditLogService auditLogService)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        [FunctionName("GetDrafts")]
        public async Task<ActionResult<InvoiceDraftResponse>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                await _auditLogService.Audit("GetInvoices");
                log.LogInformation("Trigger function (GetInvoices) received a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<InvoiceRequestDto>(requestBody);

                if (data == null)
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }

                var response = new InvoiceController(log, _mapper).GetInvoices(data);
                // var drafts = response.Invoices.Where(x => x.InvoiceStatus == InvoiceStatus.Draft.ToString()).Select(x =>
                // {
                //     return new InvoiceDraft
                //     {
                //         InvoiceAmount = x.InvoiceAmount,
                //         InvoiceDate = x.InvoiceDate,
                //         InvoiceId = x.InvoiceId,
                //         InvoiceNumber = x.InvoiceNumber
                //     };
                // });
                // for now, just return all the invoices
                var drafts = response.Invoices.Select(x =>
                {
                    return new InvoiceDraft
                    {
                        InvoiceAmount = x.InvoiceAmount,
                        InvoiceDate = x.InvoiceDate,
                        InvoiceId = x.InvoiceId,
                        InvoiceNumber = x.InvoiceNumber
                    };
                });
                var draftsResponse = new InvoiceDraftResponse { InvoiceDrafts = drafts.ToArray() };

                jsonResult = new JsonResult(draftsResponse);
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
