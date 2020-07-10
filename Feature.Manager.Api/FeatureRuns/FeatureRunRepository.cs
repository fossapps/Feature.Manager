using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns.ViewModels;
using Feature.Manager.Api.Features.ViewModels;
using Feature.Manager.Api.Models;
using Feature.Manager.Api.Uuid;
using Microsoft.EntityFrameworkCore;

namespace Feature.Manager.Api.FeatureRuns
{
    public interface IFeatureRunRepository
    {
        Task<FeatureRun> CreateFeatureRun(CreateFeatureRunRequest request);
        Task<List<FeatureRun>> GetRunsForFeatureByFeatId(string featId);
        Task<FeatureRun> StopFeatureRun(StopFeatureRunRequest request);
        Task<FeatureRun> GetById(string featureRunId);
        Task<List<RunningFeature>> GetRunningFeatures();
    }
    public class FeatureRunRepository : IFeatureRunRepository
    {
        private readonly ApplicationContext _db;
        private readonly IUuidService _uuidService;

        public FeatureRunRepository(ApplicationContext db, IUuidService uuidService)
        {
            _db = db;
            _uuidService = uuidService;
        }

        public async Task<FeatureRun> CreateFeatureRun(CreateFeatureRunRequest request)
        {
            var response = await _db.FeatureRuns.AddAsync(new FeatureRun
            {
                Allocation = request.Allocation,
                EndAt = request.EndAt,
                StartAt = request.StartAt,
                FeatId = request.FeatId,
                Id = _uuidService.GenerateUuId(),
                RunToken = _uuidService.GenerateUuId(),
            });
            await _db.SaveChangesAsync();
            return response.Entity;
        }

        public Task<List<FeatureRun>> GetRunsForFeatureByFeatId(string featId)
        {
            return _db.FeatureRuns.AsNoTracking().Where(x => x.FeatId == featId).ToListAsync();
        }

        public async Task<FeatureRun> StopFeatureRun(StopFeatureRunRequest request)
        {
            var featureRun = await GetById(request.RunId);
            _db.Attach(featureRun);
            Enum.TryParse(request.StopResult, out StopResult stopResult);
            featureRun.StopResult = stopResult;
            featureRun.EndAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return featureRun;
        }

        public Task<FeatureRun> GetById(string featureRunId)
        {
            return _db.FeatureRuns.AsNoTracking().FirstOrDefaultAsync(x => x.Id == featureRunId);
        }

        public async Task<List<RunningFeature>> GetRunningFeatures()
        {
            return await _db
                .FeatureRuns
                .AsNoTracking()
                .Where(x => (DateTime.Now > x.StartAt && DateTime.Now < x.EndAt) || x.StopResult == StopResult.AllB)
                .Join(_db.Features, run => run.FeatId, feature => feature.FeatId, (run, feature) => new RunningFeature
                {
                    Allocation = run.Allocation,
                    FeatureToken = feature.FeatureToken,
                    RunToken = run.RunToken,
                    RunId = run.Id,
                    RunStatus = run.StopResult,
                    FeatureId = feature.FeatId
                })
                .ToListAsync();
        }
    }
}
