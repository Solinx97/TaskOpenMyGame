using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserControl : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private MovementElements _movement;

    private void Awake()
    {
        _movement = GetComponent<MovementElements>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var target = eventData.pointerCurrentRaycast.gameObject;
        if (target != null && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
        {
            if (eventData.delta.x > 0)
                _movement.Activate(target, DirectionType.Right);
            else
                _movement.Activate(target, DirectionType.Left);
        }
        else if (target != null)
        {
            if (eventData.delta.y > 0)
                _movement.Activate(target, DirectionType.Top);
            else
            {
                _movement.Activate(target, DirectionType.Bottom);
            }
        }
    }

    public void OnDrag(PointerEventData eventData) {}
}