using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoBlock : MonoBehaviour
{
    [SerializeField] private Image _imgAvatar;
    [SerializeField] private TMP_Text _txtNick;
    [SerializeField] private TextBlock _leagueBlock;
    [SerializeField] private TextBlock _xpBlock;
    [SerializeField] private TextBlock _tonBlock;
    [SerializeField] private TextBlock _punkBlock;

    public Image Avatar => _imgAvatar;
    public TextBlock LeagueBlock => _leagueBlock;
    public TextBlock XPBlock => _xpBlock;
    public TextBlock TonBlock => _tonBlock;
    public TextBlock PunkBlock => _punkBlock;

    public PlayerInfoBlock SetAvatar(Sprite sprite)
    {
        _imgAvatar.sprite = sprite;
        return this;
    }
    public PlayerInfoBlock SetNick(string value)
    {
        _txtNick.text = value;
        return this;
    }
    public PlayerInfoBlock SetLeagueLevel(int value)
    {
        _leagueBlock.Value = value.ToString();
        return this;
    }
    public PlayerInfoBlock SetXp(int value)
    {
        _xpBlock.Value = value.ToString();
        return this;
    }
    public PlayerInfoBlock SetTon(int value)
    {
        _tonBlock.Value = value.ToString();
        return this;
    }
    public PlayerInfoBlock SetPunk(int value)
    {
        _punkBlock.Value = value.ToString();
        return this;
    }
}
