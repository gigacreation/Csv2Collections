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
        public static IList<string> ExtractIntoList(CsvExtractRequest request)
        {
            var reader = new StringReader(request.Csv);
            string headerLine = request.HasHeader ? reader.ReadLine() : null;

            IList<int> valueIndexes
                = DetermineTargetIndexes(request.ValueColumnIndexes, request.ValueColumnHeaders, headerLine);

            IList<string> result = new List<string>();

            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    continue;
                }

                IList<string> splitLine = SplitLine(line);

                result.Add(string.Join(
                    request.ValueSeparator,
                    valueIndexes.Select(index => splitLine.Count > index ? splitLine[index] : "")
                ));
            }

            return result;
        }

        /// <summary>
        /// Returns a dictionary with keys and values extracted from a CSV text.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The generated dictionary.</returns>
        public static IDictionary<string, string> ExtractIntoDictionary(CsvExtractRequest request)
        {
            var reader = new StringReader(request.Csv);
            string headerLine = request.HasHeader ? reader.ReadLine() : null;

            IList<int> keyIndexes
                = DetermineTargetIndexes(request.KeyColumnIndexes, request.KeyColumnHeaders, headerLine);

            IList<int> valueIndexes
                = DetermineTargetIndexes(request.ValueColumnIndexes, request.ValueColumnHeaders, headerLine);

            IDictionary<string, string> result = new Dictionary<string, string>();

            if (keyIndexes.Count == 0)
            {
                Debug.LogWarning("No key column specified.");
                return result;
            }

            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    continue;
                }

                IList<string> splitLine = SplitLine(line);

                string key = string.Join(
                    request.KeySeparator,
                    keyIndexes.Select(index => splitLine.Count > index ? splitLine[index] : "")
                );

                string value = string.Join(
                    request.ValueSeparator,
                    valueIndexes.Select(index => splitLine.Count > index ? splitLine[index] : "")
                );

                if (!result.TryAdd(key, value))
                {
                    Debug.LogWarning($"Duplicate key found: {key}");
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a multi-list with elements extracted from a CSV text.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The generated list.</returns>
        public static IList<IList<string>> ExtractIntoMultiList(CsvExtractRequest request)
        {
            var reader = new StringReader(request.Csv);
            string headerLine = request.HasHeader ? reader.ReadLine() : null;

            IList<int> valueIndexes
                = DetermineTargetIndexes(request.ValueColumnIndexes, request.ValueColumnHeaders, headerLine);

            IList<IList<string>> result = new List<IList<string>>();

            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    continue;
                }

                IList<string> splitLine = SplitLine(line);

                result.Add(valueIndexes.Count > 0
                    ? valueIndexes
                        .Select(targetIndex => splitLine.Count > targetIndex ? splitLine[targetIndex] : "")
                        .ToList()
                    : splitLine);
            }

            return result;
        }

        /// <summary>
        /// Returns a multi-dictionary with keys and values extracted from a CSV text.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The generated dictionary.</returns>
        public static IDictionary<string, IList<string>> ExtractIntoMultiDictionary(CsvExtractRequest request)
        {
            var reader = new StringReader(request.Csv);
            string headerLine = request.HasHeader ? reader.ReadLine() : null;

            IList<int> keyIndexes
                = DetermineTargetIndexes(request.KeyColumnIndexes, request.KeyColumnHeaders, headerLine);

            IList<int> valueIndexes
                = DetermineTargetIndexes(request.ValueColumnIndexes, request.ValueColumnHeaders, headerLine);

            IDictionary<string, IList<string>> result = new Dictionary<string, IList<string>>();

            if (keyIndexes.Count == 0)
            {
                Debug.LogWarning("No key column specified.");
                return result;
            }

            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    continue;
                }

                IList<string> splitLine = SplitLine(line);

                string key = string.Join(
                    request.KeySeparator,
                    keyIndexes.Select(index => splitLine.Count > index ? splitLine[index] : "")
                );

                IList<string> value = valueIndexes.Count > 0
                    ? valueIndexes
                        .Select(targetIndex => splitLine.Count > targetIndex ? splitLine[targetIndex] : "")
                        .ToList()
                    : splitLine;

                if (!result.TryAdd(key, value))
                {
                    Debug.LogWarning($"Duplicate key found: {key}");
                }
            }

            return result;
        }

        private static IList<int> DetermineTargetIndexes(
            IReadOnlyCollection<int> targetIndexes, IReadOnlyCollection<string> targetHeaders, string headerLine
        )
        {
            if (targetIndexes?.Count > 0)
            {
                return targetIndexes.ToList();
            }

            if (targetHeaders?.Count > 0)
            {
                IList<int> result = new List<int>();

                if (string.IsNullOrEmpty(headerLine))
                {
                    Debug.LogWarning("The header line doesn't exist in the csv.");
                    return result;
                }

                IList<string> headersInCsv = SplitLine(headerLine);

                foreach (string targetHeader in targetHeaders)
                {
                    int headerIndex = headersInCsv.IndexOf(targetHeader);

                    if (headerIndex == -1)
                    {
                        Debug.LogWarning($"The target header doesn't exist in the csv: {targetHeader}");
                        continue;
                    }

                    result.Add(headerIndex);
                }

                return result;
            }

            return new List<int>();
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
