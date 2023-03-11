using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MeleeBulletSpawner : BaseBulletSpawner
{
    [SerializeField] private string _hitVFXPRefab;

    private GameObject _hitVFXInstance;

    public override void Prepare()
    {
        if (!string.IsNullOrEmpty(_hitVFXPRefab))
        {
            Addressables.InstantiateAsync(_hitVFXPRefab).Completed += HitVfxSpawnedHandler;
        }
    }
    public override void Dispose()
    {
        if (_hitVFXInstance != null)
        {
            Addressables.ReleaseInstance(_hitVFXInstance);
        }
    }
    public override void DoShot(Transform startPoint, Vector3 endPoint)
    {
        if (_hitVFXInstance != null)
        {
            Transform hitVFXInstanceTransform = _hitVFXInstance.transform;
            hitVFXInstanceTransform.position = startPoint.position;
            hitVFXInstanceTransform.rotation = startPoint.rotation;
            hitVFXInstanceTransform.localScale = Vector3.one;
            _hitVFXInstance.SetActive(true);
        }
    }

    private void HitVfxSpawnedHandler(AsyncOperationHandle<GameObject> asyncOperation)
    {
        _hitVFXInstance = asyncOperation.Result;
        _hitVFXInstance.SetActive(false);
    }
}
