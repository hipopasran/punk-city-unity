using UnityEngine;

public class LobbyUnitsSpawner : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private Transform _playerSlot;
    [SerializeField] private Transform[] _otherUnitsSlots;

    private Unit _player;
    private Unit[] _otherUnits;
    private int _slotsCounter;

    public int OtherSlotsCount => _otherUnitsSlots.Length;
    public Unit Player => _player;

    private void Awake()
    {
        _otherUnits =  new Unit[_otherUnitsSlots.Length];
    }

    public void SpawnPlayer(Profile profile)
    {
        if(_player == null)
        {
            _player = Instantiate(_unitPrefab);
        }
        ApplyUnitToSlot(_player, _playerSlot);
        ApplyProfileToUnit(profile, _player);
    }
    public void SpawnOtherUnit(Profile profile)
    {
        if (_otherUnits[_slotsCounter] == null)
        {
            _otherUnits[_slotsCounter] = Instantiate(_unitPrefab);
        }
        ApplyUnitToSlot(_otherUnits[_slotsCounter], _otherUnitsSlots[_slotsCounter]);
        ApplyProfileToUnit(profile, _otherUnits[_slotsCounter]);
        _slotsCounter++;
        if(_slotsCounter == _otherUnitsSlots.Length)
        {
            _slotsCounter = 0;
        }
    }

    private void ApplyUnitToSlot(Unit unit, Transform slot)
    {
        Transform unitTransform = unit.transform;
        unitTransform.SetParent(slot);
        unitTransform.localPosition = Vector3.zero;
        unitTransform.localRotation = Quaternion.identity;
    }
    private void ApplyProfileToUnit(Profile profile, Unit unit)
    {
        unit.UnitSkin.SetSkin("skin_elf_prefab");
    }
}
