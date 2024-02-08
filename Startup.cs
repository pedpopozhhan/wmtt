using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WCDS.WebFuncions.Core.Services;

[assembly: FunctionsStartup(typeof(WCDS.WebFuncions.Startup))]
namespace WCDS.WebFuncions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddAutoMapper(typeof(Startup));
            builder.Services.AddSingleton<IDomainService, DomainService>();
            builder.Services.AddSingleton<ITimeReportingService, TimeReportingService>();
            builder.Services.AddSingleton<IWildfireFinanceService, WildfireFinanceService>();
        }
    }
}