using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Services
{
    public interface IDomainService
    {
        public Task<Response<RateUnit>> GetRateUnits();
        public Task<Response<RateType>> GetRateTypes();
    }

    public class DomainService : IDomainService
    {
        private readonly ILogger Log;
        private readonly HttpClient HttpClient;
        public DomainService(ILogger<DomainService> log, HttpClient httpClient)
        {
            HttpClient = httpClient;
            Log = log ?? throw new ArgumentNullException(nameof(log)); HttpClient = httpClient;
            ;
        }

        public async Task<Response<RateUnit>> GetRateUnits()
        {
            return await GetDomainObjects<RateUnit, FilterByRateUnit>(new FilterByRateUnit(), "/rate_units/get");
        }

        public async Task<Response<RateType>> GetRateTypes()
        {
            return await GetDomainObjects<RateType, FilterByRateType>(new FilterByRateType(), "/rate_types/get");
        }

        private async Task<Response<T>> GetDomainObjects<T, U>(U filterBy, string relativePath) where U : IFilterBy
        {
            var url = Environment.GetEnvironmentVariable("DomainServiceApiUrl");
            if (url == null)
            {
                Log.LogError("DomainServiceApiUrl not found!");
                throw new Exception("DomainServiceApiUrl not found");
            }
            url = url + relativePath;
            Log.LogInformation("Domain service url: {url}", url);

            var request = new Request<U>();
            request.FilterBy = filterBy;
            var response = await HttpClient.PostAsJsonAsync<Request<U>>(url, request);

            response.EnsureSuccessStatusCode();

            // Handle the http response
            var json = await response.Content.ReadAsStringAsync();
            Response<T> responseData = JsonConvert.DeserializeObject<Response<T>>(json);

            return responseData;
        }
    }
}


