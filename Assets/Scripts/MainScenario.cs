using GLG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Initialization, Lobby, EnemySearching, Briefing, Game, Debriefing}

public class MainScenario : MonoBehaviour
{
    [SerializeField] private bool _autoStartScenario = true;
    [SerializeField] private GameState _startState = GameState.Initialization;


    private Profile _user;
    private Profile _enemy;
    private GameState _currentState;
    private GameState _prevState = GameState.Initialization;

    void Start()
    {
        if (_autoStartScenario)
        {
            StartCoroutine(Scenario(_startState));
        }
    }


    private IEnumerator Scenario(GameState gameState)
    {
        ChangeState(gameState);
        yield return null;
        switch (gameState)
        {

            case GameState.Initialization:
                //yield return StartCoroutine(InitializationScenario());
                ChangeState(GameState.Lobby);
                break;


            case GameState.Lobby:
                //yield return StartCoroutine(LobbyScenario());
                ChangeState(GameState.EnemySearching);
                break;


            case GameState.EnemySearching:
                yield return StartCoroutine(EnemySearchingScenario());
                ChangeState(GameState.Briefing);
                break;


            case GameState.Briefing:
                yield return StartCoroutine(BriefingScenario());
                ChangeState(GameState.Game);
                break;


            case GameState.Game:
                yield return StartCoroutine(GameScenario());
                ChangeState(GameState.Debriefing);
                break;


            case GameState.Debriefing:
                yield return StartCoroutine(DebriefingScenario());
                StartCoroutine(Scenario(GameState.Lobby));
                break;


            default:
                break;
        }
        yield break;
    }

    private void ChangeState(GameState nextState)
    {
        _prevState = _currentState;
        _currentState = nextState;
    }

    //--------------------------------------------------------------------------------------------------------------------
    private IEnumerator EnemySearchingScenario()
    {

        yield break;
    }



    //--------------------------------------------------------------------------------------------------------------------
    private IEnumerator BriefingScenario()
    {

        yield break;
    }



    //--------------------------------------------------------------------------------------------------------------------
    private IEnumerator GameScenario()
    {

        yield break;
    }




    //--------------------------------------------------------------------------------------------------------------------
    private IEnumerator DebriefingScenario()
    {

        yield break;
    }



    //--------------------------------------------------------------------------------------------------------------------
    private void OnError(string message)
    {

    }
}
