#nullable disable
using TwinFinder.Matching.StringFuzzyCompare.Aggregators;
using TwinFinder.Matching.StringFuzzyCompare.Aggregators.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace TwinFinder.Matching.Compare
{


#if !SILVERLIGHT

    [Serializable]
#endif
    [DataContract]
    public class CompareDefinitionGroup
    {
        private List<CompareDefinition> compareDefinitions = new List<CompareDefinition>();

        private Aggregator aggregator = new MaximumAggregator();

        [DataMember]
        public List<CompareDefinition> CompareDefinitions
        {
            get { return this.compareDefinitions; }
            set { this.compareDefinitions = value; }
        }

        [DataMember]
        public Aggregator Aggregator
        {
            get
            {
                if (this.aggregator == null)
                {
                    this.aggregator = new MaximumAggregator();
                }

                return this.aggregator;
            }
            set { this.aggregator = value; }
        }
    }
}
#nullable enable