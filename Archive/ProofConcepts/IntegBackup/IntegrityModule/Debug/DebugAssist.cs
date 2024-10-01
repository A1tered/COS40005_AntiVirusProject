using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.Debug
{
    public static class DebugAssist
    {
        public static string StringListToStringDisplay(List<List<string>> tableRows, int rowSizeLimit = 50)
        {
            if (tableRows.Count > 1)
            {
                bool frontSnip = true;
                List<int> highestSizeColumn = new();
                int widthColumns = tableRows[0].Count();
                int tempLength = 0;
                int rowTracker = 0;
                StringBuilder displayOutput = new();
                StringBuilder columnAdder = new();
                if (tableRows[0].Count() == tableRows[1].Count())
                {
                    for (int i = 0; i < widthColumns; i++)
                    {
                        // Base value is size of field length for that column.
                        highestSizeColumn.Add(tableRows[0][i].Length);
                    }
                    // Remove field names
                    foreach (List<string> columns in tableRows)
                    {
                        for (int columnIndex = 0; columnIndex < widthColumns; columnIndex++)
                        {
                            tempLength = columns[columnIndex].Length;
                            if (tempLength > highestSizeColumn[columnIndex])
                            {
                                if (tempLength < rowSizeLimit)
                                {
                                    highestSizeColumn[columnIndex] = tempLength;
                                }
                                else
                                {
                                    highestSizeColumn[columnIndex] = rowSizeLimit;
                                }

                            }
                        }
                    }
                    // Prepare creation of output given available information.
                    foreach (List<string> column in tableRows)
                    {
                        for (int columnIndex = 0; columnIndex < widthColumns; columnIndex++)
                        {
                            columnAdder.Clear();
                            columnAdder.Append(column[columnIndex]);
                            if (columnAdder.Length > rowSizeLimit)
                            {
                                if (frontSnip)
                                {
                                    columnAdder.Remove(0, columnAdder.Length - rowSizeLimit + 3);
                                    columnAdder = new StringBuilder("..." + columnAdder.ToString());
                                }
                                else
                                {
                                    columnAdder.Remove(rowSizeLimit - 3, (columnAdder.Length - rowSizeLimit + 3));
                                    columnAdder.Append("...");
                                }
                            }
                            while (columnAdder.Length < highestSizeColumn[columnIndex])
                            {
                                // Space it out for consistency
                                columnAdder.Append(" ");
                            }
                            if (columnIndex != widthColumns - 1)
                            {
                                columnAdder.Append("|");
                            }
                            displayOutput.Append(columnAdder);
                        }
                        displayOutput.Append("\n");
                        if (rowTracker == 0)
                        {
                            int sizeTotal = highestSizeColumn.Sum();
                            for (int upper = 0; upper < sizeTotal; upper++)
                            {
                                displayOutput.Append("_");
                            }
                            displayOutput.Append("\n");
                        }
                        rowTracker++;
                    }
                    return displayOutput.ToString();
                }
                else
                {
                    return "Failure: Column Names do not match column value sizes";
                }
            }
            return "Failure: Could not produce display (no rows!)";
        }
    }
}
