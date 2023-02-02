using GLG.UI;
using UnityEngine;

public class ResumeSession_Overlay : UIController
{
    public event System.Action onNext;

    public ResumeSession_Overlay OnNext(System.Action callback)
    {
        onNext += callback;
        return this;
    }
}
