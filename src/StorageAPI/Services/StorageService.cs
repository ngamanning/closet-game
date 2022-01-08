using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Services
{
    public class StorageService : IStorageService
    {
        private readonly IConfiguration _config;
        public StorageService(IConfiguration config)
        {
            _config = config;
        }

        public async Task UploadBlobsAsync(List<IFormFile> _files)
        {
            if ( _files is null || _files.Count == 0) return;
            try
            {
                string connectionString = _config.GetConnectionString("FashionGameBlob");
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                string containerName = "fsgame" + Guid.NewGuid().ToString();
                BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
                foreach (var file in _files)
                {
                    BlobClient blobClient = containerClient.GetBlobClient(file.FileName);
                    using Stream uploadFileStream = file.OpenReadStream();
                    await blobClient.UploadAsync(uploadFileStream, true);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
