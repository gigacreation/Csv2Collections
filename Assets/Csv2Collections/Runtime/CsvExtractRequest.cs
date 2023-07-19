namespace GigaCreation.Tools.Csv2Collections
{
    public class CsvExtractRequest
    {
        /// <summary>
        /// The CSV text.
        /// </summary>
        public string Csv { get; }

        /// <summary>
        /// If true, the first line of the CSV text is treated as a header.
        /// </summary>
        public bool HasHeader { get; }

        /// <summary>
        /// The separator for value if multiple columns are specified.
        /// </summary>
        public string ValueSeparator { get; }

        /// <summary>
        /// The indexes of the columns to extract as a value.
        /// </summary>
        public int[] ValueColumnIndexes { get; }

        /// <summary>
        /// The headers of the columns to extract as a value.
        /// </summary>
        public string[] ValueColumnHeaders { get; }

        /// <summary>
        /// The separator for key if multiple columns are specified.
        /// </summary>
        public string KeySeparator { get; private set; }

        /// <summary>
        /// The indexes of the columns to extract as a key.
        /// </summary>
        public int[] KeyColumnIndexes { get; private set; }

        /// <summary>
        /// The headers of the columns to extract as a key.
        /// </summary>
        public string[] KeyColumnHeaders { get; private set; }

        public CsvExtractRequest(
            string csv, bool hasHeader = false, string valueSeparator = null, params int[] valueColumnIndexes
        )
        {
            Csv = csv;
            HasHeader = hasHeader;
            ValueSeparator = valueSeparator;
            ValueColumnIndexes = valueColumnIndexes;
        }

        public CsvExtractRequest(string csv, params string[] valueColumnHeaders)
        {
            Csv = csv;
            HasHeader = true;
            ValueSeparator = null;
            ValueColumnHeaders = valueColumnHeaders;
        }

        public CsvExtractRequest(string csv, string valueSeparator, params string[] valueColumnHeaders)
        {
            Csv = csv;
            HasHeader = true;
            ValueSeparator = valueSeparator;
            ValueColumnHeaders = valueColumnHeaders;
        }

        public void SetKeyColumnIndexes(string separator = null, params int[] indexes)
        {
            KeySeparator = separator;
            KeyColumnIndexes = indexes;
        }

        public void SetKeyColumnHeaders(string separator = null, params string[] headers)
        {
            KeySeparator = separator;
            KeyColumnHeaders = headers;
        }
    }
}
