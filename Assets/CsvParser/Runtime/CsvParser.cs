using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GigaCreation.Tools.Csv2Collections
{
    public static class CsvParser
    {
        public static List<List<string>> Parse(string csv)
        {
            var reader = new StringReader(csv);
            var result = new List<List<string>>();

            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();

                // `line` is null if the end of the string is reached.
                if (line == null)
                {
                    break;
                }

                result.Add(SplitLine(line));
            }

            return result;
        }

        public static List<Dictionary<string, string>> ParseIntoDictionaries(string csv)
        {
            List<List<string>> table = Parse(csv);

            if ((table.Count == 0) || table[0].Any(string.IsNullOrEmpty))
            {
                Debug.LogWarning("Some or all of the headers are empty.");
                return null;
            }

            if (table[0].GroupBy(header => header).Any(grouping => grouping.Count() > 1))
            {
                Debug.LogWarning("Duplicate header found.");
                return null;
            }

            var result = new List<Dictionary<string, string>>(table.Count - 1);

            for (var rowIndex = 1; rowIndex < table.Count; rowIndex++)
            {
                var dict = new Dictionary<string, string>(table[0].Count);

                for (var columnIndex = 0; columnIndex < table[0].Count; columnIndex++)
                {
                    dict.Add(table[0][columnIndex], table[rowIndex][columnIndex]);
                }

                result.Add(dict);
            }

            return result;
        }

        private static List<string> SplitLine(string line)
        {
            var columns = new List<string>();
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
