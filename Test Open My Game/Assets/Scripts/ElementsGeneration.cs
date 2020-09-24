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

    private List<int> _splitLines;
    private List<int> _splitCells;

    private void Start()
    {
        _splitLines = Split(_cellsNumbers, GridType.Line);
        _splitCells = Split(_cellsNumbers, GridType.Cell);

        Generation();
    }

    private void Generation()
    {
        float toX = 0;
        float toY = 0;

        for (int i = 0; i < _lines; i++)
        {
            for (int j = 0; j < _cells; j++)
            {
                if (_splitLines.Contains(i + 1))
                {
                    SearchMatch(i + 1, j + 1, toX, toY);
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

    private void SearchMatch(int indexToLine, int indexToCell, float toX, float toY)
    {
        int currentIndex = _splitLines.IndexOf(indexToLine);
        if (indexToCell == _splitCells[currentIndex])
        {
            var position = new Vector2(_startPosition.x + toX, _startPosition.y + toY);
            Instantiate(_elementPref, position, _elementPref.transform.rotation, transform);

            _splitLines.Remove(_splitLines[currentIndex]);
            _splitCells.Remove(_splitCells[currentIndex]);
        }
        else
        {
            int currentIndex1 = _splitCells.IndexOf(indexToCell, currentIndex);
            if (currentIndex1 >= 0)
            {
                var position = new Vector2(_startPosition.x + toX, _startPosition.y + toY);
                Instantiate(_elementPref, position, _elementPref.transform.rotation, transform);

                _splitLines.Remove(_splitLines[currentIndex]);
                _splitCells.Remove(_splitCells[currentIndex1]);
            }
        }
    }
}