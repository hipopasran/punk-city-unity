using GLG.UI;
using UnityEngine;

public class BetSelection_Screen : UIController
{
    public event System.Action onNext;


    public BetSelection_Screen OnNext(System.Action callback)
    {
        onNext += callback;
        return this;
    }
}
