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
        var elements = _leftElements.Concat(_rightElements).ToList();
        var elements1 = _topElements.Concat(_bottomElements).ToList();
        var allElements = elements.Union(elements1).ToList();

        if (allElements.Count >= _numberForDestrucation)
            Desctruction(allElements);
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
            _step, _elementLayer);

        if (hits.Length > 0)
        {
            Execute(hits, elementsForDestroy);
        }
    }
    
    private void Execute(RaycastHit2D[] hits, List<GameObject> elementsForDestroy)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            var collider = hits[i].collider.gameObject.GetComponent<BoxCollider2D>();
            _colliders.Add(collider);
            collider.enabled = false;

            elementsForDestroy.Add(hits[i].collider.gameObject);
        }

        for (int i = 0; i < elementsForDestroy.Count; i++)
        {
            FindNeighboringElement(elementsForDestroy[i]);
        }
    }

    private void Desctruction(List<GameObject> gameObjects)
    {
        foreach (var item in gameObjects)
        {
            Instantiate(_destroyPref, item.transform.position,
                item.transform.rotation, transform);

            Destroy(item.gameObject);
        }
    }
}