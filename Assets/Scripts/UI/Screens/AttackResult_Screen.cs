using GLG.UI;
using UnityEngine;

public class AttackResult_Screen : UIController
{
    public event System.Action onNext;

    public AttackResult_Screen OnNext(System.Action callback)
    {
        onNext += callback;
        return this;
    }
}
