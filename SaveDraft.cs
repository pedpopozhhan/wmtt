using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
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
using WCDS.WebFuncions.Core.Common;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Enums;
namespace WCDS.WebFuncions
{
    public class SaveDraft
    {

        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public SaveDraft(IMapper mapper, IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            this._httpContextAccessor = httpContextAccessor;
        }

        [FunctionName("SaveDraft")]
        public async Task<ActionResult<InvoiceResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                await _auditLogService.Audit("SaveDraft");
                log.LogInformation("Trigger function (SaveDraft) received a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<InvoiceRequestDto>(requestBody);

                if (data == null)
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }
                bool tokenParsed = new Common().ParseToken(_httpContextAccessor.HttpContext.Request.Headers, "Authorization", out string parsedTokenResult);
                if (tokenParsed)
                {
                    var invoiceController = new InvoiceController(log, _mapper);
                    Guid result = Guid.Empty;
                    if (!data.InvoiceId.HasValue || data.InvoiceId == Guid.Empty)
                    {
                        result = await invoiceController.CreateDraft(data, parsedTokenResult);
                    }
                    else
                    {
                        result = await invoiceController.UpdateDraft(data, parsedTokenResult);
                    }

                    jsonResult = new JsonResult(result);
                    jsonResult.StatusCode = StatusCodes.Status200OK;
                    return jsonResult;
                }
                else
                {
                    jsonResult = new JsonResult(parsedTokenResult)
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return jsonResult;
                }

                //if invoice has id, set to draft, update date and userid
                //else create
                // data.InvoiceStatus = InvoiceStatus.Draft.ToString();
                // if (!data.InvoiceId.HasValue || data.InvoiceId == Guid.Empty)
                // {

                // }
                // else
                // {

                // }
                // var response = new InvoiceController(log, _mapper).GetInvoicesWithDetails(data);
                // var drafts = response.Invoices.Where(x => x.InvoiceStatus == InvoiceStatus.Draft.ToString());
                // for now, just return all the invoices               



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
