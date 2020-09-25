using System.Collections.Generic;
using UnityEngine;

public class Normalization : MonoBehaviour
{
    #region UI

    [SerializeField]
    private float _speed = 1;
    [SerializeField]
    private float _step = 57;

    #endregion

    private List<Transform> _elementsTransform = new List<Transform>();
    private List<Vector3> _newPositions = new List<Vector3>();
    private List<bool> _checkedItems = new List<bool>();
    private UserControl _userControl;

    public bool IsCheck { get; set; }

    public bool IsOffset { get; set; }

    private void Awake()
    {
        _userControl = GetComponent<UserControl>();
    }

    private void Update()
    {
        if (IsCheck)
        {
            if (!_checkedItems.Contains(false))
            {
                IsCheck = false;
                _userControl.enabled = true;
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

    private void SetElements(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            _elementsTransform.Add(parent.GetChild(i));
            _checkedItems.Add(false);
        }
    }

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

    private void ElementOffset()
    {
        for (int i = 0; i < _elementsTransform.Count; i++)
        {
            if (_elementsTransform[i] != null)
            {
                var position = _elementsTransform[i].position;
                if (_newPositions[i].y < position.y - _step)
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