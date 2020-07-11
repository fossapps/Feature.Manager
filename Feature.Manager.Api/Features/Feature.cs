using System.ComponentModel.DataAnnotations;

namespace Feature.Manager.Api.Features
{
    public class Feature
    {
        public string Id { set; get; }

        [Required]
        public string Hypothesis { set; get; }

        [Required]
        public string Description { set; get; }

        [Required]
        public string FeatId { set; get; }

        [Required]
        public string FeatureToken { set; get; }
    }
}
