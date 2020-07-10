using System.Threading.Tasks;
using Feature.Manager.Api.Features.ViewModels;

namespace Feature.Manager.Api.Features
{
    public interface IFeatureRepository
    {
        Task<Feature> FindByFeatId(string featId);
        Task<Feature> CreateFeature(CreateFeatureRequest request);
    }
    public class FeatureRepository
    {
        
    }
}