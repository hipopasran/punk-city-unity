using UnityEngine;

public class GameParametersHub : MonoBehaviour
{
    public static event System.Action onNeedToStopMoving;
    public static event System.Action onRemoteConfigUpdated;
    public static event System.Action onLanguageChanged;
    public static event System.Action<int, int> onLevelChanged;
    public static void ApplyRemoteConfig() => onRemoteConfigUpdated?.Invoke();
    public static void NeedToStopMoving() => onNeedToStopMoving?.Invoke();
    public static void LanguageChanged() => onLanguageChanged?.Invoke();
    public static void LevelChanged(int current, int next) => onLevelChanged?.Invoke(current, next);

    private static bool _isWatchingAd;
    public static bool IsWatchingAd
    {
        get => _isWatchingAd;
        set
        {
            _isWatchingAd = value;
        }
    }
}
