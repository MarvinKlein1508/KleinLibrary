#nullable disable
namespace TwinFinder.Matching.StringFuzzyCompare.Aggregators.Base
{
    public interface IAggregator
    {
        float AggregatedSimilarity(float[] similarities, float[] weights);
    }
}
#nullable enable