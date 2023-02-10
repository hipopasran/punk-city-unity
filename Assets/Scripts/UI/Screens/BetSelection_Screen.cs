using GLG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetsData
{
    public float[] availableBets;
    public float minCustomBet;
    public float maxCustomBet;
}

public class BetSelection_Screen : UIController
{
    public event System.Action onCloseButtonClicked;
    public event System.Action<float> onBetSelected;

    [SerializeField] private Button[] _availableBetButtons;
    [SerializeField] private TMP_Text[] _availableBetTexts;
    [SerializeField] private Slider _sliderCustomBet;
    [SerializeField] private Button _btnSetCustomBet;
    [SerializeField] private TMP_Text _txtCustomBet;
    [SerializeField] private Button _btnClose;

    private BetsData _betsData;

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

    private void BetSelectedButtonHandler(float bet)
    {
        onBetSelected?.Invoke(bet);
    }
    private void CustomBetSelectedButtonHandler(float bet)
    {
        onBetSelected?.Invoke(bet);
    }
    private void CustomBetSliderValueChangedHandler(float value)
    {
        _txtCustomBet.text = "ÑÂÎß ÑÒÀÂÊÀ: $" + Mathf.Lerp(_betsData.minCustomBet, _betsData.maxCustomBet, _sliderCustomBet.value).ToString();
    }
    private void CloseButtonHandler()
    {
        onCloseButtonClicked?.Invoke();
    }
}
