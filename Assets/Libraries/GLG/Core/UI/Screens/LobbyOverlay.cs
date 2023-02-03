using GLG;
using GLG.UI;
using UnityEngine;
using UnityEngine.UI;

public class LobbyOverlay : UIController
{
    public System.Action OnBattleSearchButton;

    [SerializeField] private PlayerInfoBlock _playerInfoBlock;
    [SerializeField] private Button _battleSearchButton;


    public PlayerInfoBlock PlayerInfoBlock => _playerInfoBlock;


    private void Awake()
    {
        _battleSearchButton.onClick.AddListener(BattleSearchButtonHandler);
    }
    private void OnDestroy()
    {
        _battleSearchButton.onClick.RemoveAllListeners();
    }

    public void SetBattleSearchButtonInteractible(bool interactible)
    {
        _battleSearchButton.interactable = interactible;
    }

    private void BattleSearchButtonHandler()
    {
        OnBattleSearchButton?.Invoke();
    }
}
