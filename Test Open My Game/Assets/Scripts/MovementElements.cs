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
    private bool _isNextItemEmpty;
    private GameObject _element;
    private Vector2 _emptyPosition;
    private DirectionType _emptyDirection;

    public float Step { get => _step; set => _step = value; }

    private void Awake()
    {
        _userControl = GetComponent<UserControl>();
    }

    private void Update()
    {
        if (_isNextItemEmpty)
        {
            var isSwapped = Moving(_element, _emptyPosition, _emptyDirection);
            if (isSwapped)
            {
                _isNextItemEmpty = false;

                var normalization = GetComponent<Normalization>();
                normalization.DataInitialization();
                normalization.IsCheck = true;
                ToggleCollider(_element, true);
            }
        }

        if (_elementsPositions.Count == 2)
            Executing();
    }

    public void Activate(GameObject target, DirectionType direction)
    {
        ToggleCollider(target, false);

        _isNextItemEmpty = FindNextItem(target.transform, direction);

        if (_isNextItemEmpty && direction == DirectionType.Top)
            _isNextItemEmpty = false;

        if (!_isNextItemEmpty)
            DataInitialize(target);

        if (_isNextItemEmpty)
        {
            _element = target;
            _emptyDirection = direction;
            var elementPosition = target.transform.position;

            switch (direction)
            {
                case DirectionType.Right:
                    _emptyPosition = new Vector2(elementPosition.x + _step, elementPosition.y);
                    break;
                case DirectionType.Left:
                    _emptyPosition = new Vector2(elementPosition.x - _step, elementPosition.y);
                    break;
                case DirectionType.Top:
                    _emptyPosition = new Vector2(elementPosition.x, elementPosition.y + _step);
                    break;
                case DirectionType.Bottom:
                    _emptyPosition = new Vector2(elementPosition.x, elementPosition.y - _step);
                    break;
                default:
                    break;
            }
        }
    }

    public void DataInitialize(GameObject element)
    {
        _elementsForSwap.Add(element);
        _elementsPositions.Add(element.transform.position);
    }

    private bool FindNextItem(Transform elementTransform, DirectionType directionType)
    {
        bool isNextItemEmpty = true;
        var direction = new Vector2();

        switch (directionType)
        {
            case DirectionType.Right:
                direction = elementTransform.right;
                break;
            case DirectionType.Left:
                direction = -elementTransform.right;
                break;
            case DirectionType.Top:
                direction = elementTransform.up;
                break;
            case DirectionType.Bottom:
                direction = -elementTransform.up;
                break;
            default:
                break;
        }

        var hit = Physics2D.Raycast(elementTransform.position, direction, _step);

        if (hit)
        {
            isNextItemEmpty = false;
            DataInitialize(hit.collider.gameObject);
            ToggleCollider(hit.collider.gameObject, false);
        }
        else
        {
            _elementsForSwap.Clear();
            _elementsPositions.Clear();
        }

        return isNextItemEmpty;
    }

    private void Executing()
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
            foreach (var item in _elementsForSwap)
            {
                ToggleCollider(item, false);
            }

            _elementsForSwap.Clear();
            _elementsPositions.Clear();
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

        ToggleCollider(element, true);
    }

    private void ToggleCollider(GameObject target, bool state)
    {
        var collider = target.GetComponent<BoxCollider2D>();
        collider.enabled = state;
    }
}