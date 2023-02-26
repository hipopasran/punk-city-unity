using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour, IUnitComponent
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _movingSpeedParameterName = "MovingSpeed";

    public event Action<string> AnimationEvent;

    private Unit _unit;

    public Unit Unit => _unit;

    public void InitializeOn(Unit unit)
    {
        _unit = unit;
    }

    public void AnimationEventHandler(string parameter)
    {
        AnimationEvent?.Invoke(parameter);
    }

    public void SetTrigger(string parameterName)
    {
        _animator.SetTrigger(parameterName);
    }
    public void ResetTrigger(string parameterName)
    {
        _animator.ResetTrigger(parameterName);
    }
    public void SetFloat(string parameterName, float value)
    {
        _animator.SetFloat(parameterName, value);
    }
    public void SetBool(string parameterName, bool value)
    {
        _animator.SetBool(parameterName, value);
    }

    public void ApplyMovementSpeed(float speed)
    {
        _animator.SetFloat(_movingSpeedParameterName, speed);
    }
}
