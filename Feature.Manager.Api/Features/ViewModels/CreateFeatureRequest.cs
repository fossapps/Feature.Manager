using System.ComponentModel.DataAnnotations;

namespace Feature.Manager.Api.Features.ViewModels
{
    public class CreateFeatureRequest
    {
        [Required]
        public string FeatId { set; get; }
        [Required]
        public string Description { set; get; }
        [Required]
        public string Hypothesis { set; get; }
    }
}
