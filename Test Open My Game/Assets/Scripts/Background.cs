using UnityEngine;

public class Background : MonoBehaviour
{
    #region UI

    [SerializeField]
    private float _speed = 1;

    [Header("Direction")]

    [Tooltip("Height of the amplitude of movement along a sinusoidal trajectory")]
    [SerializeField]
    private float _height = 5;

    #endregion

    private int _count;
    private bool _test;

    private void Update()
    {
        Moving(transform);
        _count++;

        if (_count >= _height)
        {
            _test = !_test;
            _count = 0;
        }
    }

    private void Moving(Transform element)
    {
        float toY = Mathf.Sin(_speed);
        if (!_test)
            element.position = new Vector2(element.position.x + _speed, element.position.y + toY);
        else
            element.position = new Vector2(element.position.x + _speed, element.position.y - toY);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _speed = -_speed;
    }
}