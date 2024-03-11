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
            var tokenHeader = this.httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(tokenHeader))
            {
                Log.LogError("No Authorization header found");
                throw new UnauthorizedAccessException();
            }
            var parts = tokenHeader.ToString().Split(" ");
            if (parts.Length != 2)
            {
                throw new Exception("Malformed Authorization Header");
            }
            // pull username out of token
            var token = DecodeJwtToken(parts[1]);
            var name = token.Payload?["name"];
            if (name is string && string.IsNullOrEmpty((string)name))
            {
                throw new Exception("No Name found in token");
            }
            var auditLog = new AuditLog
            {
                Info = info,
                Operation = operation,
                Timestamp = DateTime.UtcNow,
                User = (string)name
            };
            dbContext.AuditLog.Add(auditLog);
            await dbContext.SaveChangesAsync();

        }

        private JwtSecurityToken DecodeJwtToken(string encodedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(encodedToken);
            return token;
        }


    }
}


