using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UnitSkin : MonoBehaviour, IUnitComponent
{
    [SerializeField] private Transform _skinParent;

    private Unit _unit;
    private Skin _currentSkin;
    private UnitItem[] _items;

    public Skin Skin => _currentSkin;
    public Unit Unit => _unit;

    public void InitializeOn(Unit unit)
    {
        _unit = unit;
    }

    public void SetSkin(string name)
    {
        AsyncOperationHandle<GameObject> asyncOperation = Addressables.LoadAssetAsync<GameObject>(name);
        asyncOperation.Completed += SkinLoadedHandler;
    }
    private void SkinLoadedHandler(AsyncOperationHandle<GameObject> asyncOperation)
    {
        if (_currentSkin != null)
        {
            Addressables.ReleaseInstance(_currentSkin.gameObject);
        }
        _currentSkin = Instantiate(asyncOperation.Result).GetComponent<Skin>();
        Transform skinTransform = _currentSkin.transform;
        skinTransform.SetParent(_skinParent, false);
        skinTransform.localScale = Vector3.one;
        skinTransform.localRotation = Quaternion.identity;
        skinTransform.localPosition = Vector3.zero;
        _items = new UnitItem[_currentSkin.ItemsSlots.Length];
    }

    public void AttachItem(string name, int itemSlot)
    {
        AsyncOperationHandle<GameObject> asyncOperation = Addressables.LoadAssetAsync<GameObject>(name);
        asyncOperation.Completed += (asyncOperationHandler) => { ItemLoadedHandler(asyncOperationHandler, itemSlot); };
    }
    private void ItemLoadedHandler(AsyncOperationHandle<GameObject> asyncOperation, int itemSlot)
    {
        if (_items[itemSlot] != null)
        {
            Addressables.ReleaseInstance(_items[itemSlot].gameObject);
        }
        _items[itemSlot] = Instantiate(asyncOperation.Result).GetComponent<UnitItem>();
        Transform itemTransform = _items[itemSlot].transform;
        itemTransform.SetParent(_currentSkin.ItemsSlots[itemSlot]);
        itemTransform.localScale = Vector3.one;
        itemTransform.localRotation = Quaternion.identity;
        itemTransform.localPosition = Vector3.zero;
    }
}
