using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Common;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Services
{
    public interface IDomainService
    {
        public Task<Response<RateUnit>> GetRateUnits();
        public Task<Response<RateType>> GetRateTypes();
        public Task<Response<RateType>> GetRateTypesByService(string serviceName);
    }

    public class DomainService : IDomainService
    {
        private readonly ILogger Log;
        private readonly HttpClient HttpClient;
        private readonly IHttpContextAccessor httpContextAccessor;

        public DomainService(ILogger<DomainService> log, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            HttpClient = httpClient;
            this.httpContextAccessor = httpContextAccessor;
            Log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<Response<RateUnit>> GetRateUnits()
        {
            return await GetDomainObjects<RateUnit, FilterByRateUnit>(new FilterByRateUnit(), "/rate_units/get");
        }

        public async Task<Response<RateType>> GetRateTypes()
        {
            return await GetDomainObjects<RateType, FilterByRateType>(new FilterByRateType(), "/rate_types/get");
        }

        public async Task<Response<RateType>> GetRateTypesByService(string serviceName)
        {
            return await GetDomainObjects<RateType, FilterByRateType>(new FilterByRateType(), "/rate_types/get", serviceName);
        }
        
        private async Task<Response<T>> GetDomainObjects<T, U>(U filterBy, string relativePath, string serviceName = null) where U : IFilterBy
        {
            var token = this.httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                Log.LogError("No Authorization header found");
                throw new UnauthorizedAccessException();
            }

            var url = Environment.GetEnvironmentVariable("DomainServiceApiUrl");
            if (url == null)
            {
                Log.LogError("DomainServiceApiUrl not found!");
                throw new Exception("DomainServiceApiUrl not found");
            }
            url = url + relativePath;
            Log.LogInformation("Domain service url: {url}", url);

            var requestObject = new Request<U> { FilterBy = filterBy, ServiceName = serviceName };
            var jsonRequest = JsonConvert.SerializeObject(requestObject);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");


            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", (string)token);
            requestMessage.Content = requestContent;

            await LoggerHelper.LogRequestAsync(Log, requestMessage);
            var response = await HttpClient.SendAsync(requestMessage);
            await LoggerHelper.LogResponseAsync(Log, response);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();

            Response<T> responseData = JsonConvert.DeserializeObject<Response<T>>(jsonResponse);

            return responseData;
        }
    }
}


