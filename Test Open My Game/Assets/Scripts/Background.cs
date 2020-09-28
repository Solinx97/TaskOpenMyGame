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
        var position = element.position;
        if (!_test)
            element.position = new Vector3(position.x + _speed, position.y + toY, position.z);
        else
            element.position = new Vector3(position.x + _speed, position.y - toY, position.z);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _speed = -_speed;
    }
}