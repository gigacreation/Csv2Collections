// ReSharper disable ArrangeObjectCreationWhenTypeEvident

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace GigaCreation.Tools.Csv2Collections.Sample
{
    public class CsvParserSample : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _table;
        [SerializeField] private Transform _column;
        [SerializeField] private Transform _cell;

        [Header("Parameters")]
        [SerializeField] private string _csvPath;

        private readonly StringBuilder _keysBuilder = new StringBuilder();
        private readonly StringBuilder _valuesBuilder = new();
        private readonly StringBuilder _equalityOperatorBuilder = new();

        private void Start()
        {
            string csv = Resources.Load<TextAsset>(_csvPath).text;

            List<List<string>> table = CsvParser.Parse(csv);

            int numOfRows = table.Count;
            int numOfColumns = table.Max(row => row.Count);

            for (var i = 0; i < numOfRows - 1; i++)
            {
                Instantiate(_cell, _cell.parent);
            }

            for (var j = 0; j < numOfColumns - 1; j++)
            {
                Instantiate(_column, _column.parent);
            }

            Transform[] columns = _table.Cast<Transform>().ToArray();

            for (var columnIndex = 0; columnIndex < columns.Length; columnIndex++)
            {
                Transform[] cells = columns[columnIndex].Cast<Transform>().ToArray();

                for (var rowIndex = 0; rowIndex < cells.Length; rowIndex++)
                {
                    string text = table[rowIndex].Count > columnIndex ? table[rowIndex][columnIndex] : "";
                    cells[rowIndex].GetComponent<TextMeshProUGUI>().SetText(text);
                }
            }

            Debug.Log(table);
        }
    }
}
