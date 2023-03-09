using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    public event System.Action<string> onClicked;

    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _quantityText;
    [SerializeField] private TMP_Text _middleText;
    [SerializeField] private TMP_Text _bottomText;

    private CardData _relatedCardData;

    public Sprite Icon { get => _icon.sprite; set => _icon.sprite = value; }
    public CardData RelatedCardData => _relatedCardData;
    public string QuantityText { get => _quantityText.text; set => _quantityText.text = value; }
    public string MiddleText { get => _middleText.text; set => _middleText.text = value; }
    public string BottomText { get => _bottomText.text; set => _bottomText.text = value; }
    public bool QuantityTextVisible { get => _quantityText.gameObject.activeSelf; set => _quantityText.gameObject.SetActive(value); }
    public bool MiddleTextVisible { get => _middleText.gameObject.activeSelf; set => _middleText.gameObject.SetActive(value); }
    public bool BottomTextVisible { get => _bottomText.gameObject.activeSelf; set => _bottomText.gameObject.SetActive(value); }

    private void Awake()
    {
        _button.onClick.AddListener(ClickHandler);
    }
    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }


    public UICard ApplyCardData(CardData cardData)
    {
        _relatedCardData = cardData;
        QuantityText = "";
        MiddleText = "";
        BottomText = cardData.displayName;
        return this;
    }

    private void ClickHandler()
    {
        onClicked?.Invoke(RelatedCardData.id);
    }

    private void Dispose()
    {
        onClicked = null;
    }
}
