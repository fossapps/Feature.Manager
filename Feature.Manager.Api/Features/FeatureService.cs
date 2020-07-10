using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns;
using Feature.Manager.Api.Features.Exceptions;
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
    }
    public class FeatureService : IFeatureService
    {
        private readonly IFeatureRepository _featureRepository;

        public FeatureService(IFeatureRepository featureRepository)
        {
            _featureRepository = featureRepository;
        }

        public async Task<Feature> Create(CreateFeatureRequest request)
        {
            try
            {
                var feature = await _featureRepository.FindByFeatId(request.FeatId);
                if (feature != null)
                {
                    throw new FeatureAlreadyExistsException();
                }

                return await _featureRepository.CreateFeature(request);
            }
            catch (FeatureAlreadyExistsException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FeatureCreatingFailedException(e.Message);
            }
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
    }
}
