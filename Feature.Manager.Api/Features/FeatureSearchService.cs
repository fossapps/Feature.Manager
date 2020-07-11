using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.Features.Exceptions;

namespace Feature.Manager.Api.Features
{
    public interface IFeatureSearchService
    {
        Task<List<Feature>> SearchFeatureByFeatId(string partialFeatId);
    }
    public class FeatureSearchService : IFeatureSearchService
    {
        private readonly IFeatureSearchRepository _featureSearchRepository;

        public FeatureSearchService(IFeatureSearchRepository featureSearchRepository)
        {
            _featureSearchRepository = featureSearchRepository;
        }

        public async Task<List<Feature>> SearchFeatureByFeatId(string partialFeatId)
        {
            try
            {
                return await _featureSearchRepository.SearchFeatureByFeatId(partialFeatId);
            }
            catch (Exception e)
            {
                throw new UnknownDbException(e.Message);
            }
        }
    }
}