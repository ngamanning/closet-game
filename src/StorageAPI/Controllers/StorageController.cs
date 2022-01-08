using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StorageAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        public StorageController (IStorageService storageService)
        {
            _storageService = storageService;
        }
        [HttpPost]
        [Route("upload-images")]
        public async Task UploadImages()
        {
            var files = Request.Form.Files.ToList();
            await _storageService.UploadBlobsAsync(files);
        }
    }
}
