using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TotechsIdentity.AppSettings;
using TotechsIdentity.Services.IService;

namespace TotechsIdentity.Services
{
    public class AzureBlobStorageMediaService : IMediaService
    {
        private readonly IOptionsMonitor<AzureStorageConfig> _storageConfig;
        private StorageSharedKeyCredential _storageCredentials;
        public AzureBlobStorageMediaService(IOptionsMonitor<AzureStorageConfig> azureStorageConfig,
                                             StorageSharedKeyCredential storageCredentials)
        {
            _storageConfig = azureStorageConfig;
            _storageCredentials = storageCredentials;
        }

        public Task<List<string>> GetThumbNailUrls()
        {
            throw new NotImplementedException();
        }

        public bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Tuple<bool, string>> UploadFileToStorage(Stream fileStream,
                                                                   string fileName)
        {
            var blobUri = new Uri("https://" +
                                  _storageConfig.CurrentValue.AccountName +
                                  ".blob.core.windows.net/" +
                                  _storageConfig.CurrentValue.ImageContainer +
                                  "/" + fileName);
            // Create the blob client.
            var blobClient = new BlobClient(blobUri, _storageCredentials);

            // Upload the file
            await blobClient.UploadAsync(fileStream);

            return new Tuple<bool, string>(await Task.FromResult(true), blobClient.Uri.AbsoluteUri);
        }

        public async Task<Tuple<bool, string>> UploadAvatarToStorage(Stream fileStream,
                                                                     string fileName)
        {
            var blobUri = new Uri("https://" +
                                  _storageConfig.CurrentValue.AccountName +
                                  ".blob.core.windows.net/" +
                                  _storageConfig.CurrentValue.ImageContainer +
                                  "/" + fileName);
            // Create the blob client.
            var blobClient = new BlobClient(blobUri, _storageCredentials);

            // Upload the file
            await blobClient.UploadAsync(fileStream);

            return new Tuple<bool, string>(await Task.FromResult(true), blobClient.Uri.AbsoluteUri);
        }
    }
}
