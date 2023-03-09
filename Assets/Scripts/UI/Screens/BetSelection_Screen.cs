using GLG.UI;
using UnityEngine;
using UnityEngine.UI;

public class BetsData
{
    public string currency;
    public float[] availableBets;
    public float minCustomBet;
    public float maxCustomBet;
}

public class BetSelection_Screen : UIController
{
    private event System.Action _onClosed;
    private event System.Action<string , float> _onBetSelected;

    [SerializeField] private Button _tonBlockSelector;
    [SerializeField] private Button _praxisBetSelector;
    [SerializeField] private BetParametersBlock[] _betsBlocks;
    [SerializeField] private Button _btnClose;

    private string _selecedCurrency;
    private float _selectedBet;

    private void Start()
    {
        _btnClose.onClick.AddListener(CloseButtonHandler);
        _tonBlockSelector.onClick.AddListener(TonSelectedHandler);
        _praxisBetSelector.onClick.AddListener(PraxisSelectedHandler);
        foreach (var item in _betsBlocks)
        {
            item.onBetSelected += ApplyBetHandler;
        }
    }
    private void OnDestroy()
    {
        _btnClose.onClick.RemoveAllListeners();
        _tonBlockSelector.onClick.RemoveListener(TonSelectedHandler);
        _praxisBetSelector.onClick.RemoveListener(PraxisSelectedHandler);
        foreach (var item in _betsBlocks)
        {
            item.onBetSelected -= ApplyBetHandler;
        }
    }

    protected override void OnStartShow()
    {
        base.OnStartShow();
        _betsBlocks[0].gameObject.SetActive(true);
        _betsBlocks[1].gameObject.SetActive(false);
    }

    public BetSelection_Screen ApplyData(BetsData[] betsData)
    {
        foreach (var betData in betsData)
        {
            foreach (var betBlock in _betsBlocks)
            {
                if(betBlock.Currency == betData.currency)
                {
                    betBlock.ApplyBetsData(betData);
                }
            }
        }
        return this;
    }
    public BetSelection_Screen OnBetSelected(System.Action<string, float> callback)
    {
        _onBetSelected += callback;
        return this;
    }
    public BetSelection_Screen OnClosed(System.Action callback)
    {
        _onClosed += callback;
        return this;
    }

    private void ApplyBetHandler(string currency, float value)
    {
        _selecedCurrency = currency;
        _selectedBet = value;
        _onBetSelected?.Invoke(_selecedCurrency, _selectedBet);
        _onBetSelected = null;
    }
    private void CloseButtonHandler()
    {
        Hide();
        _onClosed?.Invoke();
        _onClosed = null;
    }

    private void TonSelectedHandler()
    {
        foreach (var item in _betsBlocks)
        {
            if(item.Currency == "ton")
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
    }
    private void PraxisSelectedHandler()
    {
        foreach (var item in _betsBlocks)
        {
            if (item.Currency == "praxis")
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
