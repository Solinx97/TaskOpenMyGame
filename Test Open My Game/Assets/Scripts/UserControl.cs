using UnityEngine;

public class UserControl : MonoBehaviour
{
    private MovementElements _movement;

    private void Awake()
    {
        _movement = GetComponent<MovementElements>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _movement.Activate();
        }
    }
}