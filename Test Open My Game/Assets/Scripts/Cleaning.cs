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

    private UserControl _userControl;
    private Animator _animator;
    private float _time;
    private bool _isStart;

    public float Step { get; set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _userControl = GetComponentInParent<UserControl>();
    }

    private void Update()
    {
        if (_isStart)
            Calculation();
    }

    /// <summary>
    /// Start animation "Destroy" for element if it is destroyed
    /// </summary>
    public void StartAnimation()
    {
        _userControl.enabled = false;
        _isStart = true;

        _animator.SetTrigger(_animationTrigger);
    }

    /// <summary>
    /// Calculating the time after which the object is completely destroyed
    /// </summary>
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
        {
            _userControl.enabled = true;
            normalization.DataInitialization();
            normalization.IsCheck = true;
        }
    }
}