using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StorageAPI.Services
{
    public interface IStorageService
    {
        Task UploadBlobsAsync(List<IFormFile> _files);
    }
}