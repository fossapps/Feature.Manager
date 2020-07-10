using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns.Exceptions;
using Feature.Manager.Api.FeatureRuns.ViewModels;
using Feature.Manager.Api.Features.Exceptions;
using Feature.Manager.Api.Features.ViewModels;

namespace Feature.Manager.Api.FeatureRuns
{
    public interface IFeatureRunService
    {
        Task<FeatureRun> CreateFeatureRun(CreateFeatureRunRequest request);
        Task<List<FeatureRun>> GetRunsForFeatureByFeatId(string featId);
        Task<FeatureRun> StopFeatureRun(StopFeatureRunRequest request);
        Task<FeatureRun> GetById(string runId);
        Task<List<RunningFeature>> GetRunningFeatures();
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
            try
            {
                var runs = await _featureRunRepository.GetRunsForFeatureByFeatId(request.FeatId);
                if (runs.Any(x => x.EndAt == null))
                {
                    throw new FeatureAlreadyRunningException();
                }

                return await _featureRunRepository.CreateFeatureRun(request);
            }
            catch (FeatureAlreadyRunningException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new UnknownDbException(e.Message);
            }
        }

        public async Task<List<FeatureRun>> GetRunsForFeatureByFeatId(string featId)
        {
            try
            {
                return await _featureRunRepository.GetRunsForFeatureByFeatId(featId);
            }
            catch (Exception e)
            {
                throw new UnknownDbException(e.Message);
            }
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

        public async Task<List<RunningFeature>> GetRunningFeatures()
        {
            try
            {
                return await _featureRunRepository.GetRunningFeatures();
            }
            catch (Exception e)
            {
                throw new UnknownDbException(e.Message);
            }
        }
    }
}
