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

    private void OnDrawGizmos()
    {
        if(_playerPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(_playerPoint.position, new Vector3(1f, 2f, 1f));
            Gizmos.DrawRay(_playerPoint.position, _playerPoint.forward * 10f);
        }
        if (_enemyPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(_enemyPoint.position, new Vector3(1f, 2f, 1f));
            Gizmos.DrawRay(_enemyPoint.position, _enemyPoint.forward * 10f);
        }
    }
}
