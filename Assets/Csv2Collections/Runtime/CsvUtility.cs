using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GigaCreation.Tools.Csv2Collections
{
    public static class CsvUtility
    {
        /// <summary>
        /// Returns a list with elements extracted from a CSV text.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The generated list.</returns>
        public static List<string> ExtractIntoList(CsvExtractRequest request)
        {
            if (!(request.ValueColumnIndexes?.Length > 0))
            {
                Debug.LogError("The index of the column to be extracted is not specified.");
                return null;
            }

            var result = new List<string>();

            Extract(request.Csv, request.HasHeader, request.GetMaxTargetColumnIndex(), splitLine =>
            {
                result.Add(string.Join(
                    request.ValueSeparator,
                    request.ValueColumnIndexes.Select(index => splitLine[index])
                ));
            });

            return result;
        }

        /// <summary>
        /// Returns a dictionary with keys and values extracted from a CSV text.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The generated dictionary.</returns>
        public static Dictionary<string, string> ExtractIntoDictionary(CsvExtractRequest request)
        {
            if (!(request.KeyColumnIndexes?.Length > 0) || !(request.ValueColumnIndexes?.Length > 0))
            {
                Debug.LogError("The index of the column to be extracted is not specified.");
                return null;
            }

            var result = new Dictionary<string, string>();

            Extract(request.Csv, request.HasHeader, request.GetMaxTargetColumnIndex(), splitLine =>
            {
                result.Add(
                    string.Join(request.KeySeparator, request.KeyColumnIndexes.Select(index => splitLine[index])),
                    string.Join(request.ValueSeparator, request.ValueColumnIndexes.Select(index => splitLine[index]))
                );
            });

            return result;
        }

        private static void Extract(
            string csvText, bool hasHeader, int maxTargetColumnIndex, Action<IList<string>> join
        )
        {
            var reader = new StringReader(csvText);
            var columns = new List<string>();
            var builder = new StringBuilder();

            if (hasHeader)
            {
                // Skip the header.
                reader.ReadLine();
            }

            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                columns.Clear();
                builder.Clear();
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

                if (maxTargetColumnIndex >= columns.Count)
                {
                    Debug.LogError("The index of the column to be extracted is out of range.");
                    return;
                }

                join(columns);
            }
        }
    }
}
