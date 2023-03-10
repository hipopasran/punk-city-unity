using UnityEngine;

public class EnvironmentContainer : MonoBehaviour
{
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private Transform _playerPoint;
    [SerializeField] private Transform _playerJumpPoint;
    [SerializeField] private Transform _enemyPoint;
    [SerializeField] private Transform _enemyJumpPoint;

    public Transform CameraPoint => _cameraPoint;
    public Transform PlayerPoint => _playerPoint;
    public Transform EnemyPoint => _enemyPoint;
    public Transform PlayerJumpPoint => _playerJumpPoint;
    public Transform EnemyJumpPoint => _enemyJumpPoint;


    public static EnvironmentContainer Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
}
