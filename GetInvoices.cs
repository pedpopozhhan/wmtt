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
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Enums;
namespace WCDS.WebFuncions
{
    public class GetInvoices
    {

        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ApplicationDBContext _dbContext;
        string errorMessage = "Error : {0}, InnerException: {1}";
        JsonResult jsonResult = null;

        public GetInvoices(IMapper mapper, IAuditLogService auditLogService, ApplicationDBContext dbContext)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            _dbContext = dbContext;
        }

        [FunctionName("GetInvoices")]
        public async Task<ActionResult<InvoiceResponseDto>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                await _auditLogService.Audit("GetInvoices");
                log.LogInformation("Trigger function (GetInvoices) received a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<GetInvoiceRequestDto>(requestBody);
                if (data == null)
                {
                    jsonResult = new JsonResult("Invalid Request");
                    jsonResult.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonResult;
                }

                var responseDto = new InvoiceController(log, _mapper, _dbContext).GetInvoices(data);
                responseDto.Invoices = responseDto.Invoices.Where(x => x.InvoiceStatus == InvoiceStatus.Processed.ToString()).ToArray();

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
