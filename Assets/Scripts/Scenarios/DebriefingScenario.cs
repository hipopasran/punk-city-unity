using GLG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DebriefingScenario : BaseScenario
{
    private bool _isInitialized = false;
    private IEnumerator _scenarioRoutine;

    public override bool IsError { get; protected set; }
    public override bool IsRunning { get; protected set; }
    public override string ErrorMessage { get; protected set; }

    private void OnDestroy()
    {
        Dispose();
    }

    public override void StartScenario()
    {
        if (IsRunning) return;
        IsRunning = true;
        Initilize();
        if (_scenarioRoutine != null)
        {
            StopCoroutine(_scenarioRoutine);
        }
        _scenarioRoutine = Scenario();
        StartCoroutine(_scenarioRoutine);
    }
    public override void StopScenario()
    {
        if (!IsRunning) return;
        IsRunning = false;
        if (_scenarioRoutine != null)
        {
            StopCoroutine(_scenarioRoutine);
        }
        Dispose();
    }


    private IEnumerator Scenario()
    {
        //Kernel.UI.ShowUI<>();
        yield break;
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
}
