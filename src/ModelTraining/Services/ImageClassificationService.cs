
// Documentation: https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/quickstarts/image-classification?tabs=visual-studio&pivots=programming-language-csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace ModelTraining.Services
{
    public class ImageClassificationService : IImageClassificationService
    {
        private readonly IConfiguration _configuration;
        public ImageClassificationService(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task<CustomVisionTrainingClient> AuthenticateTrainingAsync()
        {
            try
            {
                var key = _configuration.GetSection("imageResources: trainingKey").Value;
                var endpoint = _configuration.GetSection("imageResources: TrainingEndpoint").Value;

                return await Task<CustomVisionTrainingClient>.Factory.StartNew(() =>
                 {
                     CustomVisionTrainingClient trainingApi = new(
                     new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(key))
                     {
                         Endpoint = endpoint
                     };
                     return trainingApi;
                 });
            }
            catch
            {
                throw;
            }
        }

        public async Task<CustomVisionPredictionClient> AuthenticatePrediction()
        {
            try
            {
                // Create a prediction endpoint, passing in the obtained prediction key
                var predictionKey = _configuration.GetSection("imageResources: predictionKey").Value;
                var endpoint = _configuration.GetSection("imageResources: predictionEndpoint").Value;

                return await Task<CustomVisionPredictionClient>.Factory.StartNew(() =>
                {
                    CustomVisionPredictionClient predictionApi = new(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
                    {
                        Endpoint = endpoint
                    };
                    return predictionApi;
                });
            }
            catch
            {
                throw;
            }
        }

        public async Task<Project> CreateProject(string _projectName)
        {
            try
            {
                var trainingAPI = await AuthenticateTrainingAsync();
                var project = await trainingAPI.CreateProjectAsync(_projectName);
                return project;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tag> AddTagsAsync(string _projectName, string _tagName)
        {
            try
            {
                var trainingAPI = await AuthenticateTrainingAsync();
                var project = trainingAPI.GetProjects()
                    .Where(project => project.Name == _projectName)
                    .FirstOrDefault();
                if (project is null)
                {
                    throw new ArgumentException($"Unable to find project name {_projectName}");
                }
                var tag = await trainingAPI.CreateTagAsync(project.Id, _tagName);
                return tag;
            }
            catch
            {
                throw;
            }
        }

        public async Task UploadImages(List<IFormFile> _uploadedImages, Tag _tag)
        {
            try
            {
                var trainingAPI = await AuthenticateTrainingAsync();
                var project = trainingAPI.GetProjects().FirstOrDefault();
                foreach (var image in _uploadedImages)
                {
                    MemoryStream stream = new();
                    await image.CopyToAsync(stream);
                    await trainingAPI.CreateImagesFromDataAsync(project.Id, stream, new List<Guid>() { _tag.Id });
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task TrainProject(string _projectName)
        {
            try
            {
                CustomVisionTrainingClient trainingApi = await AuthenticateTrainingAsync();
                var project = trainingApi.GetProjects()
                    .Where(p => p.Name.ToLower() == _projectName)
                    .FirstOrDefault();
                if (project is null)
                    throw new Exception($"Unable to find project name {_projectName}");
                var iteration = trainingApi.TrainProject(project.Id);

                // The returned iteration will be in progress, and can be queried periodically 
                // to see when it has completed
                while (iteration.Status == "Training")
                {
                    Thread.Sleep(10000);
                    // Re-query the iteration to get it's updated status
                    iteration = trainingApi.GetIteration(project.Id, iteration.Id);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task PublishIteration(Guid _projectId, Guid _iterationId, string _modelName)
        {
            try
            {
                var trainingApi = await AuthenticateTrainingAsync();
                var project = trainingApi.GetProject(_projectId);
                var predictionResourceId = _configuration.GetSection("imageResources: predictionResourceId").Value;
                trainingApi.PublishIteration(_projectId, _iterationId, _modelName, predictionResourceId);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<object>> TestIteration(CustomVisionTrainingClient _trainingAPI,
            Guid _projectId, string _publishedModelName, IFormFile _image)
        {
            try
            {
                // Make a prediction against the new project
                var predictionApi = await AuthenticatePrediction();
                MemoryStream stream = new();
                await _image.CopyToAsync(stream);
                var result = await predictionApi.ClassifyImageWithNoStoreAsync(_projectId, _publishedModelName, stream);

                // Loop over each prediction and write out the results
                List<object> results = new();
                foreach (var c in result.Predictions)
                {
                    results.Add(new
                    {
                        tagName = c.TagName,
                        predictionValue = c.Probability,
                    });
                }
                return results;
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteProject(Guid _projectId)
        {
            try
            {
                var trainingApi = await AuthenticateTrainingAsync();
                await trainingApi.DeleteProjectAsync(_projectId);
            }
            catch
            {
                throw;
            }
        }
    }
}
