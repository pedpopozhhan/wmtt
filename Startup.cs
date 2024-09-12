
using System;
using System.Linq;
using AutoMapper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WCDS.WebFuncions.Core.Common.CAS;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Core.Services.CAS;

[assembly: FunctionsStartup(typeof(WCDS.WebFuncions.Startup))]
namespace WCDS.WebFuncions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var localEnvironment = Environment.GetEnvironmentVariable("LocalEnvironment")?.ToLower() == "true";
            if (localEnvironment)
            {
                builder.Services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.SetMinimumLevel(LogLevel.None);
                    loggingBuilder.AddFilter("WCDS.WebFuncions.Core.Services.DomainService", LogLevel.None);
                    loggingBuilder.AddFilter("WCDS.WebFuncions.Core.Services.TimeReportingService", LogLevel.None);
                    loggingBuilder.AddFilter("WCDS.WebFuncions.Core.Services.WildfireFinanceService", LogLevel.None);
                });
            }
            else
            {
                builder.Services.AddLogging();

            }
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
            builder.Services.AddScoped<ICASRepository, CASRepository>();
            builder.Services.AddScoped<ICASService, CASService>();
            builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("connectionstring")), ServiceLifetime.Scoped);
            builder.Services.AddDbContext<CASDBContext>(options => options.UseOracle(Environment.GetEnvironmentVariable("casconnectionstring")), ServiceLifetime.Scoped);
            var serviceProvider = builder.Services.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}