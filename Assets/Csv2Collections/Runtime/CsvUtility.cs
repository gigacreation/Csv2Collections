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

            if (valueIndexes == null)
            {
                Debug.LogError("The value column indexes or headers are not specified.");
                return null;
            }

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

            if (keyIndexes == null)
            {
                Debug.LogError("The key column indexes or headers are not specified.");
                return null;
            }

            if (valueIndexes == null)
            {
                Debug.LogError("The value column indexes or headers are not specified.");
                return null;
            }

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
        /// Returns a nested list with elements extracted from a CSV text.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The generated list.</returns>
        public static IList<IList<string>> ExtractIntoNestedList(CsvExtractRequest request)
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

                result.Add(valueIndexes
                    ?.Select(targetIndex => splitLine.Count > targetIndex ? splitLine[targetIndex] : "")
                    .ToList() ?? splitLine);
            }

            return result;
        }

        /// <summary>
        /// Returns a dictionary with keys and values including lists extracted from a CSV text.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The generated dictionary.</returns>
        public static IDictionary<string, IList<string>> ExtractIntoDictionaryIncludingLists(CsvExtractRequest request)
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

                IList<string> value = valueIndexes
                    ?.Select(targetIndex => splitLine.Count > targetIndex ? splitLine[targetIndex] : "")
                    .ToList() ?? splitLine;

                if (!result.TryAdd(key, value))
                {
                    Debug.LogWarning($"Duplicate key found: {key}");
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a nested dictionary with keys and values extracted from a CSV text.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The generated dictionary.</returns>
        public static IDictionary<string, IDictionary<string, string>> ExtractIntoNestedDictionary(
            CsvExtractRequest request
        )
        {
            var reader = new StringReader(request.Csv);

            if (!request.HasHeader)
            {
                Debug.LogError("Nested dictionaries cannot be extracted from CSV with headers.");
                return null;
            }

            string headerLine = reader.ReadLine();
            IList<string> headersInCsv = SplitLine(headerLine);

            IList<int> keyIndexes
                = DetermineTargetIndexes(request.KeyColumnIndexes, request.KeyColumnHeaders, headerLine);

            IList<int> valueIndexes
                = DetermineTargetIndexes(request.ValueColumnIndexes, request.ValueColumnHeaders, headerLine);

            IDictionary<string, IDictionary<string, string>> result
                = new Dictionary<string, IDictionary<string, string>>();

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

                IDictionary<string, string> value = valueIndexes != null
                    ? valueIndexes
                        .ToDictionary(
                            targetIndex => headersInCsv[targetIndex],
                            targetIndex => splitLine.Count > targetIndex ? splitLine[targetIndex] : ""
                        )
                    : splitLine
                        .Select((value, index) => new { value, index })
                        .ToDictionary(
                            pair => headersInCsv.Count > pair.index ? headersInCsv[pair.index] : "",
                            pair => pair.value
                        );

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
                if (string.IsNullOrEmpty(headerLine))
                {
                    Debug.LogWarning("The header line doesn't exist in the csv.");
                    return new List<int>();
                }

                IList<string> headersInCsv = SplitLine(headerLine);
                IList<int> result = new List<int>();

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
