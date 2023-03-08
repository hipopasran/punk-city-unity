using GLG;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameScenario : BaseScenario
{
    [SerializeField] private int _environmentsCount = 4;
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private Transform _playerPoint;
    [SerializeField] private Transform _enemyPoint;
    [SerializeField] private Transform _mapContainer;
    [SerializeField] private Transform _playerJumpPoint;
    [SerializeField] private Transform _enemyJumpPoint;

    private Unit _player;
    private Unit _enemy;
    private Profile _playerProfile;
    private Profile _enemyProfile;
    private Battle_Screen _battleScreen;
    private IEnumerator _scenarioRoutine;
    private bool _isInitialized = false;


    public override bool IsError { get; protected set; }
    public override bool IsRunning { get; protected set; }
    public override string ErrorMessage { get; protected set; }

    private void OnDestroy()
    {
        Dispose();
    }

    public override void StartScenario()
    {
        if (IsRunning) return;
        IsRunning = true;
        Initilize();
        if (_scenarioRoutine != null)
        {
            StopCoroutine(_scenarioRoutine);
        }
        _scenarioRoutine = Scenario();
        StartCoroutine(_scenarioRoutine);
    }
    public override void StopScenario()
    {
        if (!IsRunning) return;
        IsRunning = false;
        if (_scenarioRoutine != null)
        {
            StopCoroutine(_scenarioRoutine);
        }
        Dispose();
    }


    private IEnumerator Scenario()
    {
        // initialization
        SetPlayer(SharedWebData.Instance.playerProfile);
        if (SharedWebData.Instance.lastEnemyProfile != null)
        {
            SetEnemy(SharedWebData.Instance.lastEnemyProfile);
        }
        yield return StartCoroutine(LoadEnvironment());
        Kernel.UI.Get<LoadingOverlay>().Hide();
        // intro
        yield return StartCoroutine(BattleStart_Subscenario());
        // battle start
        Kernel.UI.ShowUI<Battle_Screen>();
        yield return StartCoroutine(SetCards());

        while (true)
        {
            yield return null;
        }


        IsRunning = false;
        Dispose();
        yield break;
    }

    private IEnumerator LoadEnvironment()
    {
        AsyncOperationHandle<GameObject> asyncOperation = Addressables.LoadAssetAsync<GameObject>("environment_" + Random.Range(1, _environmentsCount + 1) + "_prefab");
        while (!asyncOperation.IsDone)
        {
            yield return null;
        }
        Instantiate(asyncOperation.Result, _mapContainer);
    }
    private IEnumerator SetCards()
    {
        _battleScreen.ClearCards();
        foreach (var item in EntitiesRegistry.i.Cards)
        {
            _battleScreen.AddCard(item);
            yield return new WaitForSeconds(0.2f);
        }
        yield break;
    }
    private IEnumerator BattleStart_Subscenario()
    {
        BattleStart_Screen battleStart_Screen = Kernel.UI.ShowUI<BattleStart_Screen>();
        int secondsBeforeStart = 3;
        battleStart_Screen
            .SetPlayer(_playerProfile)
            .SetEnemy(_enemyProfile);
        for (int i = 0; i < secondsBeforeStart; i++)
        {
            battleStart_Screen.SetTimerValue(secondsBeforeStart - i);
            yield return new WaitForSeconds(1f);
        }
        battleStart_Screen.Hide();
        yield break;
    }
    private IEnumerator Battle_Subscenario()
    {
        yield break;
    }

    private void Initilize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        _battleScreen = Kernel.UI.Get<Battle_Screen>();
        _playerProfile = SharedWebData.Instance.playerProfile;
        _enemyProfile = SharedWebData.Instance.lastEnemyProfile;
    }
    private void Dispose()
    {
        if (!_isInitialized) return;
        _isInitialized = false;
        Addressables.ReleaseInstance(_mapContainer.GetChild(0).gameObject);
    }

    private void SetMap(string mapName)
    {

    }

    private void SetPlayer(Profile profile)
    {
        if (_player == null)
        {
            _player = Instantiate(_unitPrefab);
        }
        ApplyUnitToSlot(_player, _playerPoint);
        ApplyProfileToUnit(profile, _player);
        _player.UnitAnimator.JumpPoint = _playerJumpPoint;
    }
    private void SetEnemy(Profile profile)
    {
        if (_enemy == null)
        {
            _enemy = Instantiate(_unitPrefab);
        }
        ApplyUnitToSlot(_enemy, _enemyPoint);
        ApplyProfileToUnit(profile, _enemy);
        _enemy.UnitAnimator.JumpPoint = _enemyJumpPoint;
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


    private void WeaponSelectedHandler(CardData cardData)
    {
        _player.UnitAttack
        .OnWeaponEquipped(() =>
        {
            _player.UnitAttack.DoAttack();
        })
        .OnAttackCompleted(() =>
        {

        })
        .EquipWeapon(cardData.id);
    }
}
