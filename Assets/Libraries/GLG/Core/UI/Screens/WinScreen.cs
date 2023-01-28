using GLG.UI;
using UnityEngine;

public class WinScreen : UIController
{
    public event System.Action onNext;

    [SerializeField] private TextBlock _levelBlock;
    [SerializeField] private TextBlock _enemiesDownBlock;
    [SerializeField] private TextBlock _rewardBlock;

    public TextBlock LevelBlock => _levelBlock;
    public TextBlock EnemiesDownBlock => _enemiesDownBlock;
    public TextBlock RewardBlock => _rewardBlock;

    public void NextButtonHandler()
    {
        onNext?.Invoke();
        onNext = null;
    }

    public WinScreen OnNext(System.Action callback)
    {
        onNext += callback;
        return this;
    }
}
