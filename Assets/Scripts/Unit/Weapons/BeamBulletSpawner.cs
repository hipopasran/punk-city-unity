using PolygonArsenal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BeamBulletSpawner : BaseBulletSpawner
{
    [SerializeField] private string _beamPrefab;
    [SerializeField] private float _beamDuration;

    private GameObject _beamPrefabInstance;

    public override void Prepare()
    {
        if (!string.IsNullOrEmpty(_beamPrefab))
        {
            Addressables.InstantiateAsync(_beamPrefab).Completed += BeamSpawnedHandler;
        }
    }
    public override void Dispose()
    {
        if (_beamPrefabInstance != null)
        {
            Addressables.ReleaseInstance(_beamPrefabInstance);
        }
    }
    public override void DoShot(Transform startPoint, Vector3 endPoint)
    {
        if (_beamPrefabInstance)
        {
            PolygonBeamStatic polygonBeam = _beamPrefabInstance.GetComponent<PolygonBeamStatic>();
            Transform beamTransform = _beamPrefabInstance.transform;
            beamTransform.position = startPoint.position;
            beamTransform.localScale = Vector3.one;
            beamTransform.LookAt(endPoint);
            polygonBeam.beamLength = Vector3.Distance(startPoint.position, endPoint);
            _beamPrefabInstance.SetActive(true);
            CoroutinesHelper.DoDelayedAction(this, _beamDuration, () =>
            {
                _beamPrefabInstance.SetActive(false);
            });
        }
    }

    private void BeamSpawnedHandler(AsyncOperationHandle<GameObject> asyncOperation)
    {
        _beamPrefabInstance = asyncOperation.Result;
        _beamPrefabInstance.SetActive(false);
    }
}
