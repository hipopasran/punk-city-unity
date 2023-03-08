using GLG;
using UnityEngine;

public class UnitMovement : MonoBehaviour, IUnitComponent, IManaged
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _speedUpDuration = 1f;
    [SerializeField] private AnimationCurve _velocityChangeCurve;

    private Unit _unit;
    private Transform _cashedTransform;
    private bool _wantToMove = false;
    private bool _wantToAnimate = false;
    private Vector3 _targetDirection;
    private Vector3 _currentVelocity;
    private float _t;

    public Unit Unit => _unit;

    public void InitializeOn(Unit unit)
    {
        Kernel.RegisterManaged(this);
        _unit = unit;
        _cashedTransform = transform;
    }
    private void OnDestroy()
    {
        Kernel.UnregisterManaged(this);
    }
    public void ManagedUpdate()
    {
        if(_wantToMove)
        {
            _t += Time.deltaTime;
            float velocityChangeProgress = Mathf.InverseLerp(0f, 1f, _t / _speedUpDuration);
            if (_targetDirection.sqrMagnitude > 0.0001)
            {
                _cashedTransform.forward = _targetDirection;
            }
            float speed = _maxSpeed * _velocityChangeCurve.Evaluate(velocityChangeProgress);
            _currentVelocity = _cashedTransform.forward * speed * Time.deltaTime;
            if(_wantToAnimate)
            {
                _unit.UnitAnimator.SetFloat("MovingSpeed", speed);
            }
            _cashedTransform.position += _currentVelocity;
            _wantToMove = false;
        }
        else
        {
            _t = 0f;
            _unit.UnitAnimator.SetFloat("MovingSpeed", 0f);
        }
    }

    public void Move(Vector3 direction, bool withAnimation = true)
    {
        _targetDirection = direction.normalized;
        if (_targetDirection.sqrMagnitude > 0.0001)
        {
            _cashedTransform.forward = _targetDirection;
        }
        _wantToMove = true;
        _wantToAnimate = withAnimation;
        _t += Time.deltaTime;
    }

    public void Warp(Vector3 worldPosition, Quaternion worldRotation)
    {
        _cashedTransform.position = worldPosition;
        _cashedTransform.rotation = worldRotation;
    }
}
