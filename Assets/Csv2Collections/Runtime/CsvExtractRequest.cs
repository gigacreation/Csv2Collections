using UnityEngine;

namespace GigaCreation.Tools.Csv2Collections
{
    public class CsvExtractRequest
    {
        /// <summary>
        /// The CSV text.
        /// </summary>
        public string Csv { get; }

        /// <summary>
        /// If true, the first line of the CSV file is treated as a header.
        /// </summary>
        public bool HasHeader { get; }

        /// <summary>
        /// The separator if multiple columns are specified.
        /// </summary>
        public string Separator { get; }

        /// <summary>
        /// The indexes of the columns to extract as a value.
        /// </summary>
        public int[] ValueColumnIndexes { get; }

        /// <summary>
        /// The indexes of the columns to extract as a key.
        /// </summary>
        public int[] KeyColumnIndexes { get; private set; }

        public CsvExtractRequest(string csv, bool hasHeader = false, string separator = null, params int[] valueIndexes)
        {
            Csv = csv;
            HasHeader = hasHeader;
            Separator = separator;
            ValueColumnIndexes = valueIndexes;
        }

        public void SetKeyColumnIndexes(params int[] indexes)
        {
            KeyColumnIndexes = indexes;
        }

        public int GetMaxTargetColumnIndex()
        {
            return Mathf.Max(
                KeyColumnIndexes == null ? 0 : Mathf.Max(KeyColumnIndexes),
                ValueColumnIndexes == null ? 0 : Mathf.Max(ValueColumnIndexes)
            );
        }
    }
}
