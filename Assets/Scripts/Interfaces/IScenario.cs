public interface IScenario
{
    public bool IsComplete { get; }
    public bool IsError { get; }
    public bool IsRunning { get; }
    public string ErrorMessage { get; }
    public void StartScenario();
    public void StopScenario();
}
