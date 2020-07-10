using System.Threading.Tasks;
using Feature.Manager.Api.Features.ViewModels;
using Feature.Manager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Feature.Manager.Api.Features
{
    public interface IFeatureRepository
    {
        Task<Feature> FindByFeatId(string featId);
        Task<Feature> CreateFeature(CreateFeatureRequest request);
        Task<Feature> ResetFeatureToken(string featId, string newToken);
    }
    public class FeatureRepository : IFeatureRepository
    {
        private readonly ApplicationContext _db;

        public FeatureRepository(ApplicationContext db)
        {
            _db = db;
        }

        public Task<Feature> FindByFeatId(string featId)
        {
            return _db.Features.FirstOrDefaultAsync(x => x.FeatId == featId);
        }

        public async Task<Feature> CreateFeature(CreateFeatureRequest request)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Feature> ResetFeatureToken(string featId, string newToken)
        {
            throw new System.NotImplementedException();
        }
    }
}