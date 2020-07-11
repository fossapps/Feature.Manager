using System.ComponentModel.DataAnnotations;

namespace Feature.Manager.Api.FeatureRuns.ViewModels
{
    public class StopFeatureRunRequest
    {
        [Required]
        public string RunId { set; get; }

        [Required]
        public string StopResult { set; get; }
    }
}
