using GLG;
using UnityEngine;

public class LevelController : MonoBehaviour, IManaged
{
    private InGameScreen _ingameScreen;
    private LobbyOverlay _lobbyOverlay;

    #region UNITY CALLS
    private void Awake()
    {
        Kernel.RegisterManaged(this);
        _ingameScreen = Kernel.UI.Get<InGameScreen>();
        _lobbyOverlay = Kernel.UI.Get<LobbyOverlay>();
        
        AddHandlers();
        
    }
    private void OnDestroy()
    {
        Kernel.UnregisterManaged(this);
        RemoveHandlers();
    }
    #endregion

    #region PRIVATE METHODS
    private void AddHandlers()
    {
        
    }
    private void RemoveHandlers()
    {
        
    }
    private void Win()
    {
        _ingameScreen.Hide();
        WinScreen winScreen = Kernel.UI.ShowUI<WinScreen>();
        winScreen.LevelBlock.Value = Kernel.LevelsManager.CurrentDisplayLevelIndex.ToString();
        winScreen.OnNext(Kernel.LevelsManager.NextLevel);
        Kernel.UI.HideUI<JoystickOverlay>().joystick.ForceStop();
    }
    private void Fail()
    {
        _ingameScreen.Hide();
        FailScreen failScreen = Kernel.UI.ShowUI<FailScreen>();
        failScreen.LevelBlock.Value = Kernel.LevelsManager.CurrentDisplayLevelIndex.ToString();
        failScreen.OnNext(Kernel.LevelsManager.RestartLevel);
        Kernel.UI.HideUI<JoystickOverlay>().joystick.ForceStop();
    }
    
    #endregion

    #region PUBLIC METHODS
    public void ManagedUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            Win();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Fail();
        }
#endif
    }
    #endregion

    #region HANDLERS
    private void GameStartedHandler()
    {
        _lobbyOverlay.Hide();
        Kernel.UI.ShowUI<InGameScreen>();
        Kernel.UI.ShowUI<JoystickOverlay>();
    }
    #endregion
}