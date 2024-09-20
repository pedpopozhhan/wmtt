using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Services;
using System.Collections.Generic;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model.Services;
using WCDS.WebFuncions.Core.Model.ContractManagement;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Context;
using AutoMapper;
using System.Linq;

namespace WCDS.WebFuncions
{
    public class GetOneGxContract
    {
        private readonly IWildfireFinanceService _wildfireFinanceService;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;
        private readonly ApplicationDBContext _dbContext;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetOneGxContract(IMapper mapper, IWildfireFinanceService wildfireFinanceService, IAuditLogService auditLogService, ApplicationDBContext dbContext)
        {
            _mapper = mapper;
            _wildfireFinanceService = wildfireFinanceService;
            _auditLogService = auditLogService;
            _dbContext = dbContext;
        }

        [FunctionName("GetOneGxContract")]
        public async Task<ActionResult<CWSContractDetailResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _auditLogService.Audit("GetOneGxContract");
            try
            {
                log.LogInformation("Trigger function GetOneGxContract received a request.");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<CWSContractDetailRequestDto>(requestBody);
                if (data == null || data.ContractID <= 0)
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }

                var response = await _wildfireFinanceService.GetCWSContract(data);

                var contractDetail = _dbContext.OneGxContractDetail.Where(p => p.ContractNumber.Trim().ToUpper() == response.ContractNumber.Trim().ToUpper() 
                                                                            && p.ContractWorkspace.Trim().ToUpper() == response.ContractWorkspaceRef.Trim().ToUpper()).FirstOrDefault();
                response.OneGxContractDetail = _mapper.Map<OneGxContractDetailDto>(contractDetail);

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
