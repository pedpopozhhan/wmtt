using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Common;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;
namespace WCDS.WebFuncions
{
    public class DeleteDraft
    {

        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public DeleteDraft(IMapper mapper, IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
        }

        [FunctionName("DeleteDraft")]
        public async Task<ActionResult<InvoiceResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                await _auditLogService.Audit("DeleteDraft");
                log.LogInformation("Trigger function (DeleteDraft) received a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var invoiceId = JsonConvert.DeserializeObject<Guid>(requestBody);

                if (invoiceId == Guid.Empty)
                {
                    jsonResult = new JsonResult("Invalid Request")
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    return jsonResult;
                }
                bool tokenParsed = new Common().ParseToken(_httpContextAccessor.HttpContext.Request.Headers, "Authorization", out string parsedTokenResult);
                if (tokenParsed)
                {
                    var response = new InvoiceController(log, _mapper).DeleteDraft(invoiceId, parsedTokenResult);
                    jsonResult = new JsonResult(Guid.Empty)
                    {
                        StatusCode = StatusCodes.Status200OK
                    };
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

            }
            catch (Exception ex)
            {
                log.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult = new JsonResult(string.Format(errorMessage, ex.Message, ex.InnerException))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return jsonResult;
            }
        }
    }
}
