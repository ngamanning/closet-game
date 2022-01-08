using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModelTraining.Services
{
    public interface IImageClassificationService
    {
        Task<Tag> AddTagsAsync(string _projectName, string _tagName);
        Task<CustomVisionPredictionClient> AuthenticatePrediction();
        Task<CustomVisionTrainingClient> AuthenticateTrainingAsync();
        Task<Project> CreateProject(string _projectName);
        Task DeleteProject(Guid _projectId);
        Task PublishIteration(Guid _projectId, Guid _iterationId, string _modelName);
        Task<List<object>> TestIteration(CustomVisionTrainingClient _trainingAPI, Guid _projectId, string _publishedModelName, IFormFile _image);
        Task TrainProject(string _projectName);
        Task UploadImages(List<IFormFile> _uploadedImages, Tag _tag);
    }
}