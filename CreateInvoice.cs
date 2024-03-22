using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Validator;
using AutoMapper;
using WCDS.WebFuncions.Core.Services;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace WCDS.WebFuncions
{
    public class CreateInvoice
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string errorMessage = "Error : {0}, InnerException: {1}";

        public CreateInvoice(IMapper mapper, IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
        }

        [FunctionName("CreateInvoice")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Put), Route = null)] HttpRequest req, ILogger _logger)
        {
            _logger.LogInformation("Trigger function (CreateInvoice) received a request");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var invoiceObj = JsonConvert.DeserializeObject<InvoiceDto>(requestBody);
                if (invoiceObj != null)
                {
                    var name = "Unknown";
                    var tokenHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                    if (!string.IsNullOrEmpty(tokenHeader))
                    {
                        var parts = tokenHeader.ToString().Split(" ");
                        if (parts.Length != 2)
                        {
                            return new UnauthorizedObjectResult("Malformed Authorization Header");
                        }
                        // pull username out of token
                        var token = DecodeJwtToken(parts[1]);
                        var part1 = token.Payload?["name"];
                        if (part1 is string && string.IsNullOrEmpty((string)part1))
                        {
                            return new UnauthorizedObjectResult("No Name found in token");
                        }
                        name = (string)part1;
                    }
                    else
                    {
                        return new UnauthorizedObjectResult("No Token Header found in the request");
                    }

                    invoiceObj.CreatedBy = name;
                    IInvoiceController iController = new InvoiceController(_logger, _mapper);
                    InvoiceValidator validationRules = new InvoiceValidator(iController);

                    var validationResult = validationRules.Validate(invoiceObj);
                    if (!validationResult.IsValid)
                    {
                        return new BadRequestObjectResult(validationResult.Errors.Select(i => i.ErrorMessage).ToList());
                    }

                    var result = await iController.CreateInvoice(invoiceObj);
                    try
                    {
                        await _auditLogService.Audit("CreateInvoice");
                    }
                    catch(Exception auditException)
                    {
                        _logger.LogError(string.Format(errorMessage, auditException.Message, auditException.InnerException));
                    }
                    return new OkObjectResult(result);
                }
                else
                {
                    return new BadRequestObjectResult("Invalid Request");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(errorMessage, ex.Message, ex.InnerException));
                var result = new ObjectResult(string.Format(errorMessage, ex.Message, ex.InnerException));
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }

        private JwtSecurityToken DecodeJwtToken(string encodedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(encodedToken);
            return token;
        }
    }
}
