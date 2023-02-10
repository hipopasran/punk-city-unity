using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectCard : MonoBehaviour
{
    [SerializeField] private Image _imgIcon;
    [SerializeField] private TMP_Text _txtDescription;

    public Sprite Icon { get => _imgIcon.sprite; set => _imgIcon.sprite = value; }
    public string Description { get => _txtDescription.text; set => _txtDescription.text = value; }
}
