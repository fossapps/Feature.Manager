using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feature.Manager.Api.FeatureRuns
{
    public class FeatureRun
    {
        public string Id { set; get; }
        public int Allocation { set; get; }
        public string FeatId { set; get; }
        public DateTime StartAt { set; get; }
        public DateTime? EndAt { set; get; }
        public string RunToken { set; get; }
        public string StopResultString
        {
            get => StopResult.ToString();
            set
            {
                Enum.TryParse(value, out StopResult result);
                StopResult = result;
            }
        }

        [NotMapped]
        public StopResult StopResult { set; get; }
    }

    public enum StopResult
    {
        AllB,
        Removed,
    }
}
