using UnityEngine;

public class Levels : MonoBehaviour
{
    #region UI

    [SerializeField]
    private GameObject[] _levels = new GameObject[0];

    #endregion

    private int _currentLevel;

    public bool IsCheck { get; set; } = true;

    private void Update()
    {
        if (IsCheck)
            LevelCompleted();
    }

    public void NextLevel()
    {
        if (_currentLevel < _levels.Length - 1)
        {
            IsCheck = false;
            _levels[_currentLevel].SetActive(false);
            _levels[_currentLevel + 1].SetActive(true);
            _currentLevel++;
        }
    }

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
                }

                break;
            }
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
}