using UnityEngine;
using UnityEngine.UI;

public class UITicket : MonoBehaviour
{
    [SerializeField] private RectTransform _secondPart;
    [SerializeField] private RectTransform _unlockedUI;
    [SerializeField] private RectTransform _anonimousUI;
    [SerializeField] private RectTransform _lockedUI;
    [SerializeField] private AspectRatioFitter _unlockedIconFitter;
    [SerializeField] private AspectRatioFitter _lockedIconFitter;
    [SerializeField] private Text _baseTicketText;
    [SerializeField] private Text _secondPartLockedText;
    [SerializeField] private Text _secondPartUnlockedText;
    [SerializeField] private Image _lockedIcon;
    [SerializeField] private Image _unlockedIcon;
    [SerializeField] private GameObject _selection;
    public Vector3 IconPosition => _unlockedIcon.transform.position;
    public UITicket SetIcon(Sprite sprite)
    {
        _lockedIcon.sprite = sprite;
        _unlockedIcon.sprite = sprite;
        _unlockedIconFitter.aspectRatio = (float)sprite.rect.width / (float)sprite.rect.height;
        _lockedIconFitter.aspectRatio = (float)sprite.rect.width / (float)sprite.rect.height;
        return this;
    }
    public UITicket SetHeader(string text)
    {
        _baseTicketText.text = text;
        return this;
    }
    public UITicket SetValue(string text)
    {
        _secondPartLockedText.text = text;
        _secondPartUnlockedText.text = text;
        return this;
    }
    public UITicket SetUnlocked()
    {
        _unlockedUI.gameObject.SetActive(true);
        _lockedUI.gameObject.SetActive(false);
        _anonimousUI.gameObject.SetActive(false);
        return this;
    }
    public UITicket SetLocked()
    {
        _unlockedUI.gameObject.SetActive(false);
        _lockedUI.gameObject.SetActive(true);
        _anonimousUI.gameObject.SetActive(false);
        return this;
    }
    public UITicket SetAnonimous()
    {
        _unlockedUI.gameObject.SetActive(false);
        _lockedUI.gameObject.SetActive(false);
        _anonimousUI.gameObject.SetActive(true);
        return this;
    }
    public UITicket DetachSecondPart(bool instantly)
    {
        if (instantly)
        {
            _secondPart.gameObject.SetActive(false);
        }
        else
        {

        }
        return this;
    }
    public UITicket SetSelection(bool selected)
    {
        _selection.SetActive(selected);
        return this;
    }
}
