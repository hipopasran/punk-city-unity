using GLG.UI;
using UnityEngine;

public class BattleStart_Screen : UIController
{
    public event System.Action onNext;


    public BattleStart_Screen OnNext(System.Action callback)
    {
        onNext += callback;
        return this;
    }
}
