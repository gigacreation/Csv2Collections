// ReSharper disable ArrangeObjectCreationWhenTypeEvident

using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace GigaCreation.Tools.Csv2Collections.Sample
{
    public class Csv2CollectionsSample : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _keysLabel;
        [SerializeField] private TextMeshProUGUI _valuesLabel;
        [SerializeField] private TextMeshProUGUI _equalityOperatorLabel;

        [Header("Parameters")]
        [SerializeField] private string _csvPath;
        [SerializeField] private bool _hasHeader;
        [SerializeField] private bool _extractByHeader;
        [SerializeField] private TestTarget _testTarget;
        [SerializeField] private string _keySeparator;
        [SerializeField] private string _valueSeparator;
        [SerializeField] private int[] _keyColumnIndexes;
        [SerializeField] private int[] _valueColumnIndexes;
        [SerializeField] private string[] _keyColumnHeaders;
        [SerializeField] private string[] _valueColumnHeaders;

        private readonly StringBuilder _keysBuilder = new StringBuilder();
        private readonly StringBuilder _valuesBuilder = new StringBuilder();
        private readonly StringBuilder _equalityOperatorBuilder = new StringBuilder();

        private void Start()
        {
            string csv = Resources.Load<TextAsset>(_csvPath).text;

            switch (_testTarget)
            {
                case TestTarget.List:
                    TestListExtracting(csv);
                    break;

                case TestTarget.Dictionary:
                    TestDictionaryExtracting(csv);
                    break;

                case TestTarget.MultiList:
                    TestMultiListExtracting(csv);
                    break;

                case TestTarget.MultiDictionary:
                    TestMultiDictionaryExtracting(csv);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TestListExtracting(string csv)
        {
            CsvExtractRequest request = _extractByHeader
                ? new CsvExtractRequest(csv, _valueSeparator, _valueColumnHeaders)
                : new CsvExtractRequest(csv, _hasHeader, _valueSeparator, _valueColumnIndexes);

            IList<string> response = CsvUtility.ExtractIntoList(request);

            if (response == null)
            {
                return;
            }

            ClearStringBuilders();

            for (var i = 0; i < response.Count; i++)
            {
                _keysBuilder.AppendLine($"[{i}]");
                _valuesBuilder.AppendLine(response[i]);
                _equalityOperatorBuilder.AppendLine("=");
            }

            _keysLabel.SetText(_keysBuilder);
            _valuesLabel.SetText(_valuesBuilder);
            _equalityOperatorLabel.SetText(_equalityOperatorBuilder);
        }

        private void TestDictionaryExtracting(string csv)
        {
            CsvExtractRequest request;

            if (_extractByHeader)
            {
                request = new CsvExtractRequest(csv, _valueSeparator, _valueColumnHeaders);
                request.SetKeyColumnHeaders(_keySeparator, _keyColumnHeaders);
            }
            else
            {
                request = new CsvExtractRequest(csv, _hasHeader, _valueSeparator, _valueColumnIndexes);
                request.SetKeyColumnIndexes(_keySeparator, _keyColumnIndexes);
            }

            IDictionary<string, string> response = CsvUtility.ExtractIntoDictionary(request);

            if (response == null)
            {
                return;
            }

            ClearStringBuilders();

            foreach (KeyValuePair<string, string> pair in response)
            {
                _keysBuilder.AppendLine($"[\"{pair.Key}\"]");
                _valuesBuilder.AppendLine(pair.Value);
                _equalityOperatorBuilder.AppendLine("=");
            }

            _keysLabel.SetText(_keysBuilder);
            _valuesLabel.SetText(_valuesBuilder);
            _equalityOperatorLabel.SetText(_equalityOperatorBuilder);
        }

        private void TestMultiListExtracting(string csv)
        {
            CsvExtractRequest request = _extractByHeader
                ? new CsvExtractRequest(csv, _valueColumnHeaders)
                : new CsvExtractRequest(csv, _hasHeader);

            IList<IList<string>> response = CsvUtility.ExtractIntoMultiList(request);

            if (response == null)
            {
                return;
            }

            ClearStringBuilders();

            for (var i = 0; i < response.Count; i++)
            {
                _keysBuilder.AppendLine($"[\"{i}\"]");
                _valuesBuilder.AppendLine(response[i][0]);
                _equalityOperatorBuilder.AppendLine("=");
            }

            _keysLabel.SetText(_keysBuilder);
            _valuesLabel.SetText(_valuesBuilder);
            _equalityOperatorLabel.SetText(_equalityOperatorBuilder);
        }

        private void TestMultiDictionaryExtracting(string csv)
        {
            CsvExtractRequest request;

            if (_extractByHeader)
            {
                request = new CsvExtractRequest(csv, _valueColumnHeaders);
                request.SetKeyColumnHeaders(_keySeparator, _keyColumnHeaders);
            }
            else
            {
                request = new CsvExtractRequest(csv, _hasHeader);
                request.SetKeyColumnIndexes(_keySeparator, _keyColumnIndexes);
            }

            IDictionary<string, IList<string>> response = CsvUtility.ExtractIntoMultiDictionary(request);

            if (response == null)
            {
                return;
            }

            ClearStringBuilders();

            foreach (KeyValuePair<string, IList<string>> pair in response)
            {
                _keysBuilder.AppendLine($"[\"{pair.Key}\"]");
                _valuesBuilder.AppendLine(pair.Value[0]);
                _equalityOperatorBuilder.AppendLine("=");
            }

            _keysLabel.SetText(_keysBuilder);
            _valuesLabel.SetText(_valuesBuilder);
            _equalityOperatorLabel.SetText(_equalityOperatorBuilder);
        }

        private void ClearStringBuilders()
        {
            _keysBuilder.Clear();
            _valuesBuilder.Clear();
            _equalityOperatorBuilder.Clear();
        }

        private enum TestTarget
        {
            List,
            Dictionary,
            MultiList,
            MultiDictionary
        }
    }
}
