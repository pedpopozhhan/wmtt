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
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Common;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.CAS;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Core.Validator;

namespace WCDS.WebFuncions
{
    public class InsertContract
    {
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ICASService _casService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public InsertContract(IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor, ICASService casService)
        {
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
            _casService = casService;
        }

        [FunctionName("InsertContract")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Put), Route = null)] HttpRequest req, ILogger _logger)
        {
            _logger.LogInformation("Trigger function (InsertContract) received a request");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var payload = JsonConvert.DeserializeObject<CASContractDto>(requestBody);
                if (payload != null)
                {
                    bool tokenParsed = new Common().ParseToken(_httpContextAccessor.HttpContext.Request.Headers, "Authorization", out string parsedTokenResult);
                    if (tokenParsed)
                    {
                        //TODO: the following service needs some better error handling and logging
                        var result = await _casService.InsertContract(payload);

                        jsonResult = new JsonResult(result)
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
                else
                {
                    jsonResult = new JsonResult("Invalid Request")
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult = new JsonResult(string.Format(errorMessage, ex.Message, ex.InnerException))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return jsonResult;
            }
        }
    }
}
