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
        private readonly string _urlKey = "WildfireFinanceServiceApiUrl";
        private readonly string _serviceApiKey = "WildfireFinanceServiceApiKey";

        public WildfireFinanceService(ILogger log, HttpClient httpClient)
        {
            _log = log ;
            _httpClient = httpClient;
        }

        public async Task<List<CustomlistDto>> GetCostCenterForDDL(ILogger _log)
        {
            var url = Environment.GetEnvironmentVariable(_urlKey);
            if (url == null)
            {
                _log.LogError(_urlKey + " not found!");
                throw new Exception(_urlKey + " not found");
            }
            
            url = url + "/getcostcenter?listType=OtherCostDDL";
            _log.LogInformation("getcostcenter url: {url}", url);
            
            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable(_serviceApiKey)); 

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
            var url = Environment.GetEnvironmentVariable(_urlKey);
            if (url == null)
            {
                _log.LogError(_urlKey + " not found!");
                throw new Exception(_urlKey + " not found");
            }

            url = url + "/GetFundMaster?listType=OtherCostDDL";
            _log.LogInformation("GetFundMaster url: {url}", url);

            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable(_serviceApiKey));

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
            var url = Environment.GetEnvironmentVariable(_urlKey);
            if (url == null)
            {
                _log.LogError(_urlKey + " not found!");
                throw new Exception(_urlKey + " not found");
            }

            url = url + "/GetGLAccount?listType=OtherCostDDL";
            _log.LogInformation("GetGLAccount url: {url}", url);

            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable(_serviceApiKey));

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
            var url = Environment.GetEnvironmentVariable(_urlKey);
            if (url == null)
            {
                _log.LogError(_urlKey + " not found!");
                throw new Exception(_urlKey + " not found");
            }

            url = url + "/GetInternalCode?listType=OtherCostDDL";
            _log.LogInformation("GetInternalCode url: {url}", url);

            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable(_serviceApiKey));

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
