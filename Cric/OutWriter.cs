using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cric
{
    class OutWriter
    {
        public void WriteFile(Dictionary<int, List<Probability>> multiData, string riverName)
        {
            var b = 0;
            using (var p = new ExcelPackage())
            {

                foreach (var md in multiData)
                {
                    var ws = p.Workbook.Worksheets.Add(md.Key.ToString());
                    FillWorkSheet(ws, md.Value);
                    b++;
                }

                var name = riverName.Replace(':', '_');

                p.SaveAs(new FileInfo($"E:/out_{name}.xlsx"));
            }
        }

        private void FillWorkSheet(ExcelWorksheet workSheet, List<Probability> data)
        {
            int col, row;
            int delta = 0;

            col = 1;
            foreach (var curData in data)
            {
                row = 2;

                workSheet.Cells[row, col].Value = curData.GetIntervalName();
                for (int i = 0; i < curData.Probabilities.Count(); i++)
                {
                    if (row == 2 && col == 1)
                    {
                        workSheet.Cells[row-1, col].Value = "P, %";
                    }
                    
                    if (col == 1)
                    {
                        workSheet.Cells[row, col].Value = curData.Probabilities[i].ProbabilityValue;
                        delta = 1;
                    }
                    else
                    {
                        delta = 0;
                    }

                    if (row == 2)
                    {
                        workSheet.Cells[row - 1, col + delta].Value = curData.GetIntervalName();
                    }

                    workSheet.Cells[row, col + delta].Value = curData.Probabilities[i].Year;
                    workSheet.Cells[row, col + delta + 1].Value = curData.Probabilities[i].Value;
                

                    row++;
                }
                col = col + 2 + delta;
            }
        }
    }
}
