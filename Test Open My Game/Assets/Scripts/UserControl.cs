using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserControl : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    #region UI

    [Tooltip("A sorted layer that will not be considered when selecting items")]
    [SerializeField]
    private int _ignoreSortingLayer = 0;

    #endregion

    private MovementElements _movement;

    private void Awake()
    {
        _movement = GetComponent<MovementElements>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var target = eventData.pointerCurrentRaycast.gameObject;
        int sortingLayer = eventData.pointerCurrentRaycast.sortingLayer;
        if (target != null && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y)
            && sortingLayer != _ignoreSortingLayer)
        {
            if (eventData.delta.x > 0)
                _movement.Activate(target, DirectionType.Right);
            else
                _movement.Activate(target, DirectionType.Left);
        }
        else if (target != null && sortingLayer != _ignoreSortingLayer)
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