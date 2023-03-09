using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetParametersBlock : MonoBehaviour
{
    public event System.Action<string, float> onBetSelected;

    [SerializeField] private string _currency;
    [Header("Predefined bets")]
    [SerializeField] private string _predefinedBetTextPrefix;
    [SerializeField] private string _predefinedBetTextPostfix;
    [SerializeField] private GameObject _predefinedBetPrefab;
    [SerializeField] private Transform _predefinedBetsParent;
    [Header("Custom bet")]
    [SerializeField] private string _customBetPrefix;
    [SerializeField] private string _custombetPostfix;
    [SerializeField] private Slider _customBetSlider;
    [SerializeField] private TMP_Text _customBetText;
    [SerializeField] private Button _customBetConfirmButton;
    
    private List<Button> _predefindeBetButtons = new List<Button>();
    private float _selectedBet = -1f;

    public string Currency => _currency;

    private void Awake()
    {
        _customBetSlider.onValueChanged.AddListener(CustomBetSliderValueChangedHandler);
        _customBetConfirmButton.onClick.AddListener(CustomBetConfirmHandler);
    }
    private void OnDestroy()
    {
        _customBetSlider.onValueChanged.RemoveListener(CustomBetSliderValueChangedHandler);
        _customBetConfirmButton.onClick.RemoveListener(CustomBetConfirmHandler);
    }

    public BetParametersBlock ApplyBetsData(BetsData betsData)
    {
        ClearPredefinedBetsButtons();
        foreach (var item in betsData.availableBets)
        {
            AddPredefinedBet(item);
        }
        SetMinCustomBet(betsData.minCustomBet);
        SetMaxCustomBet(betsData.maxCustomBet);
        return this;
    }
    public BetParametersBlock AddPredefinedBet(float bet)
    {
        GameObject button = Instantiate(_predefinedBetPrefab, _predefinedBetsParent, false);
        _predefindeBetButtons.Add(button.GetComponentInChildren<Button>());
        _predefindeBetButtons[_predefindeBetButtons.Count - 1].onClick.AddListener(() => { PredefinedBetSelectedHandler(bet); });
        button.GetComponentInChildren<TMP_Text>().text = _predefinedBetTextPrefix + bet + _predefinedBetTextPostfix;
        return this;
    }
    public BetParametersBlock ClearPredefinedBetsButtons()
    {
        foreach (var item in _predefindeBetButtons)
        {
            Destroy(item.gameObject);
        }
        _predefindeBetButtons.Clear();
        return this;
    }
    public BetParametersBlock SetMinCustomBet(float minCustomBet)
    {
        _customBetSlider.minValue = minCustomBet;
        return this;
    }
    public BetParametersBlock SetMaxCustomBet(float maxCustomBet)
    {
        _customBetSlider.maxValue = maxCustomBet;
        return this;
    }

    private void PredefinedBetSelectedHandler(float bet)
    {
        _selectedBet = bet;
        onBetSelected?.Invoke(_currency, _selectedBet);
    }
    private void CustomBetSliderValueChangedHandler(float value)
    {
        _selectedBet = value;
        _customBetText.text = _customBetPrefix + value + _custombetPostfix;
    }
    private void CustomBetConfirmHandler()
    {
        onBetSelected?.Invoke(_currency, _selectedBet);
    }
}