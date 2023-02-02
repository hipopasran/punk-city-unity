using GLG.UI;
using UnityEngine;

public class Battle_Screen : UIController
{
    public event System.Action onNext;

    public Battle_Screen OnNext(System.Action callback)
    {
        onNext += callback;
        return this;
    }
}
