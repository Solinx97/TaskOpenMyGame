using UnityEngine;

public class Cleaning : MonoBehaviour
{
    #region UI

    [Header("Destroy")]

    [SerializeField]
    private string _animationTrigger = "Destroy";
    [SerializeField]
    private float _destroyTime = 3;

    #endregion

    private Animator _animator;
    private float _time;
    private bool _isStart;

    public float Step { get; set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isStart)
            Calculation();
    }

    public void StartAnimation()
    {
        _isStart = true;

        _animator.SetTrigger(_animationTrigger);
    }

    private void Calculation()
    {
        _time += Time.deltaTime;

        if (_time >= _destroyTime)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        var normalization = GetComponentInParent<Normalization>();
        if (normalization != null)
            normalization.IsCheck = true;
    }
}