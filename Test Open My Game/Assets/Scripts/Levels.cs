using UnityEngine;

public class Levels : MonoBehaviour
{
    #region UI

    [SerializeField]
    private GameObject[] _levels = new GameObject[0];

    #endregion

    private bool _isReload;
    private int _currentLevel;
    private ElementsGeneration[] _elementsGenerations;

    public bool IsCheck { get; set; }

    public GameObject ActiveLevel { get; set; }

    private void Start()
    {
        IsCheck = true;
        TakeActiveLevel(0);
    }

    private void Update()
    {
        if (IsCheck && !_isReload)
            LevelCompleted();
    }

    /// <summary>
    /// Level change by pressing the button
    /// </summary>
    public void NextLevel()
    {
        if (_currentLevel < _levels.Length - 1)
        {
            IsCheck = false;
            _levels[_currentLevel].SetActive(false);
            _levels[_currentLevel + 1].SetActive(true);
            _currentLevel++;
            TakeActiveLevel(_currentLevel);
        }
    }

    /// <summary>
    /// Reload the current level
    /// </summary>
    public void LevelRestart()
    {
        _isReload = true;

        for (int i = 0; i < ActiveLevel.transform.childCount; i++)
        {
            ElementsDestroying(ActiveLevel.transform.GetChild(i));
        }

        for (int i = 0; i < _elementsGenerations.Length; i++)
        {
            _elementsGenerations[i].DataInitialize();
            _elementsGenerations[i].Executing();
        }

        _isReload = false;
    }

    /// <summary>
    /// Completion of the level, provided that all elements are destroyed
    /// </summary>
    private void LevelCompleted()
    {
        for (int i = _currentLevel; i < _levels.Length - 1; i++)
        {
            if (_levels[i].activeSelf)
            {
                int count = SumActiveElements(_levels[i].transform);
                if (count == 0)
                {
                    _levels[i].SetActive(false);
                    _levels[i + 1].SetActive(true);
                    _currentLevel++;
                    TakeActiveLevel(_currentLevel);
                }

                break;
            }
        }
    }

    private void ElementsDestroying(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private int SumActiveElements(Transform level)
    {
        int count = 0;
        for (int i = 0; i < level.childCount; i++)
        {
            count += level.GetChild(i).childCount;
        }

        return count;
    }

    private void TakeActiveLevel(int level)
    {
        ActiveLevel = _levels[level];
        _elementsGenerations = ActiveLevel.GetComponentsInChildren<ElementsGeneration>();
    }
}