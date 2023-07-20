using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GigaCreation.Tools.Csv2Collections
{
    public static class CsvParser
    {
        public static string[,] Parse(string csv)
        {
            var reader = new StringReader(csv);

            IList<IList<string>> nestedList = new List<IList<string>>();

            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();

                // `line` is null if the end of the string is reached.
                if (line == null)
                {
                    // TODO: 空行の時に break されないかどうかチェックする
                    break;
                }

                nestedList.Add(SplitLine(line));
            }

            int numOfRows = nestedList.Count;
            int numOfColumns = nestedList.Max(row => row.Count);

            var result = new string[numOfRows, numOfColumns];

            for (var i = 0; i < numOfRows; i++)
            {
                for (var j = 0; j < numOfColumns; j++)
                {
                    result[i, j] = nestedList[i].Count > j ? nestedList[i][j] : "";
                }
            }

            return result;
        }

        public static Dictionary<string, string>[] ParseIntoDictionaries(string csv)
        {
            string[,] table = Parse(csv);

            int numOfRows = table.GetLength(0);
            int numOfColumns = table.GetLength(1);

            if (numOfRows == 0)
            {
                Debug.LogError("The csv is empty.");
                return null;
            }

            return null;
        }

        private static IList<string> SplitLine(string line)
        {
            IList<string> columns = new List<string>();
            var builder = new StringBuilder(line.Length);
            var isInLiteral = false;

            for (var i = 0; i < line.Length; i++)
            {
                if ((line[i] == ',') && !isInLiteral)
                {
                    columns.Add(builder.ToString());
                    builder.Clear();

                    continue;
                }

                if (line[i] == '\"')
                {
                    if ((i < line.Length - 1) && (line[i + 1] == '\"'))
                    {
                        builder.Append('\"');
                        i++;
                    }
                    else
                    {
                        isInLiteral = !isInLiteral;
                    }

                    continue;
                }

                builder.Append(line[i]);
            }

            columns.Add(builder.ToString());

            return columns;
        }
    }
}
