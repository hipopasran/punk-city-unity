using UnityEngine;

public enum WeaponKind { Melee, Ranged, Grenade }

public class UnitWeapon : UnitItem
{
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private AttackPreparingKind _attackPreparingKind;
    [SerializeField] private AttackKind _attackKind;
    [SerializeField] private WeaponKind _weaponKind;

    public Transform WeaponPoint => _weaponPoint;
    public AttackKind AttackKind => _attackKind;
    public WeaponKind WeaponKind => _weaponKind;
    public AttackPreparingKind AttackPreparingKind => _attackPreparingKind;
}
