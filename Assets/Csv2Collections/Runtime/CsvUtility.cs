using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                result.Add(string.Join(request.Separator, request.ValueColumnIndexes.Select(idx => splitLine[idx])));
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
                    string.Join(request.Separator, request.KeyColumnIndexes.Select(idx => splitLine[idx])),
                    string.Join(request.Separator, request.ValueColumnIndexes.Select(idx => splitLine[idx]))
                );
            });

            return result;
        }

        private static void Extract(string csvText, bool hasHeader, int maxTargetColumnIndex, Action<string[]> action)
        {
            var reader = new StringReader(csvText);

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

                string[] splitLine = line.Split(',');

                if (maxTargetColumnIndex >= splitLine.Length)
                {
                    continue;
                }

                action(splitLine);
            }
        }
    }
}
