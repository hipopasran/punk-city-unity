using GLG.UI;
using UnityEngine;

public class BattleResult_Screen : UIController
{
    public event System.Action onNext;

    public BattleResult_Screen OnNext(System.Action callback)
    {
        onNext += callback;
        return this;
    }
}
