namespace Cric
{
    internal class IntervalInfo
    {
        public int MonthId;
        public int StDay;
        public int FnDay;
        public IntervalInfo(int month, int st, int fn)
        {
            MonthId = month;
            StDay = st;
            FnDay = fn;
        }

        public string GetStringValue()
        {
            return $"{MonthToNum.GetMonthNameById(MonthId)} {StDay}-{FnDay}";
        }
    }
}