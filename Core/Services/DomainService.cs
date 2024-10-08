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
        public Task<Response<CorporateRegion>> GetCorporateRegion();

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

        public async Task<Response<CorporateRegion>> GetCorporateRegion()
        {
            return await GetDomainObjects<CorporateRegion, FilterByCorporateRegion>(new FilterByCorporateRegion() { ColumnName = "CorporateRegionTypeId", ColumnValue ="4" }
                                                                                    , "/corporate_region/get"); ;
        }

        public async Task<Response<RateType>> GetRateTypesByService(string serviceName)
        {
            return await GetDomainObjects<RateType, FilterByRateType>(new FilterByRateType(), "/rate_types/get", serviceName);
        }
        
        private async Task<Response<T>> GetDomainObjects<T, U>(U filterBy, string relativePath, string serviceName = null) where U : IFilterBy
        {
            var token = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIyaUpOSFlENVFxdlNkVU9rZ0VHYnEwakU2RlM4X2VBOWxldW11VlYyRDA4In0.eyJleHAiOjE3MTI5MTA4OTcsImlhdCI6MTcxMjg5NjQ5OCwiYXV0aF90aW1lIjoxNzEyODk2NDk3LCJqdGkiOiJkY2UxNjUzMS03ZTFiLTQ5YTItOTZmZS1iNWYzMDhkYjMzN2UiLCJpc3MiOiJodHRwczovL2FjY2Vzcy5hZHNwLWRldi5nb3YuYWIuY2EvYXV0aC9yZWFsbXMvZThhNTRlOGItY2E0YS00MDRhLTliYTEtMDBiNTkzY2QzZmY4IiwiYXVkIjpbIkZsaWdodFJlcG9ydHMiLCJmaW5hbmNlIiwiYWNjb3VudCJdLCJzdWIiOiI0NmY1NjcxYy04NWYwLTQzYmUtOTU2OC04NDVhYmRlZjdhYjIiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJmaW5hbmNlLWFwcCIsInNlc3Npb25fc3RhdGUiOiI2MjUxNGZlYS0yZTQzLTQ4Y2MtYjliOC00MWQxMDQ1NDg5OTMiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vY29udHJhY3RzLndpbGRmaXJlYXBwcy1kZXYuYWxiZXJ0YS5jYSIsImh0dHA6Ly9sb2NhbGhvc3Q6MzAwMCJdLCJyZXNvdXJjZV9hY2Nlc3MiOnsiRmxpZ2h0UmVwb3J0cyI6eyJyb2xlcyI6WyJwX0F2aWF0X0ZsaWdodFJwdEFwcFJldF9FIiwicF9BdmlhdF9GbGlnaHRScHRfUiIsInBfQXZpYXRfRmxpZ2h0UnB0X0QiLCJwX0F2aWF0X0ZsaWdodFJwdF9XIl19LCJmaW5hbmNlIjp7InJvbGVzIjpbIkZpbl9JbnZvaWNlX1YiLCJGaW5fSW52b2ljZV9XIl19LCJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im9wZW5pZCBwcm9maWxlIGVtYWlsIHdlYXRoZXItaW5mby1zY29wZSIsInNpZCI6IjYyNTE0ZmVhLTJlNDMtNDhjYy1iOWI4LTQxZDEwNDU0ODk5MyIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiZ2VuZGVyIjoidW5zcGVjaWZpZWQiLCJkaXNwbGF5IE5hbWUiOiJJZnRpa2hhclFhbWFyIiwibmFtZSI6IklmdGlraGFyIFFhbWFyIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiaWZ0aWtoYXIucWFtYXJAZ292LmFiLmNhIiwiZ2l2ZW5fbmFtZSI6IklmdGlraGFyIiwibWlkZGxlX25hbWUiOiIiLCJmYW1pbHlfbmFtZSI6IlFhbWFyIiwiZW1haWwiOiJpZnRpa2hhci5xYW1hckBnb3YuYWIuY2EifQ.ZO9HG1gAdNwtPChEgpPMrj8kjWWDca8WVGO6moaqAQxOfKgR4eBBGUOf7jZ0YSIB_QeGoF22nRyhJYaUsDYi2BRAi0-Uu6UQsJJRedi0-5d9LPk8tF1v7Y7iHUuRqN9f-uxTvqQ_0Snles9LtKM1L2XathVS_Zy9aqNH0fbTPWid-FlJzXfG1wk9Z0IdRN8sRkZRDW3DqU8OsYZ0_IOuHsjGbvHFxSPNhr2ribUzl8LNlKEBH9vck084oFyZcR5JJHgiPKbPCuzB-6HiMXGCXJIoINAa4cE68C6glcsO6Ca39OscFazk1MiQLvw-jeJlP1MPMTQ7itWr820h4Gu9EQ";
            //var token = this.httpContextAccessor.HttpContext.Request.Headers["Authorization"];
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


