using GLG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScenario : BaseScenario
{
    private bool _canSearchBattle = true;
    private bool _startSearchBattle = false;
    private IEnumerator _updateLobbyParticipantsRoutine;

    public override bool IsError { get; protected set; }
    public override bool IsRunning { get; protected set; }
    public override string ErrorMessage { get; protected set; }

    public override void StartScenario()
    {
        if (IsRunning) return;
        IsRunning = true;
        Kernel.UI.mainCamera.transform.SetPositionAndRotation(LevelContainer.Instance.CameraPoint.position, LevelContainer.Instance.CameraPoint.rotation);
        Kernel.CoroutinesObject.StartCoroutine(Scenario());
    }

    public override void StopScenario()
    {
        if (!IsRunning) return;
        IsRunning = false;
        Kernel.CoroutinesObject.StopCoroutine(Scenario());
        if (_updateLobbyParticipantsRoutine != null)
        {
            Kernel.CoroutinesObject.StopCoroutine(_updateLobbyParticipantsRoutine);
        }
    }


    private IEnumerator Scenario()
    {
        UpdateVisualPlayerInfo();
        _updateLobbyParticipantsRoutine = UpdateLobbyParticipants_SubScenario();
        Kernel.CoroutinesObject.StartCoroutine(_updateLobbyParticipantsRoutine);
        Kernel.UI.ShowUI<LobbyOverlay>();
        while (!_startSearchBattle)
        {
            yield return null;
        }
        IsRunning = false;
        yield break;
    }

    private IEnumerator UpdateLobbyParticipants_SubScenario()
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
    }
    private void UpdateVisualLobbyParticipants()
    {
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
        IsError = true;
        IsRunning = false;
        ErrorMessage = error;
        _canSearchBattle = false;
        StopScenario();
    }

    public void StartButtonHandler()
    {
        if (!_canSearchBattle) return;
        _startSearchBattle = true;
    }
}
