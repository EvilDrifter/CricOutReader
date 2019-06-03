using System.Collections.Generic;
using System.Linq;

namespace Cric
{
    internal static class MonthToNum
    {
        static Dictionary<string, int> months = new Dictionary<string, int>
        {
            {"Январь", 1 },
            {"Февраль", 2 },
            {"Март", 3 },
            {"Апрель", 4 },
            {"Май", 5 },
            {"Июнь", 6 },
            {"Июль", 7 },
            {"Август", 8 },
            {"Сентябрь", 9 },
            {"Октябрь", 10 },
            {"Ноябрь", 11 },
            {"Декабрь", 12 },
        };

        internal static int GetMonthId(string monthName)
        {
            months.TryGetValue(monthName, out var id);
            return id > 0 ? id : 0;
        }

        internal static string GetMonthNameById(int monthId)
        {
            return months.FirstOrDefault(x => x.Value == monthId).Key;
        }
    }
}
