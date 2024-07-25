
using System;
using System.Linq;
using AutoMapper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Services;

[assembly: FunctionsStartup(typeof(WCDS.WebFuncions.Startup))]
namespace WCDS.WebFuncions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            //by default, the Application Insights SDK adds a logging filter that instructs the logger to capture only warnings and more severe logs. 
            // https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows#managing-log-levels
            builder.Services.Configure<LoggerFilterOptions>(options =>
            {
                LoggerFilterRule defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                    == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
                if (defaultRule is not null)
                {

                    options.Rules.Remove(defaultRule);
                }
            });

            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(typeof(Startup));
            builder.Services.AddScoped<IDomainService, DomainService>();
            builder.Services.AddScoped<ITimeReportingService, TimeReportingService>();
            builder.Services.AddScoped<IWildfireFinanceService, WildfireFinanceService>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("connectionstring")), ServiceLifetime.Scoped);
            var serviceProvider = builder.Services.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}