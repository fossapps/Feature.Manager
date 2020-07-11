using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.Features.ViewModels;
using Feature.Manager.Api.Models;
using Feature.Manager.Api.Uuid;
using Microsoft.EntityFrameworkCore;

namespace Feature.Manager.Api.Features
{
    public interface IFeatureRepository
    {
        Task<Feature> FindByFeatId(string featId);
        Task<Feature> CreateFeature(CreateFeatureRequest request);
        Task<Feature> ResetFeatureToken(string featId, string newToken);
        Task<List<Feature>> All();
    }
    public class FeatureRepository : IFeatureRepository
    {
        private readonly ApplicationContext _db;
        private readonly IUuidService _uuidService;

        public FeatureRepository(ApplicationContext db, IUuidService uuidService)
        {
            _db = db;
            _uuidService = uuidService;
        }

        public Task<Feature> FindByFeatId(string featId)
        {
            return _db.Features.AsNoTracking().FirstOrDefaultAsync(x => x.FeatId == featId);
        }

        public async Task<Feature> CreateFeature(CreateFeatureRequest request)
        {
            var data = await _db.Features.AddAsync(new Feature
            {
                Description = request.Description,
                Hypothesis = request.Hypothesis,
                FeatId = request.FeatId,
                Id = _uuidService.GenerateUuId(),
                FeatureToken = _uuidService.GenerateUuId()
            });
            await _db.SaveChangesAsync();
            return data.Entity;
        }

        public async Task<Feature> ResetFeatureToken(string featId, string newToken)
        {
            var feature = await FindByFeatId(featId);
            _db.Attach(feature);
            feature.FeatureToken = newToken;
            await _db.SaveChangesAsync();
            return feature;
        }

        public async Task<List<Feature>> All()
        {
            return await _db.Features.AsNoTracking().ToListAsync();
        }
    }
}
