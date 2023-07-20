using System.Collections.Generic;
using System.IO;
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

            int numOfRows = table.Count;

            if (numOfRows == 0)
            {
                Debug.LogWarning("The csv is empty.");
                return null;
            }

            if (numOfRows == 1)
            {
                Debug.LogWarning("The csv has header only.");
                return null;
            }

            return null;
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
