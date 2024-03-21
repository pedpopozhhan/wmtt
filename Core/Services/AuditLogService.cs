using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Services
{
    public interface IAuditLogService
    {
        public Task<string> Audit(string operation, string info = "");
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

        public async Task<string> Audit(string operation, string info = "")
        {
            var name = "Unknown";
            var tokenHeader = this.httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(tokenHeader))
            {

                var parts = tokenHeader.ToString().Split(" ");
                if (parts.Length != 2)
                {
                    throw new UnauthorizedAccessException("Malformed Authorization Header");
                }
                // pull username out of token
                var token = DecodeJwtToken(parts[1]);
                var part1 = token.Payload?["name"];
                if (part1 is string && string.IsNullOrEmpty((string)part1))
                {
                    throw new Exception("No Name found in token");
                }
                name = (string)part1;
            }
            var auditLog = new AuditLog
            {
                Info = info,
                Operation = operation,
                Timestamp = DateTime.UtcNow,
                User = name
            };
            dbContext.AuditLog.Add(auditLog);
            await dbContext.SaveChangesAsync();
            return name;

        }

        private JwtSecurityToken DecodeJwtToken(string encodedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(encodedToken);
            return token;
        }


    }
}


