using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Entity;

namespace WCDS.WebFuncions.Core.Services
{
    public interface IAuditLogService
    {
        public Task Audit(string operation, string info = "");
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly ILogger Log;

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ApplicationDBContext dbContext;
      
        public AuditLogService(ILogger<DomainService> log, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = new ApplicationDBContext();
            Log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task Audit(string operation, string info = "")
        {
            bool tokenParsed = new Common.Common().ParseToken(httpContextAccessor.HttpContext.Request.Headers, "Authorization", out string parsedTokenResult);
            if (tokenParsed)
            {
                var auditLog = new AuditLog
                {
                    Info = info,
                    Operation = operation,
                    Timestamp = DateTime.UtcNow,
                    User = parsedTokenResult
                };                
                dbContext.AuditLog.Add(auditLog);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception(parsedTokenResult);
            }
        }
    }
}


