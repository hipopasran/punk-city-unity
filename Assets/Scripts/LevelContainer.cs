using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    [SerializeField] private LevelController _levelController;
    [SerializeField] private BaseScenario _mainScenario;

    public static LevelContainer Instance { get; private set; }
    public LevelController LevelController => _levelController;
    public BaseScenario MainScenario => _mainScenario;

    private void Awake()
    {
        Instance = this;
    }
}
