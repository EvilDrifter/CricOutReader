namespace Cric
{
    internal class MonthDayDay
    {
        public int Month { get; private set; }
        public int DayStart { get; private set; }
        public int DayFinish { get; private set; }

        public MonthDayDay(int monthId, int dayIdStart, int dayIdFinish)
        {
            Month = monthId;
            DayStart = dayIdStart;
            DayFinish = dayIdFinish;
        }
    }
}