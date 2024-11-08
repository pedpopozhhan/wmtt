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
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Core.Validator;
using WCDS.WebFuncions.Enums;
namespace WCDS.WebFuncions
{
    public class SaveDraft
    {

        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDBContext _dbContext;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public SaveDraft(IMapper mapper, IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor, ApplicationDBContext dbContext)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            this._httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        [FunctionName("SaveDraft")]
        public async Task<ActionResult<InvoiceDto>> Run(
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
                    var invoiceController = new InvoiceController(log, _mapper, _dbContext);
                    var validationRules = new InvoiceDraftValidator(invoiceController);

                    var validationResult = validationRules.Validate(data);
                    if (!validationResult.IsValid)
                    {
                        jsonResult = new JsonResult(validationResult.Errors.Select(i => i.ErrorMessage).ToList());
                        jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                        return jsonResult;
                    }
                    InvoiceDto result = null;
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
