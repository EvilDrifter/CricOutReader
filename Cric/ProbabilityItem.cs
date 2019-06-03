namespace Cric
{
    public class ProbabilityItem
    {
        public double ProbabilityValue;
        public int Year;
        public double Value;

        public ProbabilityItem(int year, double val)
        {
            Year = year;
            Value = val;
        }
    }
}