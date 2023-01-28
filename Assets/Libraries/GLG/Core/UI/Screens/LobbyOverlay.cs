using GLG;
using GLG.UI;
using TMPro;
using UnityEngine;

public class LobbyOverlay : UIController
{
    public event System.Action onGameStarted;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private MoneyBlock _moneyBlock;

    public MoneyBlock MoneyBlock => _moneyBlock;

    public int Level
    {
        set => _levelText.text = "Level " + value;
    }

    private void Awake()
    {
        PlayerMoney.onUpdateMoney += MoneyChangedHandler;
        _moneyBlock.Money = Kernel.Economic.PlayerMoney.Money;
    }
    private void Start()
    {
        _moneyBlock.RefreshLayout();
    }
    private void OnDestroy()
    {
        PlayerMoney.onUpdateMoney += MoneyChangedHandler;
    }

    public void StartGameHandler()
    {
        onGameStarted?.Invoke();
    }

    private void MoneyChangedHandler(int money)
    {
        _moneyBlock.Money = money;
    }
}
