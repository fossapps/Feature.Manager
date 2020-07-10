using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns;
using Feature.Manager.Api.Features.ViewModels;

namespace Feature.Manager.Api.Features
{
    public interface IFeatureService
    {
        Task<Feature> Create(CreateFeatureRequest request);
        Task<Feature> ResetFeatureToken();
        Task<Feature> GetFeatureById(string id);
        Task<Feature> GetFeatureByFeatId(string featId);
        Task<List<Feature>> SearchFeatureByFeatId(string partialFeatId);
        Task<List<RunningExperiment>> GetRunningFeatures();
        Task<List<FeatureRun>> CreateFeatureRun(CreateFeatureRequest createFeatureRequest);
    }
    public class FeatureService : IFeatureService
    {
        public async Task<Feature> Create(CreateFeatureRequest request)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Feature> ResetFeatureToken()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Feature> GetFeatureById(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Feature> GetFeatureByFeatId(string featId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<Feature>> SearchFeatureByFeatId(string partialFeatId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<RunningExperiment>> GetRunningFeatures()
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<FeatureRun>> CreateFeatureRun(CreateFeatureRequest createFeatureRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}
