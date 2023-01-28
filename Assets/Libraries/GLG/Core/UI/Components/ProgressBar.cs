using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image _filling;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _cashedTransform;

    public Transform CashedTransform => _cashedTransform;
    public CanvasGroup CanvasGroup => _canvasGroup;

    public void SetValue(float value)
    {
        _filling.fillAmount = value;
    }
}
