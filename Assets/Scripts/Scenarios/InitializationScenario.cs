using GLG;
using System.Collections;
using UnityEngine;

public class InitializationScenario : MonoBehaviour, IScenario
{
    [SerializeField] private string _url = "https://punk-verse-staging.thesmartnik.com/";

    public bool IsComplete { get; private set; }
    public bool IsError { get; private set; }
    public bool IsRunning { get; private set; }
    public string ErrorMessage { get; private set; }

    public void StartScenario()
    {
        if (IsRunning) return;
        IsRunning = true;
        Kernel.CoroutinesObject.StartCoroutine(Scenario());
    }
    public void StopScenario()
    {
        if (!IsRunning) return;
        IsRunning = false;
        Kernel.CoroutinesObject.StopCoroutine(Scenario());
    }

    
    private IEnumerator Scenario()
    {
        Kernel.UI.ShowUI<LoadingOverlay>();
        string token = Web.GetToken();
        Web.Initialize(_url, token);
        // get player data
        Web.Request profileRequest = Web.SendRequest("/api/profile", Web.RequestKind.Get);
        while (!profileRequest.IsComplete)
        {
            yield return null;
        }
        if (profileRequest.IsError)
        {
            OnError(profileRequest.Result);
        }
        else
        {
            SharedWebData.Instance.playerProfile = Web.Parse<Profile>(profileRequest.Result);
        }
        // get player avatar
        Web.Request avatarRequest = Web.SendRequest(SharedWebData.Instance.playerProfile.profile_url, Web.RequestKind.Get);
        while (!avatarRequest.IsComplete)
        {
            yield return null;
        }
        if (avatarRequest.IsError)
        {
            OnError(avatarRequest.Result);
        }
        else
        {
            SharedWebData.Instance.playerProfile.avatar = avatarRequest.ResultTexture.ToSprite();
        }

        yield break;
    }

    private void OnError(string error)
    {
        Debug.Log("[LobbyScenario] ERROR: " + error);
        IsError = true;
        ErrorMessage = error;
        StopScenario();
    }
}
