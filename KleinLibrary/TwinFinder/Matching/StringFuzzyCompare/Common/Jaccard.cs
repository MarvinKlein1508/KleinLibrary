﻿#nullable disable
using System.Linq;
using TwinFinder.Matching.StringFuzzyCompare.Base;
using TwinFinder.Matching.StringTokenize;
using TwinFinder.Matching.StringTokenize.Base;

namespace TwinFinder.Matching.StringFuzzyCompare.Common
{
    /// <summary>
    /// Build the token based Jaccard coefficient, which is defined as the
    /// intersection divided by the size of the union of the tokens.
    /// Tokens can be build by NGrams
    ///
    /// ( |X & Y| ) / ( | X or Y | )
    ///   intersection / union
    /// </summary>
    public class Jaccard : StringFuzzyComparer
    {
        // ***********************Fields***********************

        private int nGramLength = 2;

        private StringTokenizer tokenizer = new NGramTokenizer();

        // ***********************Functions***********************

        public override float Compare(string str1, string str2)
        {
            if (str1 == null || str2 == null)
            {
                return 0;
            }

            if (!this.CaseSensitiv)
            {
                str1 = str1.ToLower();
                str2 = str2.ToLower();
            }

            if (str1.Equals(str2))
            {
                return 1.0f;
            }

            // building NGram of length=2
            this.tokenizer.MaxLength = this.nGramLength;

            // tokenize into NGrams e.g "Müller" -> #M Mü ül ll le er r#
            string[] nGrams1 = this.tokenizer.Tokenize(str1);

            // tokenize into NGrams e.g "Meier" -> #M Me ei ie er r#
            string[] nGrams2 = this.tokenizer.Tokenize(str2);

            float score = this.BuildJaccard(nGrams1, nGrams2);

            return this.NormalizeScore(str1, str2, score);
        }

        private float BuildJaccard(string[] nGrams1, string[] nGrams2)
        {
            // all distinct tokens together  -> #M Mü ül ll le er r# Me ei ie
            float unionNGrams = nGrams1.Union(nGrams2).Count();

            // inner set of tokens -> #M r#
            float matchingNGrams = nGrams1.Where(x => nGrams2.Contains(x)).Count();

            if (matchingNGrams == 0)
            {
                return 0.0f;
            }

            // ( |X & Y| ) / ( | X or Y | )
            // innerer Schnitt / Vereinigungsmenge
            return matchingNGrams / unionNGrams;
        }

        private float NormalizeScore(string str1, string str2, float score)
        {
            // no normalization is needed it has already the range 0-1
            return score;
        }
    }
}
#nullable enable