using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Services;

namespace WCDS.WebFuncions.Controller
{
    public interface IAzureStorageController
    {
        public Task<bool> UploadFileAsync(string fileName, string content);
        public Task<bool> DeleteFileAsync(string fileName);
        public Task<string> ReadFileAsync(string fileName);
        public Task<bool> CheckFileExistsAsync(string fileName);
    }
    public class AzureStorageController : IAzureStorageController
    {
        private readonly ILogger _log;
        private readonly string _connectionStringKey = "webStorageConnectionString";
        private readonly string _containerNameKey = "financeStorageContainer";

        string _connectionString;
        string _containerName;
        public AzureStorageController(ILogger log, IMapper mapper)
        {
            _log = log;
            _connectionString = Environment.GetEnvironmentVariable(_connectionStringKey);
            _containerName = Environment.GetEnvironmentVariable(_containerNameKey);
        }
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            _log.LogInformation(string.Format("AzureStorageController:DeleteFileAsync - Deleting file: {0}", fileName));
            bool result = false;
            try
            {
                // Get a reference to the blob container
                BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);

                // Get a reference to the blob
                BlobClient blob = container.GetBlobClient(fileName);

                // Delete the file from Azure Blob Storage
                result = await blob.DeleteIfExistsAsync();
                if (result)
                    _log.LogInformation(string.Format("AzureStorageController:DeleteFileAsync - Deleted file: {0}", fileName));
            }
            catch (Exception ex)
            {
                _log.LogError(string.Format("AzureStorageController:DeleteFileAsync An error has occured while deleting file: {0}, ErrorMessage: {1}, InnerException: {2}", fileName, ex.Message, ex.InnerException));
            }
            return result;
        }

        public async Task<string> ReadFileAsync(string fileName)
        {
            string fileContent = "";
            try
            {
                _log.LogInformation(string.Format("AzureStorageController:ReadFileAsync - Looking for file: {0}", fileName));
                // Get a reference to the blob container
                BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);

                // Get a reference to the blob
                BlobClient blob = container.GetBlobClient(fileName);

                // Download the file content from Azure Blob Storage
                using (MemoryStream stream = new MemoryStream())
                {
                    await blob.DownloadToAsync(stream);
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        fileContent = await reader.ReadToEndAsync();
                    }
                }
                if (string.IsNullOrEmpty(fileContent))
                    _log.LogInformation(string.Format("AzureStorageController:ReadFileAsync - File not found : {0}", fileName));
            }
            catch (Exception ex)
            {
                _log.LogError(string.Format("AzureStorageController:ReadFileAsync An error has occured while deleting file: {0}, ErrorMessage: {1}, InnerException: {2}", fileName, ex.Message, ex.InnerException));
            }
            return fileContent;
        }

        public async Task<bool> UploadFileAsync(string fileName, string content)
        {
            bool result = false;
            try
            {
                _log.LogInformation(string.Format("AzureStorageController:UploadFileAsync - Uploading file: {0}", fileName));
                // Get a reference to the blob container
                BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);

                // Create the container if it doesn't exist
                await container.CreateIfNotExistsAsync();

                // Get a reference to the blob
                BlobClient blob = container.GetBlobClient(fileName);

                // Upload the file content to Azure Blob Storage
                using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
                {
                    var response = await blob.UploadAsync(stream, true);
                    result = true;
                }
                _log.LogInformation(string.Format("AzureStorageController:UploadFileAsync - File Uploaded successfully: {0}", fileName));

            }
            catch (Exception ex)
            {
                _log.LogError(string.Format("AzureStorageController:UploadFileAsync An error has occured while deleting file: {0}, ErrorMessage: {1}, InnerException: {2}", fileName, ex.Message, ex.InnerException));
            }
            return result;
        }

        public async Task<bool> CheckFileExistsAsync(string fileName)
        {
            bool result = false;
            try
            {
                _log.LogInformation(string.Format("AzureStorageController:CheckFileExistsAsync - Checking if file exists: {0}", fileName));
                // Get a reference to the blob container
                BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);

                // Get a reference to the blob
                BlobClient blob = container.GetBlobClient(fileName);

                // Check if the blob exists
                result = await blob.ExistsAsync();
                if (result)
                    _log.LogInformation(string.Format("AzureStorageController:CheckFileExistsAsync - File exists: {0}", fileName));
                else
                    _log.LogInformation(string.Format("AzureStorageController:CheckFileExistsAsync - File does not exist: {0}", fileName));
            }
            catch (Exception ex)
            {
                _log.LogError(string.Format("AzureStorageController:CheckFileExistsAsync An error has occured while deleting file: {0}, ErrorMessage: {1}, InnerException: {2}", fileName, ex.Message, ex.InnerException));
            }
            return result;
        }
    }
}
