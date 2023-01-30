using System.Diagnostics;

namespace KleinLibrary.DebugHelper
{
    public static class LadeZeitenMesser
    {
        public static Stopwatch Start()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        /// <summary>
        /// Stoppt eine Stoppwatch und liefert die benötigte Zeit in Millisekunden ab.
        /// </summary>
        /// <param name="watch"></param>
        /// <param name="aktionname"></param>
        /// <returns></returns>
        public static long Stopp(Stopwatch watch, string aktionname)
        {
            if (aktionname == null)
            {
                throw new ArgumentNullException($"{nameof(aktionname)} darf nicht null sein");
            }

            if (watch == null)
            {
                return 0;
            }

            if (!watch.IsRunning)
            {
                return 0;
            }

            watch.Stop();

            return watch.ElapsedMilliseconds;
        }
    }
}
