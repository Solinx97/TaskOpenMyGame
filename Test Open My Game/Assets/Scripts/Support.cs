using UnityEngine;

public class Support : MonoBehaviour
{
    #region UI

    [Header("Element")]

    [SerializeField]
    private GameObject _pref = null;

    [Header("Params")]
    [SerializeField]
    private Vector2 _startPosition = new Vector2();
    [SerializeField]
    private int _cells = 5;
    [SerializeField]
    private float _step = 0.01f;

    #endregion

    private void Start()
    {
        float height = Screen.height;
        float step = height * _step;
        float toX = 0;

        _startPosition = new Vector2(_startPosition.x, _startPosition.y - step);
        for (int i = 0; i < _cells; i++)
        {
            Generation(toX);

            toX += step;
        }
    }

    private void Generation(float toX)
    {
        var prefPosition = _pref.transform.position;
        var position = new Vector3(_startPosition.x + toX, _startPosition.y, prefPosition.z);

        Instantiate(_pref, position, _pref.transform.rotation, transform);
    }
}