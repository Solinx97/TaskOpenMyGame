using UnityEngine;

public class Levels : MonoBehaviour
{
    #region UI

    [SerializeField]
    private GameObject[] _levels = new GameObject[0];

    #endregion

    private int _currentLevel;

    public void NextLevel()
    {
        if (_currentLevel < _levels.Length - 1)
        {
            _levels[_currentLevel].SetActive(false);
            _levels[_currentLevel + 1].SetActive(true);
            _currentLevel++;
        }
    }
}