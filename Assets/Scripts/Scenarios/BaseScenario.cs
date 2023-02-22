using UnityEngine;

public abstract class BaseScenario : MonoBehaviour, IScenario
{
    public abstract bool IsError { get; protected set; }

    public abstract bool IsRunning { get; protected set; }

    public abstract string ErrorMessage { get; protected set; }

    public abstract void StartScenario();

    public abstract void StopScenario();
}
