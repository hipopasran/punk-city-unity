using DG.Tweening;
using UnityEngine;

public class Skin : MonoBehaviour
{
    [SerializeField] private Transform[] _itemsSlots;
    [SerializeField] private Transform _bonesRoot;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _chest;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    private Color _defaultColor;
    private Color _defaultEmissionColor;

    public Transform[] ItemsSlots => _itemsSlots;
    public Transform BonesRoot => _bonesRoot;
    public Transform Head => _head;
    public Transform Chest => _chest;
    public SkinnedMeshRenderer Renderer => _skinnedMeshRenderer;

    private void Awake()
    {
        _defaultColor = _skinnedMeshRenderer.material.GetColor("_Color");
        _defaultEmissionColor = _skinnedMeshRenderer.material.GetColor("_EmissionColor");
    }

    public void SetMaterial(Material material)
    {
        _skinnedMeshRenderer.material = material;
    }
    public void SetColor(Color color, float duration = 0f)
    {
        if (duration <= 0f)
        {
            _skinnedMeshRenderer.material.SetColor("_Color", color);
        }
        else
        {
            _skinnedMeshRenderer.material.DOKill();
            _skinnedMeshRenderer.material.DOColor(color, duration);
        }
    }
    public void ResetColor(float duration = 0f)
    {
        if (duration <= 0f)
        {
            _skinnedMeshRenderer.material.SetColor("_Color", _defaultColor);
        }
        else
        {
            _skinnedMeshRenderer.material.DOKill();
            _skinnedMeshRenderer.material.DOColor(_defaultColor, duration);
        }
    }
    public void SetEmissionColor(Color color)
    {
        _skinnedMeshRenderer.material.SetColor("_EmissionColor", color);
    }
    public void ResetEmissionColor()
    {
        _skinnedMeshRenderer.material.SetColor("_EmissionColor", _defaultEmissionColor);
    }
    public void PulseColor(Color targetColor, float duration = 1f)
    {
        _skinnedMeshRenderer.material.DOKill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_skinnedMeshRenderer.material.DOColor(targetColor, duration / 2f));
        sequence.Append(_skinnedMeshRenderer.material.DOColor(_defaultColor, duration / 2f).SetEase(Ease.OutExpo));
    }
}
