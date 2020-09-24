using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestroyingElements : MonoBehaviour
{
    #region UI

    [Header("Element")]

    [SerializeField]
    private GameObject _destroyPref = null;
    [SerializeField]
    private LayerMask _elementLayer = 0;
    [SerializeField]
    private int _numberForDestrucation = 3;

    [Space()]

    [SerializeField]
    private float _step = 45;

    [Space()]

    [SerializeField]
    private float _rayCastDistance = 50;

    #endregion

    private List<GameObject> _leftElements = new List<GameObject>();
    private List<GameObject> _rightElements = new List<GameObject>();
    private List<GameObject> _topElements = new List<GameObject>();
    private List<GameObject> _bottomElements = new List<GameObject>();
    private List<BoxCollider2D> _colliders = new List<BoxCollider2D>();

    public void DataInitialize(GameObject element)
    {
        _leftElements.Add(element);
        _topElements.Add(element);
    }

    public void FindNeighboringElement(GameObject selectedElement)
    {
        SearchTowards(selectedElement.transform, DirectionType.Right, _rightElements);
        SearchTowards(selectedElement.transform, DirectionType.Left, _leftElements);
        SearchTowards(selectedElement.transform, DirectionType.Top, _topElements);
        SearchTowards(selectedElement.transform, DirectionType.Bottom, _bottomElements);
    }

    public void DestructionTowards()
    {
        if (_leftElements.Count + _rightElements.Count >= _numberForDestrucation)
        {
            var elements = _leftElements.Concat(_rightElements).ToList();
            Destruction(elements, DirectionType.Left);
        }

        if (_topElements.Count + _bottomElements.Count >= _numberForDestrucation)
        {
            var elements = _topElements.Concat(_bottomElements).ToList();
            Destruction(elements, DirectionType.Right);
        }
    }

    public void Cleaning()
    {
        _leftElements.Clear();
        _rightElements.Clear();
        _topElements.Clear();
        _bottomElements.Clear();
    }

    public void ToggleColliders()
    {
        foreach (var item in _colliders)
        {
            if (item != null)
            {
                var collider = item.GetComponent<BoxCollider2D>();
                collider.enabled = !collider.enabled;
            }
        }

        _colliders.Clear();
    }

    private void SearchTowards(Transform elementTransform, DirectionType directionType,
        List<GameObject> elementsForDestroy)
    {
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

        var hits = Physics2D.RaycastAll(elementTransform.position, direction,
            _rayCastDistance, _elementLayer);

        if (hits.Length > 0)
        {
            Execute(elementTransform, hits, elementsForDestroy, directionType);
        }
    }
    
    private void Execute(Transform mainObject, RaycastHit2D[] hits, List<GameObject> elementsForDestroy,
        DirectionType directionType)
    {
        float currentStep = _step;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].distance <= currentStep)
            {
                var collider = hits[i].collider.gameObject.GetComponent<BoxCollider2D>();
                _colliders.Add(collider);
                collider.enabled = false;

                if (elementsForDestroy.Any() && !elementsForDestroy.Contains(hits[i].collider.gameObject))
                {
                    bool isMatchPositions = DirectionPositionCheck(mainObject, 
                        hits[i].collider.transform, directionType);

                    if (isMatchPositions)
                        elementsForDestroy.Add(hits[i].collider.gameObject);
                }
                else if (!elementsForDestroy.Any())
                {
                    elementsForDestroy.Add(hits[i].collider.gameObject);
                }

                FindNeighboringElement(hits[i].collider.gameObject);
            }

            currentStep += _step;
        }
    }

    private void Destruction(List<GameObject> elements, DirectionType directionType)
    {
        if (directionType == DirectionType.Left || directionType == DirectionType.Right)
        {
            var test = elements.OrderBy(val => val.transform.position.y);
            var firstPosition = test.First().transform.position;
            var temp = new List<GameObject> { test.First() };

            for (int i = 1; i < elements.Count; i++)
            {
                if (elements[i].transform.position == firstPosition)
                {
                    temp.Add(elements[i]);
                }
            }

            foreach (var item in elements)
            {
                Instantiate(_destroyPref, item.transform.position,
                    item.transform.rotation, transform);

                Destroy(item.gameObject);
            }
        }

        if (directionType == DirectionType.Top || directionType == DirectionType.Bottom)
        {

        }
    }

    private bool DirectionPositionCheck(Transform mainElement, Transform newElement,
        DirectionType direction)
    {
        bool isMatchPositions = false;
        var firstElementPosition = mainElement.position;
        var newElementPosition = newElement.position;
        
        if (direction == DirectionType.Right || direction == DirectionType.Left)
        {
            if (newElementPosition.y == firstElementPosition.y)
                isMatchPositions = true;
        }
        else if (direction == DirectionType.Top || direction == DirectionType.Bottom)
        {
            if (newElementPosition.x == firstElementPosition.x)
                isMatchPositions = true;
        }

        return isMatchPositions;
    }
}