using System;
using System.ComponentModel.DataAnnotations;

namespace Feature.Manager.Api.FeatureRuns
{
    public class FeatureRun
    {
        public string Id { set; get; }
        public int Allocation { set; get; }

        [Required]
        public string FeatId { set; get; }
        public DateTime StartAt { set; get; }
        public DateTime? EndAt { set; get; }

        [Required]
        public string RunToken { set; get; }

        public StopResult? StopResult { set; get; }
    }

    public enum StopResult
    {
        AllB,
        Removed,
        AllA,
        ChangeSettings
    }
}
