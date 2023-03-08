using GLG;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{
    public event System.Action<(int levelIndex, string sceneName)> onLevelUnloaded;
    public event System.Action<(int levelIndex, string sceneName)> onLevelLoaded;

    [SerializeField] private LevelsPreset _levelsPreset;
    [SerializeField] private bool _autoLoadLastLevel = true;
    public int CurrentLevelIndex { get; private set; } = 0;
    public int CurrentDisplayLevelIndex { get; private set; } = 1;
    public int PrevLevelIndex { get; private set; }
    public int PrevDisplayLevelIndex { get; private set; }
    public string CurrentScene { get; private set; }
    public string PrevScene { get; private set; }
    public bool IsLoading { get; private set; }

    private void Start()
    {
        CurrentLevelIndex = PlayerPrefs.GetInt("levelIndex", 0);
        CurrentDisplayLevelIndex = PlayerPrefs.GetInt("displayLevelIndex", 1);
        if(_autoLoadLastLevel) UnloadCurrentAndLoad(CurrentLevelIndex, false, true);
    }
    public void NextLevel(bool autoShowLobby = true)
    {
        UnloadCurrentAndLoad(CurrentLevelIndex + 1, true, autoShowLobby);
    }
    public void RestartLevel(bool autoShowLobby = true)
    {
        UnloadCurrentAndLoad(CurrentLevelIndex, false, autoShowLobby);
    }
    public void UnloadCurrentAndLoad(string sceneName, bool incrementDisplayLevelIndex = true, bool autoShowLobby = true)
    {
        int index = 0;
        for (int i = 0; i < _levelsPreset.levels.Length; i++)
        {
            LevelData item = _levelsPreset.levels[i];
            if (item.sceneName == sceneName)
            {
                index = i;
                break;
            }
        }
        UnloadCurrentAndLoad(index, incrementDisplayLevelIndex, autoShowLobby);
    }
    public void UnloadCurrentAndLoad(int levelIndex, bool incrementDisplayLevelIndex = true, bool autoShowLobby = true)
    {
        PrevLevelIndex = CurrentLevelIndex;
        PrevScene = CurrentScene;
        CurrentLevelIndex = levelIndex;
        if (incrementDisplayLevelIndex)
        {
            CurrentDisplayLevelIndex++;
        }
        if (CurrentLevelIndex > _levelsPreset.levels.Length - 1)
        {
            CurrentLevelIndex = 0;
        }
        PlayerPrefs.SetInt("levelIndex", CurrentLevelIndex);
        PlayerPrefs.SetInt("displayLevelIndex", CurrentDisplayLevelIndex);
        CurrentScene = _levelsPreset.levels[CurrentLevelIndex].sceneName;
        StartCoroutine(LoadScene(CurrentScene));
    }

    IEnumerator LoadScene(string name, bool autoShowLobby = true)
    {
        Debug.Log($"[LevelsManager] LoadScene:  current:{PrevScene}  load:{name}");
        IsLoading = true;
        Kernel.UI.mainCamera.transform.SetParent(Kernel.UI.transform);
        Kernel.UI.HideAll();
        bool loadingShowed = false;
        Kernel.UI.ShowUI<LoadingOverlay>(() =>
        {
            loadingShowed = true;
        });
        while (!loadingShowed) yield return null;
        if (!string.IsNullOrEmpty(PrevScene))
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(PrevScene);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
            onLevelUnloaded?.Invoke((PrevLevelIndex, PrevScene));
        }
        CurrentScene = name;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
        yield return new WaitForSeconds(0.5f);
        IsLoading = false;
        onLevelLoaded?.Invoke((CurrentLevelIndex, CurrentScene));
        bool loadingHided = false;
        Kernel.UI.HideUI<LoadingOverlay>().OnHided(() => { loadingHided = true; });
        while (!loadingHided) yield return null;
        if (autoShowLobby) Kernel.UI.ShowUI<LobbyOverlay>();
    }
}
