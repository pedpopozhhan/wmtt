using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Services
{
    public interface IWildfireFinanceService
    {
        public Task<List<CustomlistDto>> GetGLAccountForDDL(ILogger _log);
        public Task<List<CustomlistDto>> GetCostCenterForDDL(ILogger _log);
        public Task<List<CustomlistDto>> GetInternalOrderForDDL(ILogger _log);
        public Task<List<CustomlistDto>> GetFundForDDL(ILogger _log);
    }
    public class WildfireFinanceService : IWildfireFinanceService
    {
        private readonly ILogger _log;
        private readonly HttpClient _httpClient;

        public WildfireFinanceService(ILogger log, HttpClient httpClient)
        {
            _log = log ;
            _httpClient = httpClient;
        }

        public async Task<List<CustomlistDto>> GetCostCenterForDDL(ILogger _log)
        {
            var url = Environment.GetEnvironmentVariable("WildfireFinanceServiceApiUrl");
            if (url == null)
            {
                _log.LogError("WildfireFinanceServiceApiUrl not found!");
                throw new Exception("WildfireFinanceServiceApiUrl not found");
            }
            
            url = url + "/api/getcostcenter?listType=OtherCostDDL";
            _log.LogInformation("getcostcenter url: {url}", url);
            
            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable("xFunctionsKey")); 

            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<CustomlistDto> responseData = JsonConvert.DeserializeObject<List<CustomlistDto>>(json, settings);

            return responseData;
        }

        public async Task<List<CustomlistDto>> GetFundForDDL(ILogger _log)
        {
            var url = Environment.GetEnvironmentVariable("WildfireFinanceServiceApiUrl");
            if (url == null)
            {
                _log.LogError("WildfireFinanceServiceApiUrl not found!");
                throw new Exception("WildfireFinanceServiceApiUrl not found");
            }

            url = url + "/api/GetFundMaster?listType=OtherCostDDL";
            _log.LogInformation("GetFundMaster url: {url}", url);

            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable("xFunctionsKey"));

            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<CustomlistDto> responseData = JsonConvert.DeserializeObject<List<CustomlistDto>>(json, settings);

            return responseData;
        }

        public async Task<List<CustomlistDto>> GetGLAccountForDDL(ILogger _log)
        {
            var url = Environment.GetEnvironmentVariable("WildfireFinanceServiceApiUrl");
            if (url == null)
            {
                _log.LogError("WildfireFinanceServiceApiUrl not found!");
                throw new Exception("WildfireFinanceServiceApiUrl not found");
            }

            url = url + "/api/GetGLAccount?listType=OtherCostDDL";
            _log.LogInformation("GetGLAccount url: {url}", url);

            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable("xFunctionsKey"));

            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<CustomlistDto> responseData = JsonConvert.DeserializeObject<List<CustomlistDto>>(json, settings);

            return responseData;
        }

        public async Task<List<CustomlistDto>> GetInternalOrderForDDL(ILogger _log)
        {
            var url = Environment.GetEnvironmentVariable("WildfireFinanceServiceApiUrl");
            if (url == null)
            {
                _log.LogError("WildfireFinanceServiceApiUrl not found!");
                throw new Exception("WildfireFinanceServiceApiUrl not found");
            }

            url = url + "/api/GetInternalCode?listType=OtherCostDDL";
            _log.LogInformation("GetInternalCode url: {url}", url);

            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable("xFunctionsKey"));

            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<CustomlistDto> responseData = JsonConvert.DeserializeObject<List<CustomlistDto>>(json, settings);

            return responseData;
        }
    }
}
