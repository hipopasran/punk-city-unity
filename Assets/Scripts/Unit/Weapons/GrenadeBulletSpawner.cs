using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GrenadeBulletSpawner : BaseBulletSpawner
{
    [SerializeField] private float _grenadeMoveDuration;
    [SerializeField] private float _arcHeight;
    [SerializeField] private string _greandePrefab;
    [SerializeField] private string _hitVFXPRefab;

    private GameObject _grenadePrefabInstance;
    private GameObject _hitVFXInstance;

    public override void Prepare()
    {
        if (!string.IsNullOrEmpty(_greandePrefab))
        {
            Addressables.InstantiateAsync(_greandePrefab).Completed += GrenadeSpawnedHandler;
        }
        if (!string.IsNullOrEmpty(_hitVFXPRefab))
        {
            Addressables.InstantiateAsync(_hitVFXPRefab).Completed += HitVfxSpawnedHandler;
        }
    }
    public override void Dispose()
    {
        if (_grenadePrefabInstance != null)
        {
            Addressables.ReleaseInstance(_grenadePrefabInstance);
        }
        if (_hitVFXInstance != null)
        {
            Addressables.ReleaseInstance(_hitVFXInstance);
        }
    }
    public override void DoShot(Transform startPoint, Vector3 endPoint)
    {
        if (_grenadePrefabInstance)
        {
            Transform grenadeTransform = _grenadePrefabInstance.transform;
            grenadeTransform.position = startPoint.position;
            grenadeTransform.rotation = startPoint.rotation;
            _grenadePrefabInstance.SetActive(true);
            grenadeTransform.DOJump(endPoint, _arcHeight, 1, _grenadeMoveDuration).OnComplete(() =>
            {
                if (_hitVFXInstance != null)
                {
                    Transform hitVFXInstanceTransform = _hitVFXInstance.transform;
                    hitVFXInstanceTransform.position = startPoint.position;
                    hitVFXInstanceTransform.rotation = startPoint.rotation;
                    hitVFXInstanceTransform.localScale = Vector3.one;
                    _hitVFXInstance.SetActive(true);
                }
            });
        }
    }

    private void GrenadeSpawnedHandler(AsyncOperationHandle<GameObject> asyncOperation)
    {
        _grenadePrefabInstance = asyncOperation.Result;
        _grenadePrefabInstance.SetActive(false);
    }
    private void HitVfxSpawnedHandler(AsyncOperationHandle<GameObject> asyncOperation)
    {
        _hitVFXInstance = asyncOperation.Result;
        _hitVFXInstance.SetActive(false);
    }
}
