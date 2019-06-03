using System.Collections.Generic;

namespace Cric
{
    class Probability
    {
        public MonthDayDay monthDayDay { get; private set; }
        public MonthDay monthDayStart { get; private set; }
        public MonthDay monthDayFinish { get; private set; }
        public int ItemId { get; private set; }

        public List<ProbabilityItem> Probabilities;

        public Probability(MonthDayDay _monthDayDay, int key)
        {
            monthDayDay = _monthDayDay;
            Init(key);
        }

        public Probability(MonthDay _monthDayStart, MonthDay _monthDayFinish, int key)
        {
            monthDayStart = _monthDayStart;
            monthDayFinish = _monthDayFinish;
            Init(key);
        }

        private void Init(int key)
        {
            ItemId = key;
            Probabilities = new List<ProbabilityItem>();
        }

        public string GetIntervalName()
        {
            if (monthDayDay!=null)
                return $"{MonthToNum.GetMonthNameById(monthDayDay.Month)} {monthDayDay.DayStart}-{monthDayDay.DayFinish}";

            if (monthDayStart!=null && monthDayFinish != null)
                return $"{MonthToNum.GetMonthNameById(monthDayStart.Month)}.{monthDayStart.Day}-{MonthToNum.GetMonthNameById(monthDayFinish.Month)}.{monthDayFinish.Day}";

            return string.Empty;
        }
    }
}
