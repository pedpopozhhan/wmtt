using System;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Common;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Model.Services;
using WCDS.WebFuncions.Enums;

namespace WCDS.WebFuncions.Core.Services
{
    public interface ITimeReportingService
    {
        public Task<Response<TimeReportCostDetailDto>> GetTimeReportByIds(int[] ids);
        public Task<Response<TimeReportCostDto>> GetTimeReportCosts(string contractNumber, string status);
        public Task<Response<ContractSearchResultDto>> GetContracts();
    }

    public class TimeReportingService : ITimeReportingService
    {
        private readonly ILogger _log;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        ApplicationDBContext _dbContext;
        private readonly string _token;
        public TimeReportingService(ILogger<TimeReportingService> log, HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ApplicationDBContext dbContext)
        {
            _httpClient = httpClient;
            this._httpContextAccessor = httpContextAccessor;
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _dbContext = dbContext;
            _token = GetToken();
        }

        public async Task<Response<TimeReportCostDetailDto>> GetTimeReportByIds(int[] ids)
        {
            var url = Environment.GetEnvironmentVariable("AviationReportingServiceApiUrl");
            if (url == null)
            {
                _log.LogError("AviationReportingServiceApiUrl not found!");
                throw new Exception("AviationReportingServiceApiUrl not found");
            }
            url = url + "/flight-report/get/cost-details";
            _log.LogInformation("GetTimeReportById url: {url}", url);

            var requestObject = new Request<FilterByCostDetails> { FilterBy = new FilterByCostDetails(ids) };
            var jsonRequest = JsonConvert.SerializeObject(requestObject);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", _token);
            requestMessage.Content = requestContent;
            // await LoggerHelper.LogRequestAsync(_log, requestMessage);
            var response = await _httpClient.SendAsync(requestMessage);
            // await LoggerHelper.LogResponseAsync(_log, response);
            response.EnsureSuccessStatusCode();

            // Handle the http response
            var json = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            Response<TimeReportCostDetailDto> responseData = JsonConvert.DeserializeObject<Response<TimeReportCostDetailDto>>(json, settings);
            if (responseData.Data != null && responseData.Data.Count > 0)
            {
                var softDeletedInvoices = _dbContext.Invoice.Where(p => p.InvoiceStatus == InvoiceStatus.DraftDeleted.ToString()).Select(inv => inv.InvoiceId).ToHashSet();
                responseData.Data = responseData.Data.Where(i => !_dbContext.InvoiceTimeReportCostDetails.Any(r => r.FlightReportCostDetailsId == i.FlightReportCostDetailsId
                                                                    && !softDeletedInvoices.Contains(r.InvoiceId))).ToList();
            }

            return responseData;
        }

        public async Task<Response<TimeReportCostDto>> GetTimeReportCosts(string contractNumber, string status)
        {
            var url = Environment.GetEnvironmentVariable("AviationReportingServiceApiUrl");
            if (url == null)
            {
                _log.LogError("AviationReportingServiceApiUrl not found!");
                throw new Exception("AviationReportingServiceApiUrl not found");
            }
            url += "/flight-report-dashboard/cost";
            _log.LogInformation("GetTimeReportCosts url: {url}", url);

            var requestObject = new Request<FilterByCostRequest> { FilterBy = new FilterByCostRequest(contractNumber, status) };
            var jsonRequest = JsonConvert.SerializeObject(requestObject);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", _token);
            requestMessage.Content = requestContent;
            await LoggerHelper.LogRequestAsync(_log, requestMessage);
            var response = await _httpClient.SendAsync(requestMessage);
            await LoggerHelper.LogResponseAsync(_log, response);
            response.EnsureSuccessStatusCode();

            // Handle the http response
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            Response<TimeReportCostDto> responseData = JsonConvert.DeserializeObject<Response<TimeReportCostDto>>(json, settings);

            return responseData;
        }

        ///flight-report-dashboard/vendors/get

        public async Task<Response<ContractSearchResultDto>> GetContracts()
        {
            var url = Environment.GetEnvironmentVariable("AviationReportingServiceApiUrl");
            if (url == null)
            {
                _log.LogError("AviationReportingServiceApiUrl not found!");
                throw new Exception("AviationReportingServiceApiUrl not found");
            }
            url += "/flight-report-dashboard/vendors/get";
            _log.LogInformation("GetContracts url: {url}", url);

            var request = new Request<FilterBy>
            {
                FilterBy = new FilterBy()
            };

            var jsonRequest = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", _token);
            requestMessage.Content = requestContent;
            await LoggerHelper.LogRequestAsync(_log, requestMessage);
            var response = await _httpClient.SendAsync(requestMessage);
            await LoggerHelper.LogResponseAsync(_log, response);
            response.EnsureSuccessStatusCode();

            // Handle the http response
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            Response<ContractSearchResultDto> responseData = JsonConvert.DeserializeObject<Response<ContractSearchResultDto>>(json, settings);

            return responseData;
        }

        private string GetToken()
        {
            var token = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIyaUpOSFlENVFxdlNkVU9rZ0VHYnEwakU2RlM4X2VBOWxldW11VlYyRDA4In0.eyJleHAiOjE3MTI5MTA4OTcsImlhdCI6MTcxMjg5NjQ5OCwiYXV0aF90aW1lIjoxNzEyODk2NDk3LCJqdGkiOiJkY2UxNjUzMS03ZTFiLTQ5YTItOTZmZS1iNWYzMDhkYjMzN2UiLCJpc3MiOiJodHRwczovL2FjY2Vzcy5hZHNwLWRldi5nb3YuYWIuY2EvYXV0aC9yZWFsbXMvZThhNTRlOGItY2E0YS00MDRhLTliYTEtMDBiNTkzY2QzZmY4IiwiYXVkIjpbIkZsaWdodFJlcG9ydHMiLCJmaW5hbmNlIiwiYWNjb3VudCJdLCJzdWIiOiI0NmY1NjcxYy04NWYwLTQzYmUtOTU2OC04NDVhYmRlZjdhYjIiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJmaW5hbmNlLWFwcCIsInNlc3Npb25fc3RhdGUiOiI2MjUxNGZlYS0yZTQzLTQ4Y2MtYjliOC00MWQxMDQ1NDg5OTMiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vY29udHJhY3RzLndpbGRmaXJlYXBwcy1kZXYuYWxiZXJ0YS5jYSIsImh0dHA6Ly9sb2NhbGhvc3Q6MzAwMCJdLCJyZXNvdXJjZV9hY2Nlc3MiOnsiRmxpZ2h0UmVwb3J0cyI6eyJyb2xlcyI6WyJwX0F2aWF0X0ZsaWdodFJwdEFwcFJldF9FIiwicF9BdmlhdF9GbGlnaHRScHRfUiIsInBfQXZpYXRfRmxpZ2h0UnB0X0QiLCJwX0F2aWF0X0ZsaWdodFJwdF9XIl19LCJmaW5hbmNlIjp7InJvbGVzIjpbIkZpbl9JbnZvaWNlX1YiLCJGaW5fSW52b2ljZV9XIl19LCJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im9wZW5pZCBwcm9maWxlIGVtYWlsIHdlYXRoZXItaW5mby1zY29wZSIsInNpZCI6IjYyNTE0ZmVhLTJlNDMtNDhjYy1iOWI4LTQxZDEwNDU0ODk5MyIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiZ2VuZGVyIjoidW5zcGVjaWZpZWQiLCJkaXNwbGF5IE5hbWUiOiJJZnRpa2hhclFhbWFyIiwibmFtZSI6IklmdGlraGFyIFFhbWFyIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiaWZ0aWtoYXIucWFtYXJAZ292LmFiLmNhIiwiZ2l2ZW5fbmFtZSI6IklmdGlraGFyIiwibWlkZGxlX25hbWUiOiIiLCJmYW1pbHlfbmFtZSI6IlFhbWFyIiwiZW1haWwiOiJpZnRpa2hhci5xYW1hckBnb3YuYWIuY2EifQ.ZO9HG1gAdNwtPChEgpPMrj8kjWWDca8WVGO6moaqAQxOfKgR4eBBGUOf7jZ0YSIB_QeGoF22nRyhJYaUsDYi2BRAi0-Uu6UQsJJRedi0-5d9LPk8tF1v7Y7iHUuRqN9f-uxTvqQ_0Snles9LtKM1L2XathVS_Zy9aqNH0fbTPWid-FlJzXfG1wk9Z0IdRN8sRkZRDW3DqU8OsYZ0_IOuHsjGbvHFxSPNhr2ribUzl8LNlKEBH9vck084oFyZcR5JJHgiPKbPCuzB-6HiMXGCXJIoINAa4cE68C6glcsO6Ca39OscFazk1MiQLvw-jeJlP1MPMTQ7itWr820h4Gu9EQ";
            //var token = this._httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                _log.LogError("No Authorization header found");
                throw new UnauthorizedAccessException();
            }
            return (string)token;
        }
    }
}


