﻿using System.Collections.Generic;
using UnityEngine;

public class Normalization : MonoBehaviour
{
    #region UI

    [SerializeField]
    private float _speed = 1;

    #endregion

    private List<Transform> _elementsTransform = new List<Transform>();
    private List<Vector3> _newPositions = new List<Vector3>();
    private List<bool> _checkedItems = new List<bool>();
    private UserControl _userControl;
    private MovementElements _movement;

    public bool IsCheck { get; set; }

    private void Awake()
    {
        _userControl = GetComponent<UserControl>();
        _movement = GetComponent<MovementElements>();
    }

    private void Update()
    {
        if (IsCheck)
        {
            _userControl.enabled = false;
            if (!_checkedItems.Contains(false))
            {
                _userControl.enabled = true;
                IsCheck = false;

                foreach (var item in _elementsTransform)
                {
                    _movement.DestructionElements(item.gameObject);
                }
            }

            GetOffsetPosition();
            ElementOffset();
        }
    }

    public void DataInitialization()
    {
        _elementsTransform.Clear();
        _checkedItems.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            SetElements(transform.GetChild(i));
        }
    }

    /// <summary>
    /// Getting all active items in a level
    /// </summary>
    /// <param name="parent"></param>
    private void SetElements(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            _elementsTransform.Add(parent.GetChild(i));
            _checkedItems.Add(false);
        }
    }

    /// <summary>
    /// Checking all the elements to see if they are hanging over the void
    /// </summary>
    public void GetOffsetPosition()
    {
        _newPositions.Clear();

        for (int i = 0; i < _elementsTransform.Count; i++)
        {
            if (_elementsTransform[i] != null)
            {
                var hits = Physics2D.RaycastAll(_elementsTransform[i].position, -_elementsTransform[i].transform.up);
                var position = hits[0].collider.transform.position;
                var distance = position - _elementsTransform[i].position;

                if (distance.magnitude > 0 || hits.Length == 1)
                    _newPositions.Add(position);
                else
                    _newPositions.Add(hits[1].collider.transform.position);
            }
            else
            {
                _elementsTransform.Remove(_elementsTransform[i]);
                _checkedItems.Remove(_checkedItems[i]);
                i--;
            }
        }
    }

    /// <summary>
    /// Move an element down one step if there is a void under the element
    /// </summary>
    private void ElementOffset()
    {
        for (int i = 0; i < _elementsTransform.Count; i++)
        {
            if (_elementsTransform[i] != null)
            {
                var position = _elementsTransform[i].position;
                if (_newPositions[i].y < position.y - _movement.Step)
                    _elementsTransform[i].position = new Vector2(position.x, position.y - _speed);
                else
                    _checkedItems[i] = true;
            }
            else
            {
                _elementsTransform.Remove(_elementsTransform[i]);
                _checkedItems.Remove(_checkedItems[i]);
                i--;
            }
        }
    }
}