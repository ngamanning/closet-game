using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelTraining.Services;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace ModelTraining.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageTrainingController : ControllerBase
    {
        private readonly IImageClassificationService _imageTrainingService;
        private readonly ILogger<ImageTrainingController> _logger;

        public ImageTrainingController(ILogger<ImageTrainingController> logger, IImageClassificationService imageService)
        {
            _logger = logger;
            _imageTrainingService = imageService;
        }

    }
}
