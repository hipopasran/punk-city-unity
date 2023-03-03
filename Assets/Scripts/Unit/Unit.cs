using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitAnimator _unitAnimator;
    [SerializeField] private UnitMovement _unitMovement;
    [SerializeField] private UnitSkin _unitSkin;

    public UnitAnimator UnitAnimator => _unitAnimator;
    public UnitMovement UnitMovement => _unitMovement;
    public UnitSkin UnitSkin => _unitSkin;

    private void Awake()
    {
        _unitAnimator.InitializeOn(this);
        _unitMovement.InitializeOn(this);
        _unitSkin.InitializeOn(this);
    }

}
