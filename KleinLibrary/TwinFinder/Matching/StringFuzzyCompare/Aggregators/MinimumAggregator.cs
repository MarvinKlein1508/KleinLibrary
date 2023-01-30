#nullable disable
using TwinFinder.Matching.StringFuzzyCompare.Aggregators.Base;
using System.Linq;

namespace TwinFinder.Matching.StringFuzzyCompare.Aggregators
{


    public class MinimumAggregator : Aggregator
    {
        public override float AggregatedSimilarity(float[] similarities, float[] weights = null)
        {
            return similarities.Min();
        }
    }
}
#nullable enable