using UnityEngine;

public enum WeaponKind { Melee, Ranged, Grenade }

public class UnitWeapon : UnitItem
{
    [SerializeField] BaseBulletSpawner _bulletSpawner;
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private AttackPreparingKind _attackPreparingKind;
    [SerializeField] private AttackKind _attackKind;
    [SerializeField] private WeaponKind _weaponKind;

    public Transform WeaponPoint => _weaponPoint;
    public AttackKind AttackKind => _attackKind;
    public WeaponKind WeaponKind => _weaponKind;
    public AttackPreparingKind AttackPreparingKind => _attackPreparingKind;

    public void Prepare()
    {
        _bulletSpawner.Prepare();
    }
    public void Dispose()
    {
        _bulletSpawner.Dispose();
    }
    public void DoShot(Vector3 target)
    {
        _bulletSpawner.DoShot(_weaponPoint, target);
    }
}
