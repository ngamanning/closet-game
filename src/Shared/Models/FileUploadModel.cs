using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
    }
}
