using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TotechsIdentity.AppSettings;
using TotechsIdentity.Enums;
using TotechsIdentity.Services.IService;

namespace TotechsIdentity.Services
{
    public class AzureBlobStorageMediaService : IMediaService
    {
        private readonly IOptionsMonitor<AzureStorageConfig> _storageConfig;
        private StorageSharedKeyCredential _storageCredentials;
        private BlobContainerClient _avatarBlobContainerClient;

        public AzureBlobStorageMediaService(IOptionsMonitor<AzureStorageConfig> azureStorageConfig,
                                             StorageSharedKeyCredential storageCredentials,
                                             BlobContainerClient avatarBlobContainerClient)
        {
            _storageConfig = azureStorageConfig;
            _storageCredentials = storageCredentials;
            _avatarBlobContainerClient = avatarBlobContainerClient;
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
        
        public async Task<Tuple<string, string>> UploadAvatarToStorage(Stream fileStream,
                                                                     string fileName)
        {
            var blobGuid = Guid.NewGuid().ToString("N");
            var blobUri = new Uri("https://" +
                                  _storageConfig.CurrentValue.AccountName +
                                  ".blob.core.windows.net/" +
                                  _storageConfig.CurrentValue.ImageContainer +
                                  "/" + fileName);

            // Create the blob client.
            var blobClient = new BlobClient(blobUri, _storageCredentials);
            // Upload the file
            await blobClient.UploadAsync(fileStream);
            
            return new Tuple<string, string>(blobClient.Name, blobClient.Uri.AbsoluteUri);
        }

    }
}
