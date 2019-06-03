using System;
using System.Collections.Generic;

namespace Cric
{
    class Interval
    {
        public DateTime DateStart;
        public DateTime DateFinish;
        public Dictionary<int, double> IntervalValue;

        public Interval(DateTime dateStart, DateTime dateFinish)
        {
            DateStart = dateStart;
            DateFinish = dateFinish;
            IntervalValue = new Dictionary<int, double>();
        }
    }
}
