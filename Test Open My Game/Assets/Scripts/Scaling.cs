using UnityEngine;

public class Scaling : MonoBehaviour
{
    #region UI

    [Tooltip("The coefficient of the percentage (for example, if you need 5% - indicate 0.05)")]
    [SerializeField]
    private float _scaleToX = 0.02f;
    [SerializeField]
    private float _scaleToY = 0.02f;

    [Header("Scaling")]

    [SerializeField]
    private ScalingType _scalingType = ScalingType.Rectangle;

    #endregion

    private void Update()
    {
        switch (_scalingType)
        {
            case ScalingType.Square:
                ScuareScaling();
                break;
            case ScalingType.Rectangle:
                RectangleScaling();
                break;
            default:
                break;
        }
    }

    private void ScuareScaling()
    {
        float height = Screen.height;
        transform.localScale = new Vector2(height * _scaleToX, height * _scaleToX);
    }

    private void RectangleScaling()
    {
        float width = Screen.width;
        float height = Screen.height;

        float temp = Camera.main.orthographicSize;
        //print(temp);
        transform.localScale = new Vector2(width * _scaleToX, height * _scaleToY);
    }
}