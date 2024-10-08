using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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
        public Task<CWSContractsResponseDto> GetCWSContracts();
        public Task<CWSContractDetailResponseDto> GetCWSContract(CWSContractDetailRequestDto req);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<CWSContractDetailResponseDto> GetCWSContract(CWSContractDetailRequestDto req)
        {
            var url = Environment.GetEnvironmentVariable(_urlKey);
            if (url == null)
            {
                _log.LogError(_urlKey + " not found!");
                throw new Exception(_urlKey + " not found");
            }
            url = url + "/GetContract";
            _log.LogInformation("GetContract url: {url}", url);
            
            var jsonRequest = JsonConvert.SerializeObject(req);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var msg = new HttpRequestMessage(HttpMethod.Post, url);
            msg.Headers.Add("x-functions-key", Environment.GetEnvironmentVariable(_serviceApiKey));
            msg.Headers.TryAddWithoutValidation("Authorization", GetToken());
            msg.Content = requestContent;

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

            CWSContractDetailResponseDto responseData = JsonConvert.DeserializeObject<CWSContractDetailResponseDto>(json, settings);
            return responseData;
        }

        public async Task<CWSContractsResponseDto> GetCWSContracts()
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

            CWSContractsResponseDto responseData = JsonConvert.DeserializeObject<CWSContractsResponseDto>(json, settings);
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
            var token = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIyaUpOSFlENVFxdlNkVU9rZ0VHYnEwakU2RlM4X2VBOWxldW11VlYyRDA4In0.eyJleHAiOjE3MTI5MTA4OTcsImlhdCI6MTcxMjg5NjQ5OCwiYXV0aF90aW1lIjoxNzEyODk2NDk3LCJqdGkiOiJkY2UxNjUzMS03ZTFiLTQ5YTItOTZmZS1iNWYzMDhkYjMzN2UiLCJpc3MiOiJodHRwczovL2FjY2Vzcy5hZHNwLWRldi5nb3YuYWIuY2EvYXV0aC9yZWFsbXMvZThhNTRlOGItY2E0YS00MDRhLTliYTEtMDBiNTkzY2QzZmY4IiwiYXVkIjpbIkZsaWdodFJlcG9ydHMiLCJmaW5hbmNlIiwiYWNjb3VudCJdLCJzdWIiOiI0NmY1NjcxYy04NWYwLTQzYmUtOTU2OC04NDVhYmRlZjdhYjIiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJmaW5hbmNlLWFwcCIsInNlc3Npb25fc3RhdGUiOiI2MjUxNGZlYS0yZTQzLTQ4Y2MtYjliOC00MWQxMDQ1NDg5OTMiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vY29udHJhY3RzLndpbGRmaXJlYXBwcy1kZXYuYWxiZXJ0YS5jYSIsImh0dHA6Ly9sb2NhbGhvc3Q6MzAwMCJdLCJyZXNvdXJjZV9hY2Nlc3MiOnsiRmxpZ2h0UmVwb3J0cyI6eyJyb2xlcyI6WyJwX0F2aWF0X0ZsaWdodFJwdEFwcFJldF9FIiwicF9BdmlhdF9GbGlnaHRScHRfUiIsInBfQXZpYXRfRmxpZ2h0UnB0X0QiLCJwX0F2aWF0X0ZsaWdodFJwdF9XIl19LCJmaW5hbmNlIjp7InJvbGVzIjpbIkZpbl9JbnZvaWNlX1YiLCJGaW5fSW52b2ljZV9XIl19LCJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im9wZW5pZCBwcm9maWxlIGVtYWlsIHdlYXRoZXItaW5mby1zY29wZSIsInNpZCI6IjYyNTE0ZmVhLTJlNDMtNDhjYy1iOWI4LTQxZDEwNDU0ODk5MyIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiZ2VuZGVyIjoidW5zcGVjaWZpZWQiLCJkaXNwbGF5IE5hbWUiOiJJZnRpa2hhclFhbWFyIiwibmFtZSI6IklmdGlraGFyIFFhbWFyIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiaWZ0aWtoYXIucWFtYXJAZ292LmFiLmNhIiwiZ2l2ZW5fbmFtZSI6IklmdGlraGFyIiwibWlkZGxlX25hbWUiOiIiLCJmYW1pbHlfbmFtZSI6IlFhbWFyIiwiZW1haWwiOiJpZnRpa2hhci5xYW1hckBnb3YuYWIuY2EifQ.ZO9HG1gAdNwtPChEgpPMrj8kjWWDca8WVGO6moaqAQxOfKgR4eBBGUOf7jZ0YSIB_QeGoF22nRyhJYaUsDYi2BRAi0-Uu6UQsJJRedi0-5d9LPk8tF1v7Y7iHUuRqN9f-uxTvqQ_0Snles9LtKM1L2XathVS_Zy9aqNH0fbTPWid-FlJzXfG1wk9Z0IdRN8sRkZRDW3DqU8OsYZ0_IOuHsjGbvHFxSPNhr2ribUzl8LNlKEBH9vck084oFyZcR5JJHgiPKbPCuzB-6HiMXGCXJIoINAa4cE68C6glcsO6Ca39OscFazk1MiQLvw-jeJlP1MPMTQ7itWr820h4Gu9EQ";
            //var token = this.httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                _log.LogError("No Authorization header found");
                throw new UnauthorizedAccessException();
            }
            return (string)token;
        }
    }
}
