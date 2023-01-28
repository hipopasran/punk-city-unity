using GLG.UI;
using UnityEngine;

public class FailScreen : UIController
{
    public event System.Action onNext;

    [SerializeField] private TextBlock _levelBlock;
    [SerializeField] private TextBlock _enemiesDownBlock;
    [SerializeField] private TextBlock _rewardBlock;

    public TextBlock LevelBlock => _levelBlock;
    public TextBlock EnemiesDownBlock => _enemiesDownBlock;
    public TextBlock RewardBlock => _rewardBlock;

    public void PlayButtonHandler()
    {
        onNext?.Invoke();
        onNext = null;
    }

    public FailScreen OnNext(System.Action callback)
    {
        onNext += callback;
        return this;
    }
}
