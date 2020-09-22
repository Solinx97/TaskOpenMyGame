using System.Collections.Generic;
using UnityEngine;

public class ElementsGeneration : MonoBehaviour
{
    #region UI

    [Header("Grid")]

    [SerializeField]
    private int _lines = 5;
    [SerializeField]
    private int _cells = 5;
    [SerializeField]
    private float _stepToX = 55;
    [SerializeField]
    private float _stepToY = 55;
    [Tooltip("Start position for elements generation on the grid")]
    [SerializeField]
    private Vector2 _startPosition = new Vector2();

    [Header("Elements")]

    [SerializeField]
    private GameObject _elementPref = null;

    [Header("Generation")]

    [Tooltip("The cell numbers in which the element is created. Format: 'Line.Cell' (example, '2.3')")]
    [SerializeField]
    private string []_cellsNumbers = new string[0];

    #endregion

    private void Start()
    {
        var lines = Split(_cellsNumbers, GridType.Line);
        var cells = Split(_cellsNumbers, GridType.Cell);

        Generation(lines, cells);
    }

    private void Generation(List<int> linesNumbers, List<int> cellsNumbers)
    {
        float toX = 0;
        float toY = 0;

        for (int i = 0; i < _lines; i++)
        {
            for (int j = 0; j < _cells; j++)
            {
                if (linesNumbers.Contains(i + 1))
                {
                    var currentIndex = linesNumbers.IndexOf(i + 1);
                    if (j + 1 == cellsNumbers[currentIndex])
                    {
                        var position = new Vector2(_startPosition.x + toX, _startPosition.y + toY);
                        Instantiate(_elementPref, position, _elementPref.transform.rotation, transform);

                        linesNumbers.Remove(linesNumbers[currentIndex]);
                        cellsNumbers.Remove(cellsNumbers[currentIndex]);
                    }
                }

                toX += _stepToX;
            }

            toY += _stepToY;
            toX = 0;
        }
    }

    private List<int> Split(string[] elementsForSplit, GridType gridType)
    {
        var numbers = new List<int>();

        for (int i = 0; i < elementsForSplit.Length; i++)
        {
            var split = elementsForSplit[i].Split('.');

            switch (gridType)
            {
                case GridType.Line:
                    numbers.Add(int.Parse(split[0]));
                    break;
                case GridType.Cell:
                    numbers.Add(int.Parse(split[1]));
                    break;
                default:
                    break;
            }
        }

        return numbers;
    }
}
