using System;
using System.ComponentModel.DataAnnotations;

namespace Feature.Manager.Api.FeatureRuns.ViewModels
{
    public class CreateFeatureRunRequest
    {
        [Required]
        public string FeatId { set; get; }

        [Required]
        [Range(0, 100)]
        public int Allocation { set; get; }

        [Required]
        public DateTime StartAt { set; get; }

        public DateTime? EndAt { set; get; }
    }
}
