// ReSharper disable ArrangeObjectCreationWhenTypeEvident

using System.Text;
using UnityEngine;

namespace GigaCreation.Tools.Csv2Collections.Sample
{
    public class CsvParserSample : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _row;
        [SerializeField] private GameObject _column;

        [Header("Parameters")]
        [SerializeField] private string _csvPath;

        private readonly StringBuilder _keysBuilder = new StringBuilder();
        private readonly StringBuilder _valuesBuilder = new();
        private readonly StringBuilder _equalityOperatorBuilder = new();

        private void Start()
        {
            string csv = Resources.Load<TextAsset>(_csvPath).text;

            string[,] table = CsvParser.Parse(csv);

            Debug.Log(table);
        }
    }
}
