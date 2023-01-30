namespace KleinLibrary.Erweiterungen
{
    public static class MathExtensions
    {

        public static decimal BerechneProzent(decimal neuerWert, decimal alterWert)
        {
            return Math.Round((1 - (neuerWert / alterWert)) * 100, 2);
        }

        public static decimal BerechnePreis(decimal preis, decimal rabatt)
        {
            return Math.Round(preis * (1 - (rabatt / 100)), 2);
        }

        public static int GetDecimalPlaces(decimal n)
        {
            n = Math.Abs(n); // make sure it is positive.
            n -= (int)n;     // remove the integer part of the number.
            var decimalPlaces = 0;
            while (n > 0)
            {
                decimalPlaces++;
                n *= 10;
                n -= (int)n;
            }
            return decimalPlaces;
        }
    }
}
