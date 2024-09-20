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
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.ContractManagement;

namespace WCDS.WebFuncions
{
    public class SaveOneGxContractDetail
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly IDomainService _domainService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDBContext _dbContext;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public SaveOneGxContractDetail(IMapper mapper, IAuditLogService auditLogService, IDomainService domainService, IHttpContextAccessor httpContextAccessor, ApplicationDBContext dbContext)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            _domainService = domainService;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        [FunctionName("SaveOneGxContractDetail")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post), Route = null)] HttpRequest req, ILogger _logger)
        {
            _logger.LogInformation("Trigger function (SaveOneGxContractDetail) received a request");
            try
            {
                await _auditLogService.Audit("SaveOneGxContractDetail");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<OneGxContractDetailDto>(requestBody);

                if (data == null)
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }

                bool tokenParsed = new Common().ParseToken(_httpContextAccessor.HttpContext.Request.Headers, "Authorization", out string parsedTokenResult);
                if (tokenParsed)
                {
                    var oneGxContractController = new OneGxContractController(_logger, _mapper, _dbContext);
                var validationRules = new OneGxContractDetailValidator();

                var validationResult = validationRules.Validate(data);
                if (!validationResult.IsValid)
                {
                    jsonResult = new JsonResult(validationResult.Errors.Select(i => i.ErrorMessage).ToList());
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }

                OneGxContractDetailDto result = null;
                if (!data.OneGxContractId.HasValue || data.OneGxContractId == Guid.Empty)
                {
                    result = await oneGxContractController.CreateOneGxContractDetail(data, parsedTokenResult);                    
                }
                else
                {
                    result = await oneGxContractController.UpdateOneGxContractDetail(data, parsedTokenResult);                    
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
                _logger.LogError(string.Format("SaveOneGxContractDetail: ", errorMessage, ex.Message, ex.InnerException));
                jsonResult = new JsonResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                jsonResult.StatusCode = StatusCodes.Status500InternalServerError;
                return jsonResult;
            }
        }
    }
}
