namespace KleinLibrary.Core
{
    public class HolidayDays
    {
        private readonly List<Holiday> _holidays = new List<Holiday>();

        public HolidayDays()
        {
            Initialize();
        }

        public string GetHoliday(DateTime date, State state)
        {
            foreach (Holiday holiday in _holidays)
            {
                if (date.ToShortDateString().Equals(holiday.GetDate(GetEasterSunday(date.Year)).ToShortDateString()))
                {
                    if (holiday.States.Any(entry => state == entry))
                    {
                        return holiday.HolidayName;
                    }
                }
            }

            return string.Empty;
        }

        public bool IsHoliday(DateTime date, State state)
        {
            return GetHoliday(date, state).Length > 0;
        }

        private void Initialize()
        {
            State[] all = { State.BadenWürtenberg, State.Bayern, State.Berlin, State.Brandenburg, State.Bremen, State.Hamburg, State.Hessen, State.MecklenburgVorpommern, State.Niedersachsen, State.NordrheinWestfalen, State.RheinlandPfalz, State.Saarland, State.Sachsen, State.SachsenAnhalt, State.SchleswigHolstein, State.Thüringen };

            _holidays.Add(new Holiday("Neujahr", "01.01", all));
            _holidays.Add(new Holiday("Heiligen Drei Könige", "06.01", new[] { State.BadenWürtenberg, State.Bayern, State.SachsenAnhalt }));
            _holidays.Add(new Holiday("Karfreitag", -2, all));
            _holidays.Add(new Holiday("Ostersonntag", 0, all));
            _holidays.Add(new Holiday("Ostermontag", 1, all));
            _holidays.Add(new Holiday("Tag der Arbeit", "01.05", all));
            _holidays.Add(new Holiday("Christi Himmelfahrt", 39, all));
            _holidays.Add(new Holiday("Pfingstsonntag", 49, all));
            _holidays.Add(new Holiday("Pfingstmontag", 50, all));
            _holidays.Add(new Holiday("Fronleichnam", 60, new[] { State.BadenWürtenberg, State.Bayern, State.Hessen, State.NordrheinWestfalen, State.RheinlandPfalz, State.Saarland }));
            _holidays.Add(new Holiday("Maria Himmelfahrt", "15.08", new[] { State.Saarland }));
            _holidays.Add(new Holiday("Tag der dt. Einheit", "03.10", all));
            _holidays.Add(new Holiday("Allerheiligen", "01.11", new[] { State.BadenWürtenberg, State.Bayern, State.NordrheinWestfalen, State.RheinlandPfalz, State.Saarland }));
            _holidays.Add(new Holiday("1. Weinachtstag", "25.12", all));
            _holidays.Add(new Holiday("2. Weinachtstag", "26.12", all));
        }

        private static DateTime GetEasterSunday(int year)
        {
            int c = year / 100;
            int n = year - 19 * (year / 19);
            int k = (c - 17) / 25;
            int i = c - c / 4 - (c - k) / 3 + 19 * n + 15;
            i = i - 30 * (i / 30);
            i = i - i / 28 * (1 - i / 28) * (29 / (i + 1)) * ((21 - n) / 11);
            int j = year + year / 4 + i + 2 - c + c / 4;
            j = j - 7 * (j / 7);
            int l = i - j;

            int easterMonth = 3 + (l + 40) / 44;
            int easterDay = l + 28 - 31 * (easterMonth / 4);

            return Convert.ToDateTime(string.Format("{0}.{1}.{2}", easterDay, easterMonth, year));
        }
    }

    internal class Holiday
    {
        private readonly HolidayKind _holidayKind;
        private readonly int _addDays;
        private readonly string? _fixedDate;

        public string HolidayName { get; private set; }
        public DateTime Date { get; private set; }
        public State[] States { get; private set; }

        internal Holiday(string holidayName, string fixedDate, State[] states)
        {
            _addDays = 0;
            _holidayKind = HolidayKind.Fixed;
            _fixedDate = fixedDate;

            HolidayName = holidayName;
            States = states;
        }

        internal Holiday(string holidayName, int addDays, State[] states)
        {
            _addDays = addDays;
            _holidayKind = HolidayKind.Dynamic;

            HolidayName = holidayName;
            States = states;
        }

        public DateTime GetDate(DateTime easterSunday)
        {
            Date = _holidayKind != HolidayKind.Fixed ? easterSunday.AddDays(_addDays) : DateTime.Parse($"{_fixedDate}.{easterSunday.Year}");
            return DateTime.Parse(string.Format("{0:dd.MM.yyyy}", Date));
        }
    }

    public enum State
    {
        BadenWürtenberg,
        Bayern,
        Berlin,
        Brandenburg,
        Bremen,
        Hamburg,
        Hessen,
        MecklenburgVorpommern,
        Niedersachsen,
        NordrheinWestfalen,
        RheinlandPfalz,
        Saarland,
        Sachsen,
        SachsenAnhalt,
        SchleswigHolstein,
        Thüringen
    }

    public enum HolidayKind
    {
        Fixed,
        Dynamic
    }
}
