using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feature.Manager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Feature.Manager.Api.Features
{
    public interface IFeatureSearchRepository
    {
        Task<List<Feature>> SearchFeatureByFeatId(string partialFeatId);
    }
    public class FeatureSearchRepository : IFeatureSearchRepository
    {
        private readonly ApplicationContext _db;

        public FeatureSearchRepository(ApplicationContext db)
        {
            _db = db;
        }

        public Task<List<Feature>> SearchFeatureByFeatId(string partialFeatId)
        {
            return _db.Features.AsNoTracking().Where(x => x.FeatId.StartsWith(partialFeatId)).ToListAsync();
        }
    }
}
