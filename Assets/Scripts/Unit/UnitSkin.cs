using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UnitSkin : MonoBehaviour, IUnitComponent
{
    [SerializeField] private float _defaultItemScaleDuration = 0.5f;
    [SerializeField] private Transform _skinParent;

    private Unit _unit;
    private Skin _currentSkin;
    private UnitItem[] _items;
    private event Action<UnitItem, int> _onItemAttachedOneTime;

    public Skin Skin => _currentSkin;
    public Unit Unit => _unit;

    public void InitializeOn(Unit unit)
    {
        _unit = unit;
    }

    public UnitSkin SetSkin(string name)
    {
        AsyncOperationHandle<GameObject> asyncOperation = Addressables.LoadAssetAsync<GameObject>(name);
        asyncOperation.Completed += SkinLoadedHandler;
        return this;
    }
    public UnitSkin RemoveItem(int slot)
    {
        if (_items[slot] != null)
        {
            _items[slot].transform.DOScale(0.001f, 0.5f).OnComplete(() =>
            {
                Addressables.ReleaseInstance(_items[slot].gameObject);
            });
        }
        return this;
    }
    public UnitSkin ClearItems()
    {
        foreach (var item in _items)
        {
            if (item != null)
            {
                item.transform.DOScale(0.001f, 0.5f).OnComplete(() =>
                {
                    Addressables.ReleaseInstance(item.gameObject);
                });
            }
        }
        return this;
    }
    public UnitSkin AttachItem(string name, int itemSlot, float delay = 0f)
    {
        AsyncOperationHandle<GameObject> asyncOperation = Addressables.InstantiateAsync(name);
        asyncOperation.Completed += (asyncOperationHandler) => { ItemLoadedHandler(asyncOperationHandler, itemSlot, delay); };
        return this;
    }
    public UnitSkin OnItemAttached(System.Action<UnitItem, int> callback)
    {
        _onItemAttachedOneTime += callback;
        return this;
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
        _unit.UnitAnimator.FullReset();
    }
    private void ItemLoadedHandler(AsyncOperationHandle<GameObject> asyncOperation, int itemSlot, float delay = 0f)
    {
        if (_items[itemSlot] != null)
        {
            Addressables.ReleaseInstance(_items[itemSlot].gameObject);
        }
        _items[itemSlot] = asyncOperation.Result.GetComponent<UnitItem>();
        Transform itemTransform = _items[itemSlot].transform;
        itemTransform.SetParent(_currentSkin.ItemsSlots[itemSlot]);
        itemTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        itemTransform.localRotation = Quaternion.identity;
        itemTransform.localPosition = Vector3.zero;
        itemTransform.DOScale(1f, _defaultItemScaleDuration).SetEase(Ease.OutBack).SetDelay(delay);
        _onItemAttachedOneTime?.Invoke(_items[itemSlot], itemSlot);
        _onItemAttachedOneTime = null;
    }
}
