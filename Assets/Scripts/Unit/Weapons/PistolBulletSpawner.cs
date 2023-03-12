using DG.Tweening;
using GLG;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PistolBulletSpawner : BaseBulletSpawner
{
    [SerializeField] private bool _hitVfxRotationAccordingShotDirection = true;
    [SerializeField] private float _bulletMoveDuration;
    [SerializeField] private string _bulletPrefab;
    [SerializeField] private string _shotVFXPrefab;
    [SerializeField] private string _hitVFXPRefab;

    private GameObject _bulletPrefabInstance;
    private GameObject _shotVFXInstance;
    private GameObject _hitVFXInstance;

    public override void Prepare()
    {
        if (!string.IsNullOrEmpty(_bulletPrefab))
        {
            Addressables.InstantiateAsync(_bulletPrefab).Completed += BulletSpawnedHandler;
        }
        if (!string.IsNullOrEmpty(_shotVFXPrefab))
        {
            Addressables.InstantiateAsync(_shotVFXPrefab).Completed += ShotVfxSpawnedHandler;
        }
        if (!string.IsNullOrEmpty(_hitVFXPRefab))
        {
            Addressables.InstantiateAsync(_hitVFXPRefab).Completed += HitVfxSpawnedHandler;
        }
    }
    public override void Dispose()
    {
        if (_bulletPrefabInstance != null)
        {
            Addressables.ReleaseInstance(_bulletPrefabInstance);
        }
        if (_shotVFXInstance != null)
        {
            Addressables.ReleaseInstance(_shotVFXInstance);
        }
        if (_hitVFXInstance != null)
        {
            CoroutinesHelper.DoDelayedAction(Kernel.CoroutinesObject, 3f, () => { Addressables.ReleaseInstance(_hitVFXInstance); });
        }
    }
    public override void DoShot(Transform startPoint, Vector3 endPoint)
    {
        if (_shotVFXInstance != null)
        {
            Transform shotVFXInstanceTransform = _shotVFXInstance.transform;
            shotVFXInstanceTransform.position = startPoint.position;
            shotVFXInstanceTransform.rotation = startPoint.rotation;
            shotVFXInstanceTransform.localScale = Vector3.one;
            _shotVFXInstance.SetActive(true);
        }
        if (_bulletPrefabInstance != null)
        {
            Transform bulletInstanceTransform = _bulletPrefabInstance.transform;
            bulletInstanceTransform.position = startPoint.position;
            bulletInstanceTransform.localScale = Vector3.one;
            bulletInstanceTransform.LookAt(endPoint);
            _bulletPrefabInstance.SetActive(true);
            bulletInstanceTransform.DOMove(endPoint, _bulletMoveDuration).OnComplete(() =>
            {
                _bulletPrefabInstance.SetActive(false);
                if (_hitVFXInstance != null)
                {
                    Transform hitVFXInstanceTransform = _hitVFXInstance.transform;
                    hitVFXInstanceTransform.position = endPoint;
                    if(_hitVfxRotationAccordingShotDirection)
                    {
                        hitVFXInstanceTransform.forward = -startPoint.forward;
                    }
                    else
                    {
                        hitVFXInstanceTransform.rotation = Quaternion.identity;
                    }
                    hitVFXInstanceTransform.localScale = Vector3.one;
                    _hitVFXInstance.SetActive(true);
                }
            });
        }
    }

    private void BulletSpawnedHandler(AsyncOperationHandle<GameObject> asyncOperation)
    {
        _bulletPrefabInstance = asyncOperation.Result;
        _bulletPrefabInstance.SetActive(false);
    }
    private void ShotVfxSpawnedHandler(AsyncOperationHandle<GameObject> asyncOperation)
    {
        _shotVFXInstance = asyncOperation.Result;
        _shotVFXInstance.SetActive(false);
    }
    private void HitVfxSpawnedHandler(AsyncOperationHandle<GameObject> asyncOperation)
    {
        _hitVFXInstance = asyncOperation.Result;
        _hitVFXInstance.SetActive(false);
    }
}
