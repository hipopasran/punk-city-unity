using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UnitAttack : MonoBehaviour, IUnitComponent
{

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
        _unit.UnitAnimator.PlayAttack(_currentWeapon.AttackPreparingKind, _currentWeapon.AttackKind);
        return this;
    }
    public UnitAttack EquipWeapon(string key, int level)
    {
        AsyncOperationHandle<GameObject> asyncOperation = Addressables.LoadAssetAsync<GameObject>(name);
        asyncOperation.Completed += WeaponLoadedHandler;
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


    private void WeaponLoadedHandler(AsyncOperationHandle<GameObject> asyncOperation)
    {
        if (_currentWeapon != null)
        {
            Addressables.ReleaseInstance(_currentWeapon.gameObject);
        }
        _currentWeapon = Instantiate(asyncOperation.Result).GetComponent<UnitWeapon>();
        _unit.UnitSkin.AttachItem(_currentWeapon, 0);
        _onWeaponEquipped?.Invoke();
        _onWeaponEquipped = null;
    }

    private void AnimationEventHandler(string message)
    {

    }
    private void AttackAnimationCompletedHandler()
    {
        _onAttackCompleted?.Invoke();
        _onAttackCompleted = null;
    }
}
