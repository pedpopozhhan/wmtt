using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Services
{
    public interface ITimeReportingService
    {
        public Task<Response<TimeReportCostDetailDto>> GetTimeReportByIds(int[] ids);
    }

    public class TimeReportingService : ITimeReportingService
    {
        private readonly ILogger Log;
        private readonly HttpClient HttpClient;
        ApplicationDBContext dbContext;
        public TimeReportingService(ILogger<TimeReportingService> log, HttpClient httpClient)
        {
            HttpClient = httpClient;
            Log = log ?? throw new ArgumentNullException(nameof(log));
            dbContext = new ApplicationDBContext();
        }

        public async Task<Response<TimeReportCostDetailDto>> GetTimeReportByIds(int[] ids)
        {
            var url = Environment.GetEnvironmentVariable("AviationReportingServiceApiUrl");
            if (url == null)
            {
                Log.LogError("AviationReportingServiceApiUrl not found!");
                throw new Exception("AviationReportingServiceApiUrl not found");
            }
            url = url + "/flight-report/get/cost-details";
            Log.LogInformation("GetTimeReportById url: {url}", url);

            var request = new Request<FilterByCostDetails>();
            request.FilterBy = new FilterByCostDetails(ids);
            var response = await HttpClient.PostAsJsonAsync<Request<FilterByCostDetails>>(url, request);

            response.EnsureSuccessStatusCode();

            // Handle the http response
            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            Response<TimeReportCostDetailDto> responseData = JsonConvert.DeserializeObject<Response<TimeReportCostDetailDto>>(json, settings);
            if(responseData.Data != null && responseData.Data.Length > 0)
            {
                responseData.Data = responseData.Data.Where(i => !dbContext.InvoiceTimeReportCostDetails.Any(r => r.TimeReportCostDetailReferenceId == i.FlyingHoursId)).ToArray();
            }

            return responseData;
        }
    }
}


