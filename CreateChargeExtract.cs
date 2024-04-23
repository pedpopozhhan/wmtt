using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AutoMapper;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Validator;
using WCDS.WebFuncions.Core.Common;
using System.Linq;

namespace WCDS.WebFuncions
{
    public class CreateChargeExtract
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public CreateChargeExtract(IMapper mapper, IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
        }

        [FunctionName("CreateChargeExtract")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Put), Route = null)] HttpRequest req, ILogger _logger)
        {
            _logger.LogInformation("Trigger function (CreateChargeExtract) received a request");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var chargeExtractObj = JsonConvert.DeserializeObject<ChargeExtractDto>(requestBody);
                if (chargeExtractObj != null)
                {
                    bool tokenParsed = new Common().ParseToken(_httpContextAccessor.HttpContext.Request.Headers, "Authorization", out string parsedTokenResult);
                    if (tokenParsed)
                    {
                        chargeExtractObj.RequestedBy = parsedTokenResult;
                        IChargeExtractController iController = new ChargeExtractController(_logger, _mapper);
                        ChargeExtractValidator validationRules = new ChargeExtractValidator(iController);

                        var validationResult = validationRules.Validate(chargeExtractObj);
                        if (!validationResult.IsValid)
                        {
                            jsonResult = new JsonResult(validationResult.Errors.Select(i => i.ErrorMessage).ToList());
                            jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                            return jsonResult;
                        }

                        var result = await iController.CreateChargeExtract(chargeExtractObj);
                        try
                        {
                            await _auditLogService.Audit("CreateChargeExtract");
                        }
                        catch (Exception auditException)
                        {
                            _logger.LogError(string.Format(errorMessage, auditException.Message, auditException.InnerException));
                        }

                        jsonResult = new JsonResult(result);
                        jsonResult.StatusCode = StatusCodes.Status200OK;
                        return jsonResult;
                    }
                    else
                    {
                        jsonResult = new JsonResult(parsedTokenResult);
                        jsonResult.StatusCode = StatusCodes.Status401Unauthorized;
                        return jsonResult;
                    }
                }
                else
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult = new JsonResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult.StatusCode = StatusCodes.Status500InternalServerError;
                return jsonResult;
            }
        }


    }
}
