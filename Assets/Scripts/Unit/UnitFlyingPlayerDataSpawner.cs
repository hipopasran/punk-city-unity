using GLG;
using UnityEngine;

public class UnitFlyingPlayerDataSpawner : MonoBehaviour
{
    [SerializeField] private bool _spawnOnStart = false;
    [SerializeField] private FlyingPlayerDataVisual _prefab;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Transform _target;

    public FlyingPlayerDataVisual PlayerData { get; private set; }

    public FlyingPlayerDataVisual Spawn()
    {
        PlayerData = Kernel.UI.Get<FlyingLabelsOverlay>().CreatePlayerData
            (
            _target, 
            _offset, 
            lockOnTarget: true, 
            stayOnScreen: true, 
            layout: true, 
            combine: false
            ).PlayerData;
        return PlayerData;
    }
    public void Despawn()
    {
        Kernel.UI.Get<FlyingLabelsOverlay>().RemovePlayerData(transform);
    }

    private void Start()
    {
        if(_spawnOnStart)
        {
            Spawn();
        }
    }
}
