using KleinLibrary.Core;

namespace KleinLibrary.Erweiterungen
{
    public static class DateTimeErweiterungen
    {
        public static string RelativeZeit(DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;

            var ts = new TimeSpan(DateTime.Now.Ticks - yourDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            return delta switch
            {
                < 1 * MINUTE => ts.Seconds == 1 ? "vor einer Sekunde" : $"vor {ts.Seconds} Sekunden",
                < 2 * MINUTE => "vor einer Minute",
                < 45 * MINUTE => $"vor {ts.Minutes} Minuten",
                < 120 * MINUTE => "vor einer Stunde",
                < 24 * HOUR => $"vor {ts.Hours} Stunden",
                < 48 * HOUR => "gestern",
                _ => DateTime.Now.ToString("dd. MMMM yyyy"),
            };
        }


        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek endOfWeek)
        {
            return StartOfWeek(dt, (DayOfWeek)(((int)endOfWeek + 1) % 7)).AddDays(6).Date;
        }

        public static DateTime NächstesDatumWochentag(DateTime datum, bool descending)
        {
            DateTime tmpDatum = datum;
            HolidayDays holiday = new HolidayDays();

            while (holiday.IsHoliday(tmpDatum, State.NordrheinWestfalen) || tmpDatum.DayOfWeek is DayOfWeek.Saturday || tmpDatum.DayOfWeek is DayOfWeek.Sunday)
            {
                tmpDatum = tmpDatum.AddDays(descending ? -1 : 1);
            }

            return tmpDatum;
        }

        public static (DateTime von, DateTime bis) GetZeitraumLetztenMonat()
        {
            DateTime letzterMonat = DateTime.Now.AddMonths(-1);
            DateTime start = new DateTime(letzterMonat.Year, letzterMonat.Month, 1);
            DateTime ende = new DateTime(letzterMonat.Year, letzterMonat.Month, DateTime.DaysInMonth(letzterMonat.Year, letzterMonat.Month));

            return (start, ende);
        }

        public static (DateTime von, DateTime bis) GetZeitraumAktuellerMonat()
        {
            DateTime aktuellerMonat = DateTime.Now;
            DateTime start = new DateTime(aktuellerMonat.Year, aktuellerMonat.Month, 1);
            DateTime ende = new DateTime(aktuellerMonat.Year, aktuellerMonat.Month, DateTime.DaysInMonth(aktuellerMonat.Year, aktuellerMonat.Month));

            return (start, ende);
        }
    }

}
