namespace Cric
{
    internal class MonthDay
    {
        public int Month { get; private set; }
        public int Day { get; private set; }

        public MonthDay(int monthId, int dayId)
        {
            Month = monthId;
            Day = dayId;
        }
    }
}