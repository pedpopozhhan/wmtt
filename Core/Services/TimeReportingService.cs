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
        public TimeReportingService(ILogger<TimeReportingService> log, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            this._httpContextAccessor = httpContextAccessor;
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _dbContext = new ApplicationDBContext();
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
            Response<TimeReportCostDetailDto> responseData = JsonConvert.DeserializeObject<Response<TimeReportCostDetailDto>>(json, settings);
            if (responseData.Data != null && responseData.Data.Count > 0)
            {
                responseData.Data = responseData.Data.Where(i => !_dbContext.InvoiceTimeReportCostDetails.Any(r => r.FlightReportCostDetailsId == i.FlightReportCostDetailsId)).ToList();
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
            var token = this._httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                _log.LogError("No Authorization header found");
                throw new UnauthorizedAccessException();
            }
            return (string)token;
        }
    }
}


