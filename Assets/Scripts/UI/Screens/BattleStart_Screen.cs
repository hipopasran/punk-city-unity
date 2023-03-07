using GLG.UI;
using TMPro;
using UnityEngine;

public class BattleStart_Screen : UIController
{
    [SerializeField] private PlayerInfoBlock _playerBlock;
    [SerializeField] private PlayerInfoBlock _enemyBlock;
    [SerializeField] private TMP_Text _txtTimer;

    public PlayerInfoBlock Player => _playerBlock;
    public PlayerInfoBlock Enemy => _enemyBlock;

    public BattleStart_Screen SetTimerValue(int seconds)
    {
        _txtTimer.text = "До начала " + seconds;
        return this;
    }
    public BattleStart_Screen SetEnemy(Profile profile)
    {

        return this;
    }
    public BattleStart_Screen SetPlayer(Profile profile)
    {

        return this;
    }
}
