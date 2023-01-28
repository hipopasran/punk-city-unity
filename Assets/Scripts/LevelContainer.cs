using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    [SerializeField] private LevelController _levelController;

    public static LevelContainer Instance { get; private set; }
    public LevelController LevelController => _levelController;

    private void Awake()
    {
        Instance = this;
    }
}
