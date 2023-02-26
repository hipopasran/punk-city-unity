using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    [SerializeField] private LevelController _levelController;
    [SerializeField] private BaseScenario _mainScenario;
    [SerializeField] private LobbyUnitsSpawner _lobbyUnitsSpawner;
    [SerializeField] private Transform _cameraPoint;

    public static LevelContainer Instance { get; private set; }
    public LevelController LevelController => _levelController;
    public BaseScenario MainScenario => _mainScenario;
    public LobbyUnitsSpawner LobbyUnitsSpawner => _lobbyUnitsSpawner;
    public Transform CameraPoint => _cameraPoint;

    private void Awake()
    {
        Instance = this;
    }
}
