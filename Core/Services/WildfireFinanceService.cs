using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Common;
using WCDS.WebFuncions.Core.Model.ContractManagement;
using WCDS.WebFuncions.Core.Model.FinanceDocument;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Services
{
    public interface IWildfireFinanceService
    {
        public Task<List<CustomlistDto>> GetGLAccountForDDL();
        public Task<List<CustomlistDto>> GetCostCenterForDDL();
        public Task<List<CustomlistDto>> GetInternalOrderForDDL();
        public Task<List<CustomlistDto>> GetFundForDDL();
        public Task<FinanceDocumentResponseDto> GetFinanceDocuments(FinanceDocumentRequestDto request);
        public Task<CWSContractResponseDto> GetCWSContracts();


    }
    public class WildfireFinanceService : IWildfireFinanceService
    {
        private readonly ILogger _log;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string _urlKey = "WildfireFinanceServiceApiUrl";
        private readonly string _serviceApiKey = "WildfireFinanceServiceApiKey";

        public WildfireFinanceService(ILogger<WildfireFinanceService> log, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _httpClient = httpClient;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CustomlistDto>> GetCostCenterForDDL()
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
            msg.Headers.TryAddWithoutValidation("Authorization", GetToken());

            await LoggerHelper.LogRequestAsync(_log, msg);
            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            await LoggerHelper.LogResponseAsync(_log, response);
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

        public async Task<CWSContractResponseDto> GetCWSContracts()
        {
            var url = Environment.GetEnvironmentVariable(_urlKey);
            if (url == null)
            {
                _log.LogError(_urlKey + " not found!");
                throw new Exception(_urlKey + " not found");
            }
            url = url + "/GetContracts";
            _log.LogInformation("GetContracts url: {url}", url);

            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable(_serviceApiKey));
            msg.Headers.TryAddWithoutValidation("Authorization", GetToken());

            await LoggerHelper.LogRequestAsync(_log, msg);
            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            await LoggerHelper.LogResponseAsync(_log, response);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            CWSContractResponseDto responseData = JsonConvert.DeserializeObject<CWSContractResponseDto>(json, settings);
            return responseData;
        }

        public async Task<FinanceDocumentResponseDto> GetFinanceDocuments(FinanceDocumentRequestDto request)
        {
            var url = Environment.GetEnvironmentVariable(_urlKey);
            if (url == null)
            {
                _log.LogError(_urlKey + " not found!");
                throw new Exception(_urlKey + " not found");
            }

            url = url + "/GetFinanceDocument?InvoiceNumber=" + request.InvoiceNumber + "&InvoiceAmount="
                + request.InvoiceAmount + "&VendorBusinessId=" + request.VendorBusinessId;
            _log.LogInformation("GetFinanceDocument url: {url}", url);

            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable(_serviceApiKey));
            msg.Headers.TryAddWithoutValidation("Authorization", GetToken());

            await LoggerHelper.LogRequestAsync(_log, msg);
            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            await LoggerHelper.LogResponseAsync(_log, response);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            FinanceDocumentResponseDto responseData = JsonConvert.DeserializeObject<FinanceDocumentResponseDto>(json, settings);
            return responseData;
        }

        public async Task<List<CustomlistDto>> GetFundForDDL()
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
            msg.Headers.TryAddWithoutValidation("Authorization", GetToken());

            await LoggerHelper.LogRequestAsync(_log, msg);
            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            await LoggerHelper.LogResponseAsync(_log, response);
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

        public async Task<List<CustomlistDto>> GetGLAccountForDDL()
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
            msg.Headers.TryAddWithoutValidation("Authorization", GetToken());

            await LoggerHelper.LogRequestAsync(_log, msg);
            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            await LoggerHelper.LogResponseAsync(_log, response);
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

        public async Task<List<CustomlistDto>> GetInternalOrderForDDL()
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
            msg.Headers.TryAddWithoutValidation("Authorization", GetToken());

            await LoggerHelper.LogRequestAsync(_log, msg);
            var response = _httpClient.SendAsync(msg).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            await LoggerHelper.LogResponseAsync(_log, response);

            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<CustomlistDto> responseData = JsonConvert.DeserializeObject<List<CustomlistDto>>(json, settings);

            return responseData;
        }

        private string GetToken()
        {
            var token = this.httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                _log.LogError("No Authorization header found");
                throw new UnauthorizedAccessException();
            }
            return (string)token;
        }
    }
}
