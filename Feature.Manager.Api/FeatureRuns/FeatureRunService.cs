using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns.Exceptions;
using Feature.Manager.Api.FeatureRuns.ViewModels;
using Feature.Manager.Api.Features.Exceptions;

namespace Feature.Manager.Api.FeatureRuns
{
    public interface IFeatureRunService
    {
        Task<FeatureRun> CreateFeatureRun(CreateFeatureRunRequest request);
        Task<List<FeatureRun>> GetRunsForFeatureByFeatId(string featId);
        Task<FeatureRun> StopFeatureRun(StopFeatureRunRequest request);
        Task<FeatureRun> GetById(string runId);
    }
    public class FeatureRunService : IFeatureRunService
    {
        private readonly IFeatureRunRepository _featureRunRepository;

        public FeatureRunService(IFeatureRunRepository featureRunRepository)
        {
            _featureRunRepository = featureRunRepository;
        }

        public async Task<FeatureRun> CreateFeatureRun(CreateFeatureRunRequest request)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<FeatureRun>> GetRunsForFeatureByFeatId(string featId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<FeatureRun> StopFeatureRun(StopFeatureRunRequest request)
        {
            try
            {
                var result = await GetById(request.RunId);
                if (result == null)
                {
                    throw new FeatureRunNotFoundException();
                }

                if (!Enum.TryParse(request.StopResult, out StopResult stopResult))
                {
                    throw new InvalidStopResultValueException();
                }

                return await _featureRunRepository.StopFeatureRun(request);
            }
            catch (FeatureRunNotFoundException)
            {
                throw;
            }
            catch (InvalidStopResultValueException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new UnknownDbException(e.Message);
            }
        }

        public async Task<FeatureRun> GetById(string runId)
        {
            try
            {
                return await _featureRunRepository.GetById(runId);
            }
            catch (Exception e)
            {
                throw new UnknownDbException(e.Message);
            }
        }
    }
}
