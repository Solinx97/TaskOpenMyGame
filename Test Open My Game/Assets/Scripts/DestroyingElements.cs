using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestroyingElements : MonoBehaviour
{
    #region UI

    [Header("Element")]

    [SerializeField]
    private LayerMask _elementLayer = 0;
    [SerializeField]
    private int _numberForDestrucation = 3;

    [Space()]

    [SerializeField]
    private float _step = 45;

    #endregion

    private List<GameObject> _leftRightElements = new List<GameObject>();
    private List<GameObject> _topBottomElements = new List<GameObject>();
    private List<BoxCollider2D> _colliders = new List<BoxCollider2D>();

    public void DataInitialize(GameObject element)
    {
        _leftRightElements.Add(element);
        _topBottomElements.Add(element);
    }

    /// <summary>
    /// Finding adjacent elements of the same type to create a block of elements that will be destroyed
    /// </summary>
    /// <param name="selectedElement">Selected item to check</param>
    public void FindNeighboringElement(GameObject selectedElement)
    {
        SearchTowards(selectedElement.transform, DirectionType.Right, _leftRightElements);
        SearchTowards(selectedElement.transform, DirectionType.Left, _leftRightElements);
        SearchTowards(selectedElement.transform, DirectionType.Top, _topBottomElements);
        SearchTowards(selectedElement.transform, DirectionType.Bottom, _topBottomElements);
    }

    /// <summary>
    /// Combining elements of the same type vertically and horizontally into one block and checking the number of elements
    /// </summary>
    public void DestructionTowards()
    {
        var allElements = _leftRightElements.Union(_topBottomElements).ToList();

        if (allElements.Count >= _numberForDestrucation)
            Desctruction(allElements);
        else
            ToggleColliders(true);
    }

    public void Cleaning()
    {
        _leftRightElements.Clear();
        _topBottomElements.Clear();
    }

    /// <summary>
    /// Move the beam in the direction of the selected block's displacement and check for the presence of a second element of the same type
    /// </summary>
    /// <param name="elementTransform">Selected item transform</param>
    /// <param name="directionType">Direction</param>
    /// <param name="elementsForDestruction">Block elements one type for destruction</param>
    private void SearchTowards(Transform elementTransform, DirectionType directionType,
        List<GameObject> elementsForDestruction)
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
            _step, _elementLayer);

        if (hits.Length > 0)
        {
            Execute(hits, elementsForDestruction);
        }
    }

    /// <summary>
    /// Adding adjacent elements of the same type to a block
    /// </summary>
    /// <param name="hits">All adjacent elements</param>
    /// <param name="elementsForDestruction">Current block</param>
    private void Execute(RaycastHit2D[] hits, List<GameObject> elementsForDestruction)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            var collider = hits[i].collider.gameObject.GetComponent<BoxCollider2D>();
            _colliders.Add(collider);
            collider.enabled = false;

            elementsForDestruction.Add(hits[i].collider.gameObject);
        }

        for (int i = 0; i < elementsForDestruction.Count; i++)
        {
            FindNeighboringElement(elementsForDestruction[i]);
        }
    }

    private void Desctruction(List<GameObject> elementsForDestruction)
    {
        var matchesToX = SearchMatches(elementsForDestruction, true);
        var matchesToY = SearchMatches(elementsForDestruction, false);
        var result = matchesToX.Concat(matchesToY).ToList();

        foreach (var item in result)
        {
            var cleaning = item.GetComponent<Cleaning>();
            cleaning.Step = _step;
            cleaning.StartAnimation();
        }

        ToggleColliders(true);
    }

    /// <summary>
    /// Checking elements vertically and horizontally from a common block of found identical elements for destruction
    /// </summary>
    /// <param name="elementsForDestroy">Block elements</param>
    /// <param name="searchToX">Search on coord X or coord Y</param>
    /// <returns></returns>
    private List<GameObject> SearchMatches(List<GameObject> elementsForDestroy, bool searchToX)
    {
        var sortedElements = new List<float>();
        if (searchToX)
            sortedElements = elementsForDestroy.Select(val => val.transform.position.x).Distinct().ToList();
        else
            sortedElements = elementsForDestroy.Select(val => val.transform.position.y).Distinct().ToList();

        var identicalCollections = new List<List<GameObject>>();
        var result = new List<GameObject>();

        foreach (var item in sortedElements)
        {
            if (searchToX)
                identicalCollections.Add(elementsForDestroy.Where(val => val.transform.position.x == item).ToList());
            else
                identicalCollections.Add(elementsForDestroy.Where(val => val.transform.position.y == item).ToList());
        }

        foreach (var item in identicalCollections)
        {
            if (item.Count >= _numberForDestrucation)
                result = result.Concat(item).ToList();
        }

        return result;
    }

    /// <summary>
    /// Enable or disable the collider of an element that is already checked
    /// </summary>
    /// <param name="state">On/off</param>
    private void ToggleColliders(bool state)
    {
        foreach (var item in _colliders)
        {
            if (item != null)
            {
                var collider = item.GetComponent<BoxCollider2D>();
                collider.enabled = state;
            }
        }

        _colliders.Clear();
    }
}