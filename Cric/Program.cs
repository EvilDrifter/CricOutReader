using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cric
{
    class Program
    {
        static void Main(string[] args)
        {
            var outReader = new OutReader();

            //var MegaData = new Dictionary<int, List<Probability>>();
            var c = new OutWriter();
            foreach (var river in outReader.rivers)
            {
                var MegaData = new Dictionary<int, List<Probability>>();
                foreach (var key in outReader.itemsKey)
                {
                    MegaData.Add(key, new List<Probability>());
                    MegaData[key].AddRange(outReader.GetProbabilities(river.Name, key));
                    MegaData[key].AddRange(outReader.GetProbabilities(river.Name, new MonthDay(5, 1), new MonthDay(10, 20), key, CountIdentity.Av));
                    MegaData[key].AddRange(outReader.GetProbabilities(river.Name, new MonthDay(10, 21), new MonthDay(5, 1), key, CountIdentity.Av));
                    MegaData[key].AddRange(outReader.GetProbabilities(river.Name, new MonthDay(5, 1), new MonthDay(4, 30), key, CountIdentity.Av));
                    MegaData[key].AddRange(outReader.GetProbabilities(river.Name, new MonthDay(5, 1), new MonthDay(4, 30), key, CountIdentity.Max));
                    MegaData[key].AddRange(outReader.GetProbabilities(river.Name, new MonthDay(5, 1), new MonthDay(4, 30), key, CountIdentity.Min));
                    MegaData[key].AddRange(outReader.GetProbabilities(river.Name, new MonthDay(5, 11), new MonthDay(10, 10), key, CountIdentity.Av));
                    MegaData[key].AddRange(outReader.GetProbabilities(river.Name, new MonthDay(5, 1), new MonthDay(4, 30), key, CountIdentity.Sum));
                }

                c.WriteFile(MegaData, river.Name);
            }

            Console.WriteLine("No Hello World!");
        }
    }

    class OutReader
    {
        string[] outFile;
        List<IntervalInfo> intervals;
        List<int> years;
        public List<int> itemsKey;
        public List<RiverPart> rivers;

        public OutReader()
        {
            ReadOutFile(@"C:\dev\console\Cric\out.txt");

            GetFileKeys();
            GetFileIntervals();
            rivers = ReadFile();

            //var result = GetProbabilities("Саяно-Шушенс", 20);
        }

        public List<IntervalInfo> GetMonths()
        {
            return intervals;
        }

        public List<Probability> GetProbabilities(string riverName, int key)
        {
            List<Probability> result;
            var river = rivers.FirstOrDefault(x => x.Name == riverName);
            if (river == null) return null;

            result = new List<Probability>();

            foreach (var interval in intervals)
            {
                result.Add(river.GetProbability(interval, key));
            }

            return result;
        }

        public List<Probability> GetProbabilities(string riverName, MonthDay infoStart, MonthDay infoFinish, int key, CountIdentity identity)
        {
            List<Probability> result;
            var river = rivers.FirstOrDefault(x => x.Name == riverName);
            if (river == null) return null;

            result = new List<Probability>();
            result.Add(river.GetProbability(infoStart, infoFinish, years, key, identity));
            
            return result;
        }

        private void ReadOutFile(string filePath)
        {
            var enc1251 = CodePagesEncodingProvider.Instance.GetEncoding(1251);
            outFile = File.ReadAllLines(filePath, enc1251);
        }
        private int GetFileStart()
        {
            for (int i = 0; i < outFile.Length; i++)
            {
                var lineWithNoSpaces = outFile[i].Replace(" ", string.Empty);
                var items = lineWithNoSpaces.Split(';');
                if (items.FirstOrDefault() == "1")
                {
                    return i;
                }
            }

            return 0;
        }
        private void GetFileKeys()
        {
            itemsKey = new List<int>();

            var startPoint = GetFileStart();
            if (startPoint == 0) return;

            var lineWithNoSpaces = outFile[startPoint].Replace(" ", string.Empty);
            var items = lineWithNoSpaces.Split(';');

            for (int j = 5; j < items.Length; j++)
            {
                if (!string.IsNullOrEmpty(items[j]))
                {
                    itemsKey.Add(j + 1);
                }
            }
        }
        private void GetFileIntervals()
        {
            intervals = new List<IntervalInfo>();
            years = new List<int>();
            int st;
            int fn;

            var startPoint = GetFileStart() + 1;
            if (startPoint == 1) return;

            for (int i = startPoint; i < outFile.Length; i++)
            {
                var lineWithNoSpaces = outFile[i].Replace(" ", string.Empty);
                var items = lineWithNoSpaces.Split(';');

                if (int.TryParse(items[0], out int n))
                {
                    if (!years.Contains(n))
                    {
                        years.Add(n);
                    }

                    var month = MonthToNum.GetMonthId(items[1]);

                    st = Convert.ToInt32(items[2]);
                    fn = Convert.ToInt32(items[3]);
                    var interval = intervals.FirstOrDefault(x => x.MonthId == month && x.StDay == st);
                    if (interval == null)
                    {
                        intervals.Add(new IntervalInfo(month, st, fn));
                    }
                }
            }
        }
        private List<RiverPart> ReadFile()
        {
            var FullRiver = new List<RiverPart>();
            var startPoint = GetFileStart()+1;
            if (startPoint == 1) return null;

            for (int i = startPoint; i < outFile.Length; i++)
            {
                var lineWithNoSpaces = outFile[i].Replace(" ", string.Empty);
                var items = lineWithNoSpaces.Split(';');

                if (int.TryParse(items[0], out int n))
                {
                    var river = FullRiver.FirstOrDefault(x => x.Name == items[4]);
                    if (river == null)
                    {
                        river = new RiverPart(items[4]);
                        FullRiver.Add(river);
                    }

                    var month = MonthToNum.GetMonthId(items[1]);
                    var curDateStart = new DateTime(Convert.ToInt32(items[0]), month, Convert.ToInt32(items[2]));
                    var curDateFinish = new DateTime(Convert.ToInt32(items[0]), month, Convert.ToInt32(items[3]));

                    var curInterval = new Interval(curDateStart, curDateFinish);
                    for (int j = 5; j < items.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(items[j]))
                        {
                            curInterval.IntervalValue.Add(j+1, Convert.ToDouble(items[j]));
                        }
                    }

                    river.Intervals.Add(curInterval);
                }  
            }
            return FullRiver;
        }
    }
}
