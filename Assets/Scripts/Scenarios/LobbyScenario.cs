using GLG;
using SimpleInputNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScenario : BaseScenario, IManaged
{
    private bool _startSearchBattle = false;
    private IEnumerator _updateLobbyParticipantsRoutine;

    private string _selectedCurrency;
    private float _selectedBet;
    private bool _readyToSearchBattle = false;
    private bool _isInitilized = false;
    private Unit _player;
    private Joystick _joystick;
    private bool _isRunning = false;

    public override bool IsError { get; protected set; }
    public override bool IsRunning { get => _isRunning; protected set { _isRunning = value; Debug.Log("IsRunning: " + _isRunning); } }
    public override string ErrorMessage { get; protected set; }

    private void Awake()
    {
        Kernel.RegisterManaged(this);
    }
    private void OnDestroy()
    {
        Kernel.UnregisterManaged(this);
    }

    public override void StartScenario()
    {
        if (IsRunning) return;
        IsRunning = true;
        Initilize();
        Kernel.UI.mainCamera.transform.SetPositionAndRotation(LevelContainer.Instance.CameraPoint.position, LevelContainer.Instance.CameraPoint.rotation);
        StartCoroutine(Scenario());
    }

    public override void StopScenario()
    {
        if (!IsRunning) return;
        IsRunning = false;
        StopCoroutine(Scenario());
        if (_updateLobbyParticipantsRoutine != null)
        {
            StopCoroutine(_updateLobbyParticipantsRoutine);
        }
        Dispose();
    }


    private IEnumerator Scenario()
    {
        UpdateVisualPlayerInfo();
        _updateLobbyParticipantsRoutine = UpdateLobbyParticipants_SubScenario();
        StartCoroutine(_updateLobbyParticipantsRoutine);
        yield return StartCoroutine(UpdateAvailableBets_SubScenario());
        Kernel.UI.ShowUI<LobbyOverlay>();
        while (!_readyToSearchBattle)
        {
            yield return null;
        }
        _readyToSearchBattle = false;

        IsRunning = false;
        Dispose();
        yield break;
    }

    private IEnumerator UpdateLobbyParticipants_SubScenario()// TOTEST
    {
        Web.Request request = Web.SendRequest("/api/lobby_participants", Web.RequestKind.Get);
        while (!request.IsComplete)
        {
            yield return null;
        }
        if (request.IsError)
        {
            OnError(request.Result);
        }
        else
        {
            SharedWebData.Instance.lastLobbyParticipants = Web.Parse<Lobby_participants>(request.Result);
            UpdateVisualLobbyParticipants();
        }
    }
    private IEnumerator UpdateAvailableBets_SubScenario()// TODO
    {
        // get bets here;
        BetsData[] availableBets = new BetsData[2];
        availableBets[0] =
            new BetsData()
            {
                currency = "ton",
                minCustomBet = 1f,
                maxCustomBet = 100f,
                availableBets = new float[4] { 1, 5, 10, 100 }
            };
        availableBets[1] =
            new BetsData()
            {
                currency = "praxis",
                minCustomBet = 1f,
                maxCustomBet = 100f,
                availableBets = new float[4] { 1, 5, 10, 100 }
            };
        Kernel.UI.Get<BetSelection_Screen>().ApplyData(availableBets);
        yield break;
    }

    private void UpdateVisualPlayerInfo()
    {
        Profile player = SharedWebData.Instance.playerProfile;
        Kernel.UI.Get<LobbyOverlay>().PlayerInfoBlock
        .SetAvatar(player.avatar)
        .SetXp(player.experience, player.new_level_threshold)
        .SetNick(player.identification)
        .SetPunk(player.praxis_balance)
        .SetTon(player.ton_balance)
        .SetLeagueLevel(player.level);
        LobbyUnitsSpawner spawner = LevelContainer.Instance.LobbyUnitsSpawner;
        spawner.SpawnPlayer(player);
        _player = spawner.Player;
    }
    private void UpdateVisualLobbyParticipants()
    {
        if (SharedWebData.Instance.lastLobbyParticipants.participants == null)
        {
            return;
        }
        List<Profile> participantsProfiles = new List<Profile>(SharedWebData.Instance.lastLobbyParticipants.participants);
        LobbyUnitsSpawner spawner = LevelContainer.Instance.LobbyUnitsSpawner;
        int unitsToSpawn = Mathf.Min(spawner.OtherSlotsCount, participantsProfiles.Count);

        for (int i = 0; i < unitsToSpawn; i++)
        {
            int randomIndex = Random.Range(0, participantsProfiles.Count);
            spawner.SpawnOtherUnit(participantsProfiles[randomIndex]);
            participantsProfiles.RemoveAt(randomIndex);
        }
    }
    private void OnError(string error)
    {
        Debug.Log("[LobbyScenario] ERROR: " + error);
    }


    private void Initilize()
    {
        if (_isInitilized) return;
        _isInitilized = true;
        Kernel.UI.Get<LobbyOverlay>().OnBattleSearchButton += SearchBattleButtonHandler;
        _joystick = Kernel.UI.ShowUI<JoystickOverlay>().joystick;
    }
    private void Dispose()
    {
        if (!_isInitilized) return;
        _isInitilized = false;
        Kernel.UI.Get<LobbyOverlay>().OnBattleSearchButton -= SearchBattleButtonHandler;
    }


    #region HANDLERS
    public void StartButtonHandler()
    {
        Kernel.UI.ShowUI<BetSelection_Screen>();
    }
    public void BetCurrencyChangedHandler(string currency)
    {
        _selectedCurrency = currency;
    }
    public void BetChangedHandler(float value)
    {
        _selectedBet = value;
    }
    public void SearchBattleButtonHandler()
    {
        Kernel.UI.ShowUI<BetSelection_Screen>()
            .OnBetSelected((string currency, float value) =>
            {
                _selectedCurrency = currency;
                _selectedBet = value;
                _readyToSearchBattle = true;
            })
            .OnClosed(() =>
            {
                _readyToSearchBattle = false;
            });
    }

    public void ManagedUpdate()
    {
        if (_player)
        {
            if (_joystick.IsPressed)
            {
                Vector3 dir = new Vector3(_joystick.Value.x, 0f, _joystick.Value.y);
                _player.UnitMovement.Move(dir);
            }
        }
    }
    #endregion
}
