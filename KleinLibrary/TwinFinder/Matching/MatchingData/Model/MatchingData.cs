#nullable disable
using System.Collections.Generic;
using TwinFinder.Matching.Compare;
using TwinFinder.Matching.Key;
namespace TwinFinder.Matching.MatchingData.Model
{


    public class MatchingData
    {
        // ***********************Fields***********************

        private List<KeyDefinition> keyDefinitions = new List<KeyDefinition>();
        private List<CompareDefinition> compareDefinitions = new List<CompareDefinition>();

        // ***********************Properties***********************

        public List<KeyDefinition> KeyDefinitions
        {
            get { return this.keyDefinitions; }
            set { this.keyDefinitions = value; }
        }

        public List<CompareDefinition> CompareDefinitions
        {
            get { return this.compareDefinitions; }
            set { this.compareDefinitions = value; }
        }

    }
}
#nullable enable