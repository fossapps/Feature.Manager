using System;
using System.Threading.Tasks;
using Feature.Manager.Api.Features.Exceptions;
using Feature.Manager.Api.Features.ViewModels;
using Feature.Manager.Api.Uuid;

namespace Feature.Manager.Api.Features
{
    public interface IFeatureService
    {
        Task<Feature> Create(CreateFeatureRequest request);
        Task<Feature> ResetFeatureToken(string featId);
        Task<Feature> GetFeatureByFeatId(string featId);
    }

    public class FeatureService : IFeatureService
    {
        private readonly IFeatureRepository _featureRepository;
        private readonly IUuidService _uuidService;

        public FeatureService(IFeatureRepository featureRepository, IUuidService uuidService)
        {
            _featureRepository = featureRepository;
            _uuidService = uuidService;
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

        public async Task<Feature> ResetFeatureToken(string featId)
        {
            try
            {
                var feature = await _featureRepository.FindByFeatId(featId);
                if (feature == null)
                {
                    throw new FeatureNotFoundException();
                }
                return await _featureRepository.ResetFeatureToken(featId, _uuidService.GenerateUuId());
            }
            catch (FeatureNotFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FeatureTokenResetFailedException(e.Message);
            }
        }

        public async Task<Feature> GetFeatureByFeatId(string featId)
        {
            try
            {
                return await _featureRepository.FindByFeatId(featId);
            }
            catch (Exception e)
            {
                throw new UnknownDbException(e.Message);
            }
        }
    }
}
