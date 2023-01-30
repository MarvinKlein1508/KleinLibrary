#nullable disable
using TwinFinder.Matching.StringFuzzyCompare.Aggregators.Base;
using System.Linq;

namespace TwinFinder.Matching.StringFuzzyCompare.Aggregators
{

    public class MaximumAggregator : Aggregator
    {
        public override float AggregatedSimilarity(float[] similarities, float[] weights = null)
        {
            return similarities.Max();
        }
    }
}
#nullable enable