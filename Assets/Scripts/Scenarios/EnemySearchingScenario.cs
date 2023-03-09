using GLG;
using System.Collections;


public class EnemySearchingScenario : BaseScenario
{
    private bool _isInitialized;
    private IEnumerator _scenarioRoutine;

    public override bool IsError { get; protected set; }
    public override bool IsRunning { get; protected set; }
    public override string ErrorMessage { get; protected set; }

    public override void StartScenario()
    {
        if (IsRunning) return;
        IsRunning = true;
        Initilize();
        _scenarioRoutine = Scenario();
        StartCoroutine(_scenarioRoutine);
    }
    public override void StopScenario()
    {
        if (!IsRunning) return;
        IsRunning = false;
        Dispose();
        if(_scenarioRoutine != null)
        {
            StopCoroutine(_scenarioRoutine);
        }    
    }
    private void Initilize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
    }
    private void Dispose()
    {
        if (!_isInitialized) return;
        _isInitialized = false;
    }

    private IEnumerator Scenario()
    {
        SharedWebData.Instance.lastEnemyProfile = new Profile()
        {
            avatar = null,
            experience = 100,
            id = 1,
            identification = "@test_player_1",
            level = 2,
            new_level_threshold = 300,
            praxis_balance = 100,
            profile_url = null,
            ton_balance = 100
        };
        IsRunning = false;
        yield break;
    }
}
