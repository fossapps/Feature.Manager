using Feature.Manager.Api.FeatureRuns;

namespace Feature.Manager.Api.Features.ViewModels
{
    public class RunningFeature
    {
        public string FeatureToken { set; get; }
        public int Allocation { set; get; }
        public string FeatureId { set; get; }
        public string RunId { set; get; }
        public string RunToken { set; get; }
        public StopResult RunStatus { set; get; }
    }
}
