using GLG.UI;
using UnityEngine;

public class BattleSearch_Screen : UIController
{
    public event System.Action onNext;

    public BattleSearch_Screen OnNext(System.Action callback)
    {
        onNext += callback;
        return this;
    }
}
