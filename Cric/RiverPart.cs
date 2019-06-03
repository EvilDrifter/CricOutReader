using System;
using System.Collections.Generic;
using System.Linq;

namespace Cric
{
    class RiverPart
    {
        public string Name;
        public List<Interval> Intervals;

        public RiverPart(string name)
        {
            Name = name;
            Intervals = new List<Interval>();
        }

        public Probability GetProbability(IntervalInfo info, int key)
        {
            return GetProbability(new MonthDayDay(info.MonthId, info.StDay, info.FnDay), key);
        }

        public Probability GetProbability(MonthDayDay monthDayDay, int key)
        {
            var intervals = GetIntarvalsFor(monthDayDay);

            var probabilities = new List<ProbabilityItem>();
            foreach (var interval in intervals)
            {
                interval.IntervalValue.TryGetValue(key, out var val);
                probabilities.Add(new ProbabilityItem(interval.DateStart.Year, val));
            }

            var result = new Probability(monthDayDay, key);
            result.Probabilities = CountProbobilityValueInList(probabilities).ToList();

            return result;
        }

        public Probability GetProbability(MonthDay infoStart, MonthDay infoFinish, IEnumerable<int> years, int key, CountIdentity identity)
        {
            var intervals = GetIntarvalsFor(infoStart, infoFinish, years).OrderBy(x=>x.DateStart);
            var probabilities = new List<ProbabilityItem>();

            foreach (var year in years)
            {
                var yearData = GetYearData(intervals, infoStart, infoFinish, year);
                double top = 0;
                double down = 0;
                double max = - 10 ^6;
                double min = 10 ^ 6;
                double sum = 0;
                double periodResult = 0;
                foreach (var yearItem in yearData)
                {
                    var days = (yearItem.DateFinish.Day - yearItem.DateStart.Day) + 1;

                    top += (days * yearItem.IntervalValue[key]);
                    down += days;

                    if (yearItem.IntervalValue[key] > max)
                    {
                        max = yearItem.IntervalValue[key];
                    }
                    if (yearItem.IntervalValue[key] < min)
                    {
                        min = yearItem.IntervalValue[key];
                    }
                    sum += yearItem.IntervalValue[key];
                }
                if (yearData.Any())
                {
                    
                    if (identity == CountIdentity.Av)
                    {
                        periodResult = top / down;
                    }
                    else if (identity == CountIdentity.Sum)
                    {
                        periodResult = sum;
                    }
                    else if (identity == CountIdentity.Max)
                    {
                        periodResult = max;
                    }
                    else if (identity == CountIdentity.Min)
                    {
                        periodResult = min;
                    }

                    probabilities.Add(new ProbabilityItem(year, periodResult));
                }       
            }

            var result = new Probability(infoStart, infoFinish, key);
            result.Probabilities = CountProbobilityValueInList(probabilities).ToList();

            return result;
        }

        private IEnumerable<ProbabilityItem> CountProbobilityValueInList(IEnumerable<ProbabilityItem> probabilities)
        {
            var sorted = probabilities.OrderByDescending(x => x.Value).ToList();
            for (int i = 0; i < sorted.Count(); i++)
            {
                double t = i + 1;
                double d = sorted.Count() + 2;
                sorted[i].ProbabilityValue = t * 100 / d;
            }

            return sorted;
        }


        private List<Interval> GetYearData(IEnumerable<Interval> intervals, MonthDay infoStart, MonthDay infoFinish, int year)
        {
            var targetYear = year;
            if (infoStart.Month > infoFinish.Month)
            {
                targetYear = year + 1;
            }

            return intervals.Where(x =>
                x.DateStart >= new DateTime(year, infoStart.Month, infoStart.Day) && 
                x.DateFinish <= new DateTime(targetYear, infoFinish.Month, infoFinish.Day))
            .ToList();
        }

        public List<Interval> GetIntarvalsFor(MonthDay infoStart, MonthDay infoFinish, IEnumerable<int> years)
        {
            if (infoStart.Month > infoFinish.Month)
            {
                return GetIntervalsForOutside(infoStart, infoFinish, years);
            }
            else
            {
                return GetIntervalsForInside(infoStart, infoFinish, years);
            }
        }

        private List<Interval> GetIntervalsForOutside(MonthDay infoStart, MonthDay infoFinish, IEnumerable<int> years)
        {
            return GetIntervalsForInside(infoStart, new MonthDay(12, 31), years)
                .Concat(GetIntervalsForInside(new MonthDay(1, 1), infoFinish, years))
                .ToList();
        }

        private List<Interval> GetIntervalsForInside(MonthDay infoStart, MonthDay infoFinish, IEnumerable<int> years)
        {
            var resultInterval = new List<Interval>();

            foreach (var year in years)
            {
                //var temp = Intervals.Where(x =>
                //    x.DateStart.Year >= year &&
                //    x.DateStart.Month >= infoStart.Month &&
                //    x.DateStart.Day >= infoStart.Day &&
                //    x.DateFinish.Year <= year &&
                //    x.DateFinish.Month <= infoFinish.Month &&
                //    x.DateFinish.Day <= infoFinish.Day);

                var temp = Intervals.Where(x => 
                    x.DateStart >= new DateTime(year, infoStart.Month, infoStart.Day) && 
                    x.DateFinish <= new DateTime(year, infoFinish.Month, infoFinish.Day));

                if (temp.Any())
                {
                    resultInterval.AddRange(temp);
                }
            }

            //var restrictBottom = Intervals.Where(x =>
            //    x.DateStart.Month >= infoStart.Month &&
            //    x.DateStart.Day >= infoStart.Day)
            //.ToDictionary(p => p.DateStart);

            //var restrictUp = Intervals.Where(x =>
            //    x.DateFinish.Month <= infoFinish.Month &&
            //    x.DateFinish.Day <= infoFinish.Day)
            //.ToDictionary(p => p.DateStart);

            //var restrictBottomKeys = restrictBottom.Keys.ToList();
            //var restrictUpKeys = restrictUp.Keys.ToList();
            //var allKeys = restrictBottomKeys.Union(restrictUpKeys).Distinct();

            //foreach (var idx in allKeys)
            //{
            //    restrictBottom.TryGetValue(idx, out var bottom);
            //    restrictUp.TryGetValue(idx, out var up);
            //    if (bottom != null && up != null)
            //    {
            //        resultInterval.Add(bottom);
            //    }
            //}
            return resultInterval;


            //var unionList = restrictBottom.Union(restrictUp).ToList();

            //return restrictBottom.Union(restrictUp).Distinct().ToList();

            //var intervals = Intervals.Where(x =>
            //    x.DateStart.Month >= infoStart.Month &&
            //    x.DateStart.Day >= infoStart.Day &&
            //    x.DateFinish.Month <= infoFinish.Month &&
            //    x.DateFinish.Day <= infoFinish.Day)
            //.ToList();

            //return intervals;
        }

        private List<Interval> GetIntarvalsFor(MonthDayDay monthDayDay)
        {
            var result = Intervals.Where(x =>
                x.DateStart.Month == monthDayDay.Month &&
                x.DateStart.Day == monthDayDay.DayStart);

            if (monthDayDay.Month != 2)
            {
                result = result.Where(x => x.DateFinish.Day == monthDayDay.DayFinish);
            }
            else 
            {
                result = result.Where(x => x.DateFinish.Day == 28 || x.DateFinish.Day == 29);
            }

            return result.ToList();
        }
    }
}
