using UnityEngine;

public class UnitAttack : MonoBehaviour, IUnitComponent
{
    public event System.Action onAttack;

    public AttackKind LastAttackKind { get; private set; }
    public AttackPreparingKind AttackPreparingKind { get; private set; }
    public Transform Target { get; set; }

    private UnitWeapon _currentWeapon;
    private Unit _unit;
    private event System.Action _onWeaponEquipped;
    private event System.Action _onAttackCompleted;

    public Unit Unit => _unit;

    public void InitializeOn(Unit unit)
    {
        _unit = unit;
        _unit.UnitAnimator.AnimationEvent += AnimationEventHandler;
        _unit.UnitAnimator.OnAttackCompleted += AttackAnimationCompletedHandler;
    }

    public UnitAttack DoAttack()
    {
        LastAttackKind = _currentWeapon.AttackKind;
        _unit.UnitAnimator.PlayAttack(_currentWeapon.AttackPreparingKind, _currentWeapon.AttackKind);
        return this;
    }
    public UnitAttack EquipWeapon(string key, int level, float delay = 0f)
    {
        if(delay == -1f)
        {
            delay = EntitiesRegistry.i.WeaponsRegistry[key].additionalEquipDelay;
        }
        _unit.UnitSkin.AttachItem($"weapon_{key}_{level}_prefab", 0, delay)
            .OnItemAttached((item, slot) => { _currentWeapon = item as UnitWeapon; _currentWeapon.Prepare(); _onWeaponEquipped?.Invoke(); });
        return this;
    }
    public UnitAttack ClearWeapon()
    {
        _currentWeapon.Dispose();
        _unit.UnitSkin.RemoveItem(0);
        return this;
    }
    public UnitAttack OnWeaponEquipped(System.Action callback)
    {
        _onWeaponEquipped += callback;
        return this;
    }
    public UnitAttack OnAttackCompleted(System.Action callback)
    {
        _onAttackCompleted += callback;
        return this;
    }

    private void AnimationEventHandler(string message)
    {
        if (message == "attack")
        {
            _currentWeapon.DoShot(Target.position);
            onAttack?.Invoke();
        }
    }
    private void AttackAnimationCompletedHandler()
    {
        _onAttackCompleted?.Invoke();
        _onAttackCompleted = null;
    }
}
