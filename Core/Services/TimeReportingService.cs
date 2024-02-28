using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly ILogger Log;
        private readonly HttpClient HttpClient;
        private readonly IHttpContextAccessor httpContextAccessor;
        ApplicationDBContext dbContext;
        public TimeReportingService(ILogger<TimeReportingService> log, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            HttpClient = httpClient;
            this.httpContextAccessor = httpContextAccessor;
            Log = log ?? throw new ArgumentNullException(nameof(log));
            dbContext = new ApplicationDBContext();
        }

        public async Task<Response<TimeReportCostDetailDto>> GetTimeReportByIds(int[] ids)
        {
            var token = this.httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                Log.LogError("No Authorization header found");
                throw new UnauthorizedAccessException();
            }
            var url = Environment.GetEnvironmentVariable("AviationReportingServiceApiUrl");
            if (url == null)
            {
                Log.LogError("AviationReportingServiceApiUrl not found!");
                throw new Exception("AviationReportingServiceApiUrl not found");
            }
            url = url + "/flight-report/get/cost-details";
            Log.LogInformation("GetTimeReportById url: {url}", url);

            // var request = new Request<FilterByCostDetails>();
            // request.FilterBy = new FilterByCostDetails(ids);
            // var response = await HttpClient.PostAsJsonAsync<Request<FilterByCostDetails>>(url, request);
            var requestObject = new Request<FilterByCostDetails> { FilterBy = new FilterByCostDetails(ids) };
            var jsonRequest = JsonConvert.SerializeObject(requestObject);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", (string)token);
            requestMessage.Content = requestContent;

            var response = await HttpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            // Handle the http response
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            Response<TimeReportCostDetailDto> responseData = JsonConvert.DeserializeObject<Response<TimeReportCostDetailDto>>(json, settings);
            if (responseData.Data != null && responseData.Data.Length > 0)
            {
                responseData.Data = responseData.Data.Where(i => !dbContext.InvoiceTimeReportCostDetails.Any(r => r.FlightReportCostDetailsId == i.FlightReportCostDetailsId)).ToArray();
            }

            return responseData;
        }

        public async Task<Response<TimeReportCostDto>> GetTimeReportCosts(string contractNumber, string status)
        {
            var token = this.httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                Log.LogError("No Authorization header found");
                throw new UnauthorizedAccessException();
            }
            var url = Environment.GetEnvironmentVariable("AviationReportingServiceApiUrl");
            if (url == null)
            {
                Log.LogError("AviationReportingServiceApiUrl not found!");
                throw new Exception("AviationReportingServiceApiUrl not found");
            }
            url += "/flight-report-dashboard/cost";
            Log.LogInformation("GetTimeReportCosts url: {url}", url);

            // var request = new Request<FilterByCostRequest>
            // {
            //     FilterBy = new FilterByCostRequest(contractNumber, status)
            // };
            // var response = await HttpClient.PostAsJsonAsync(url, request);
            var requestObject = new Request<FilterByCostRequest> { FilterBy = new FilterByCostRequest(contractNumber, status) };
            var jsonRequest = JsonConvert.SerializeObject(requestObject);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", (string)token);
            requestMessage.Content = requestContent;

            var response = await HttpClient.SendAsync(requestMessage);
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
                Log.LogError("AviationReportingServiceApiUrl not found!");
                throw new Exception("AviationReportingServiceApiUrl not found");
            }
            url += "/flight-report-dashboard/vendors/get";
            Log.LogInformation("GetContracts url: {url}", url);

            var request = new Request<FilterBy>
            {
                FilterBy = new FilterBy()
            };
            var response = await HttpClient.PostAsJsonAsync(url, request);

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
    }
}


