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
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using System.Linq;
using WCDS.WebFuncions.Enums;

namespace WCDS.WebFuncions
{
    public class GetInvoiceList
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ApplicationDBContext _dbContext;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetInvoiceList(IMapper mapper, IAuditLogService auditLogService, ApplicationDBContext dbContext)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            _dbContext = dbContext;
        }
        [FunctionName("GetInvoiceList")]
        public async Task<ActionResult<InvoiceListResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                await _auditLogService.Audit("GetInvoiceList");
                log.LogInformation("Trigger function (GetInvoiceList) received a request.");                

                var responseDto = new InvoiceController(log, _mapper, _dbContext).GetInvoiceList();
                responseDto.InvoiceList = responseDto.InvoiceList.Where(x => x.InvoiceStatus == InvoiceStatus.Processed.ToString()
                                                                            || x.InvoiceStatus == InvoiceStatus.Draft.ToString()).ToArray();

                jsonResult = new JsonResult(responseDto);
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