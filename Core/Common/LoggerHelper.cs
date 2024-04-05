using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WCDS.WebFuncions.Core.Common
{
    public static class LoggerHelper
    {
        public static async Task LogRequestAsync(ILogger logger, HttpRequestMessage requestMessage)
        {
            // Construct log message
            var logMessage = new StringBuilder();
            logMessage.AppendLine($"Request URI: {requestMessage.RequestUri}");
            logMessage.AppendLine("Headers:");
            foreach (var header in requestMessage.Headers)
            {
                logMessage.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }
            // Include authorization header if present
            if (requestMessage.Headers.Authorization != null)
            {
                logMessage.AppendLine($"  Authorization: {requestMessage.Headers.Authorization}");
            }
            string requestBody = await requestMessage.Content.ReadAsStringAsync();

            logMessage.AppendLine("Request Body:");
            logMessage.AppendLine(requestBody);

            logger.LogDebug(logMessage.ToString());
        }

        public static async Task LogResponseAsync(ILogger logger, HttpResponseMessage responseMessage)
        {
            var logMessage = new StringBuilder();
            logMessage.AppendLine("Response Body:");
            var json = await responseMessage.Content.ReadAsStringAsync();
            logMessage.AppendLine(json);
            logger.LogDebug(logMessage.ToString());
        }
    }
}