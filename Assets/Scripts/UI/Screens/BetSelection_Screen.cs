using GLG.UI;
using TMPro;
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

    [SerializeField] private Button[] _availableBetButtons;
    [SerializeField] private TMP_Text[] _availableBetTexts;
    [SerializeField] private Slider _sliderCustomBet;
    [SerializeField] private Button _btnSetCustomBet;
    [SerializeField] private TMP_Text _txtCustomBet;
    [SerializeField] private Button _btnClose;

    private BetsData _betsData;
    private string _selecedCurrency;
    private float _selectedBet;

    private void Start()
    {
        for (int i = 0; i < _availableBetButtons.Length; i++)
        {
            _availableBetTexts[i].text = '$' + _betsData.availableBets[i].ToString();
            _availableBetButtons[i].onClick.AddListener(() => BetSelectedButtonHandler(_betsData.availableBets[i]));
        }
        _btnSetCustomBet.onClick.AddListener(() => CustomBetSelectedButtonHandler(Mathf.Lerp(_betsData.minCustomBet, _betsData.maxCustomBet, _sliderCustomBet.value)));
        _sliderCustomBet.onValueChanged.AddListener(CustomBetSliderValueChangedHandler);
        _btnClose.onClick.AddListener(CloseButtonHandler);
    }
    private void OnDestroy()
    {
        for (int i = 0; i < _availableBetButtons.Length; i++)
        {
            _availableBetButtons[i].onClick.RemoveAllListeners();
        }
        _btnSetCustomBet.onClick.RemoveAllListeners();
        _sliderCustomBet.onValueChanged.RemoveAllListeners();
        _btnClose.onClick.RemoveAllListeners();
    }

    public BetSelection_Screen ApplyData(BetsData betsData)
    {

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

    private void BetCyrrencyChanged(string currency)
    {
        _selecedCurrency = currency;
    }
    private void BetSelectedButtonHandler(float bet)
    {
        _selectedBet = bet;
    }
    private void CustomBetSelectedButtonHandler(float bet)
    {
        _selectedBet = bet;
    }
    private void CustomBetSliderValueChangedHandler(float value)
    {
        _txtCustomBet.text = "—¬Œﬂ —“¿¬ ¿: $" + Mathf.Lerp(_betsData.minCustomBet, _betsData.maxCustomBet, _sliderCustomBet.value).ToString();
    }
    private void ApplyBetHandler()
    {
        _onBetSelected?.Invoke(_selecedCurrency, _selectedBet);
        _onBetSelected = null;
    }
    private void CloseButtonHandler()
    {
        Hide();
        _onClosed?.Invoke();
        _onClosed = null;
    }
}
