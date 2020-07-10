using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns.ViewModels;

namespace Feature.Manager.Api.FeatureRuns
{
    public interface IFeatureRunService
    {
        Task<FeatureRun> CreateFeatureRun(CreateFeatureRunRequest request);
        Task<List<FeatureRun>> GetRunsForFeatureByFeatId(string featId);
        Task<List<FeatureRun>> GetRunsForFeatureById(string id);
        Task<FeatureRun> StopFeatureRun(StopFeatureRunRequest request);
    }
    public class FeatureRunService
    {
        
    }
}
