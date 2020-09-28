using System.Collections.Generic;
using UnityEngine;

public class ElementsGeneration : MonoBehaviour
{
    #region UI

    [Header("Elements")]

    [SerializeField]
    private GameObject _elementPref = null;

    [Header("Generation")]

    [Tooltip("The cell numbers in which the element is created. Format: 'Line.Cell' (example, '2.3')")]
    [SerializeField]
    private string []_cellsNumbers = new string[0];

    #endregion

    private MovementElements _movement;
    private Levels _levels;
    private List<int> _splitLines;
    private List<int> _splitCells;

    private void Awake()
    {
        _movement = GetComponentInParent<MovementElements>();
        _levels = GetComponentInParent<Levels>();
    }

    private void Start()
    {
        DataInitialize();

        Executing();
    }

    public void DataInitialize()
    {
        _splitLines = Split(_cellsNumbers, GridType.Line);
        _splitCells = Split(_cellsNumbers, GridType.Cell);
    }

    public void Executing()
    {
        float toX = 0;
        float toY = 0;

        for (int i = 0; i < _movement.Lines; i++)
        {
            for (int j = 0; j < _movement.Cells; j++)
            {
                if (_splitLines.Contains(i + 1))
                {
                    SearchMatch(i + 1, j + 1, toX, toY);
                }

                toX += _movement.Step;
            }

            toY += _movement.Step;
            toX = 0;
        }

        var normalization = GetComponentInParent<Normalization>();
        normalization.DataInitialization();

        _levels.IsCheck = true;
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
            Generation(currentIndex, currentIndex, toX, toY);
        }
        else
        {
            int currentIndex1 = _splitCells.IndexOf(indexToCell, currentIndex);
            if (currentIndex1 >= 0)
            {
                Generation(currentIndex, currentIndex1, toX, toY);
            }
        }
    }

    private void Generation(int indexToLine, int indexToCell, float toX, float toY)
    {
        var position = new Vector2(_movement.StartPosition.x + toX, _movement.StartPosition.y + toY);
        Instantiate(_elementPref, position, _elementPref.transform.rotation, transform);

        _splitLines.Remove(_splitLines[indexToLine]);
        _splitCells.Remove(_splitCells[indexToCell]);
    }
}