local.settings.json should look like this:

```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "DomainServiceApiUrl": "<url to domain service api without trailing slash>",
        "AviationReportingServiceApiUrl": "<url to aviation reporting service api without trailing slash>"
    },
    "Host": {
        "CORS": "*",
        "CORSCredentials": false
    },
    "ConnectionStrings": {
        "SQLConnectionString": "<sqlclient-connection-string>"
    },
    "logging": {
        "logLevel": {
            "default": "Debug",
            "WCDS.WebFuncions.Core.Services.DomainService": "Debug",
            "WCDS.WebFuncions.Core.Services.TimeReportingService": "Debug"
        }
    }
}

```