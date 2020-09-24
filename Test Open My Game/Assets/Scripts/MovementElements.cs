using System.Collections.Generic;
using UnityEngine;

public class MovementElements : MonoBehaviour
{
    #region UI

    [Tooltip("Speed swapping two elements")]
    [SerializeField]
    private float _speed = 1;
    [Tooltip("Step to adjacent element")]
    [SerializeField]
    private float _step = 56;

    #endregion

    private List<GameObject> _elementsForSwap = new List<GameObject>();
    private List<Vector2> _elementsPositions = new List<Vector2>();
    private UserControl _userControl;

    public float Step { get => _step; set => _step = value; }

    private void Awake()
    {
        _userControl = GetComponent<UserControl>();
    }

    private void Update()
    {
        Executing();
    }

    public void Activate()
    {
        var ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var info = Physics2D.OverlapPoint(ray);
        if (info != null && !_elementsForSwap.Contains(info.gameObject))
        {
            ToggleCollider(info.gameObject);
            _elementsForSwap.Add(info.gameObject);
            _elementsPositions.Add(info.gameObject.transform.position);
        }
    }

    private void Executing()
    {
        if (_elementsPositions.Count == 2)
        {
            bool isNeighbour = CheckingNeighbour(_elementsPositions[0], _elementsPositions[1]);
            if (isNeighbour)
            {
                _userControl.enabled = false;
                var firstDirection = ChoiceDirection(_elementsPositions[0], _elementsPositions[1]);
                var secondDirection = ChoiceDirection(_elementsPositions[1], _elementsPositions[0]);

                Swapping(firstDirection, secondDirection);
            }
            else
            {
                _elementsForSwap.Clear();
                _elementsPositions.Clear();
            }
        }
    }

    private void Swapping(DirectionType first, DirectionType second)
    {
        bool firstSwaped = false;
        bool secondSwapped = false;

        if (_elementsForSwap.Count == 2)
        {
            firstSwaped = Moving(_elementsForSwap[0], _elementsPositions[1], first);
            secondSwapped = Moving(_elementsForSwap[1], _elementsPositions[0], second);
        }

        if (firstSwaped && secondSwapped)
        {
            DestructionElements(_elementsForSwap[0]);
            DestructionElements(_elementsForSwap[1]);

            _elementsForSwap.Clear();
            _elementsPositions.Clear();
            _userControl.enabled = true;
        }
    }

    private bool Moving(GameObject obj, Vector3 targetPosition, DirectionType direction)
    {
        var position = obj.transform.position;
        bool isSwaped = false;
        var distance = obj.transform.position - targetPosition;

        if (distance.magnitude > 0)
        {
            switch (direction)
            {
                case DirectionType.Right:
                    obj.transform.position = new Vector3(position.x + _speed, position.y, position.z);
                    break;
                case DirectionType.Left:
                    obj.transform.position = new Vector3(position.x - _speed, position.y, position.z);
                    break;
                case DirectionType.Top:
                    obj.transform.position = new Vector3(position.x, position.y + _speed, position.z);
                    break;
                case DirectionType.Bottom:
                    obj.transform.position = new Vector3(position.x, position.y - _speed, position.z);
                    break;
                default:
                    break;
            }
        }
        else
        {
            isSwaped = true;
        }

        return isSwaped;
    }

    private DirectionType ChoiceDirection(Vector2 firstPosition, Vector2 secondPosition)
    {
        float differenceToX = firstPosition.x - secondPosition.x;

        DirectionType direction;
        if (differenceToX > 0)
        {
            direction = DirectionType.Left;
        }
        else if (differenceToX < 0)
        {
            direction = DirectionType.Right;
        }
        else
        {
            float differenceToY = firstPosition.y - secondPosition.y;
            if (differenceToY > 0)
            {
                direction = DirectionType.Bottom;
            }
            else
            {
                direction = DirectionType.Top;
            }
        }

        return direction;
    }

    private bool CheckingNeighbour(Vector3 firstPosition, Vector3 secondPosition)
    {
        bool isNeighbour = false;
        var distance = firstPosition - secondPosition;
        if(distance.magnitude <= Step)
        {
            isNeighbour = true;
        }

        return isNeighbour;
    }

    private void DestructionElements(GameObject element)
    {
        var destroyingElements = element.GetComponentInParent<DestroyingElements>();

        destroyingElements.DataInitialize(element);
        destroyingElements.FindNeighboringElement(element);
        destroyingElements.DestructionTowards();
        destroyingElements.Cleaning();
        destroyingElements.ToggleColliders();

        ToggleCollider(element);
    }

    private void ToggleCollider(GameObject target)
    {
        var collider = target.GetComponent<BoxCollider2D>();
        collider.enabled = !collider.enabled;
    }
}