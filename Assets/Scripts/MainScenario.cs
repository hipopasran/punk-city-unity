using GLG;
using System.Collections;
using UnityEngine;

public enum GameState { Initialization, Lobby, EnemySearching, Briefing, Game, Debriefing }

public class MainScenario : MonoBehaviour
{
    [SerializeField] private InitializationScenario _initializationScenario;
    [SerializeField] private bool _autoStartScenario = true;
    [SerializeField] private GameState _startState = GameState.Initialization;

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
        while (true)
        {
            switch (_currentState)
            {

                case GameState.Initialization:
                    Debug.Log("=====     INITIALIZATION SCENARIO    ===== ");
                    _initializationScenario.StartScenario();
                    while (_initializationScenario.IsRunning)
                    {
                        yield return null;
                    }
                    ChangeState(GameState.Lobby);
                    break;


                case GameState.Lobby:
                    Debug.Log("=====     LOBBY SCENARIO    ===== ");
                    Kernel.LevelsManager.UnloadCurrentAndLoad("Lobby");
                    while (Kernel.LevelsManager.IsLoading) yield return null;
                    ChangeState(GameState.EnemySearching);
                    LobbyScenario lobbyScenario = LevelContainer.Instance.MainScenario as LobbyScenario;
                    lobbyScenario.StartScenario();
                    while (lobbyScenario.IsRunning)
                    {
                        yield return null;
                    }
                    break;


                case GameState.EnemySearching:
                    Debug.Log("=====     EnemySearching SCENARIO     ===== ");
                    yield return StartCoroutine(EnemySearchingScenario());
                    ChangeState(GameState.Briefing);
                    break;


                case GameState.Briefing:
                    Debug.Log("=====     Briefing SCENARIO     ===== ");
                    Kernel.LevelsManager.UnloadCurrentAndLoad("Game");
                    while (Kernel.LevelsManager.IsLoading) yield return null;
                    yield return StartCoroutine(BriefingScenario());
                    ChangeState(GameState.Game);
                    break;


                case GameState.Game:
                    Debug.Log("=====     Game SCENARIO     ===== ");
                    GameScenario gameScenario = LevelContainer.Instance.MainScenario as GameScenario;
                    gameScenario.StartScenario();
                    ChangeState(GameState.Debriefing);
                    while (gameScenario.IsRunning)
                    {
                        yield return null;
                    }
                    break;


                case GameState.Debriefing:
                    Debug.Log("=====     Debriefing SCENARIO     ===== ");
                    //Kernel.LevelsManager.UnloadCurrentAndLoad("Game");
                    yield return StartCoroutine(DebriefingScenario());
                    StartCoroutine(Scenario(GameState.Lobby));
                    break;


                default:
                    break;
            }
            yield return null;
        }
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
