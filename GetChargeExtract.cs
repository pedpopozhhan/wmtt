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
using WCDS.WebFuncions.Core.Validator;
using WCDS.WebFuncions.Core.Common;
using System.Linq;
using WCDS.WebFuncions.Core.Model.ChargeExtract;
using WCDS.WebFuncions.Core.Context;

namespace WCDS.WebFuncions
{
    public class GetChargeExtract
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDBContext _dbContext;
        private readonly IWildfireFinanceService _wildfireFinanceService;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;


        public GetChargeExtract(IMapper mapper, IAuditLogService auditLogService, IWildfireFinanceService wildfireFinanceService, IHttpContextAccessor httpContextAccessor, ApplicationDBContext dbContext)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _wildfireFinanceService = wildfireFinanceService;
        }

        [FunctionName("GetChargeExtract")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post), Route = null)] HttpRequest req, ILogger _logger)
        {
            _logger.LogInformation("Trigger function (GetChargeExtract) received a request");
            try
            {
                await _auditLogService.Audit("GetChargeExtract");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var requestObj = JsonConvert.DeserializeObject<ChargeExtractRequestDto>(requestBody);
                if (requestObj != null)
                {
                    bool tokenParsed = new Common().ParseToken(_httpContextAccessor.HttpContext.Request.Headers, "Authorization", out string parsedTokenResult);
                    if (tokenParsed)
                    {
                        //requestObj.RequestedBy = parsedTokenResult;
                        IChargeExtractController iController = new ChargeExtractController(_logger, _mapper, _wildfireFinanceService, _dbContext);
                        GetChargeExtractValidator validationRules = new GetChargeExtractValidator(iController);

                        var validationResult = validationRules.Validate(requestObj);
                        if (!validationResult.IsValid)
                        {
                            jsonResult = new JsonResult(validationResult.Errors.Select(i => i.ErrorMessage).ToList());
                            jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                            return jsonResult;
                        }

                        var result = iController.GetChargeExtract(requestObj.ChargeExtractId);

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
