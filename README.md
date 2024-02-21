local.settings.json should look like this:

```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "DomainServiceApiUrl": "<url to domain service api without trailing slash>",
        "AviationReportingServiceApiUrl": "<url to aviation reporting service api without trailing slash>",
        "WildfireFinanceServiceApiUrl":"<url to aviation finance service api without trailing slash>"
        "WildfireFinanceServiceApiKey":"<api key from zaure>",
        "ContractAppUrl": "<Contracts App Base Url>"
    },
    "Host": {
        "CORS": "*",
        "CORSCredentials": false
    },

    "logging": {
        "logLevel": {
            "default": "Debug",
            "WCDS.WebFuncions.Core.Services.DomainService": "Debug",
            "WCDS.WebFuncions.Core.Services.TimeReportingService": "Debug"
        }
    }
}

set an env variable:
$env:connectionstring="Server=V-VD-PD-0-1367\SQLEXPRESS;Database=Contracts;Trusted_Connection=true;TrustServerCertificate=true;"

You might have to run management studio as an admin and then create a user with the dbcreator perms.
Run the migration command
dotnet ef database update

```
