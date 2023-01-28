using DG.Tweening;
using GLG;
using GLG.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SkinShopOverlay : UIController
{
    public SpriteAtlas skinsIconsAtlas;
    public GameObject shadow;
    public GameObject closeButton;
    public GameObject buyAdButton;
    public Transform modelParent;
    public UniversalShopBlock[] items;
    public event System.Action<string> onSkinSelected;

    private GameObject _lastModel;
    private PlayerShopsManager _playerShopsManager;
    private PlayerShopsManager PlayerShopsManager
    {
        get
        {
            if (_playerShopsManager == null)
            {
                _playerShopsManager = Kernel.Economic.PlayerShopsManager;
            }
            return _playerShopsManager;
        }
    }
    private void Start()
    {
        GameParametersHub.onLanguageChanged += OnLanguageChanged;
        foreach (var item in items)
        {
            item.onBuyButtonClicked += OnItemSelected;
        }
    }
    private void OnDestroy()
    {
        GameParametersHub.onLanguageChanged -= OnLanguageChanged;
        foreach (var item in items)
        {
            item.onBuyButtonClicked -= OnItemSelected;
        }
    }

    public bool UpdateItemData(UniversalShopBlock item)
    {
        bool locked;
        PlayerShopItem shopItem = PlayerShopsManager.GetItem(item.id, ShopType.Skin);

        if (shopItem.currentLevel > 0 || shopItem.id == 0)
        {
            item.SetLocked(false);
            locked = false;
        }
        else
        {
            item.SetLocked(true);
            locked = true;
        }
        if (shopItem.isSelected) item.Select();
        else item.Deselect();

        //item.Header = shopItem.DisplayName;
        item.SetIcon(skinsIconsAtlas, shopItem.sprite);
        return locked;
    }
    public void UpdateTexts()
    {
        if (!PlayerShopsManager.Configured) return;
        bool somethingLocked = false;

        foreach (var item in items)
        {
            somethingLocked = UpdateItemData(item) || somethingLocked;
        }
        buyAdButton.SetActive(somethingLocked);
    }

    private void SetModelPreview(PlayerShopItem item)
    {
        if (_lastModel) Destroy(_lastModel);
        //Debug.Log($"Loading model: {item.prefabs[0]}");
        Transform instance = (Instantiate(Resources.Load(item.prefabs[0], typeof(GameObject))) as GameObject).transform;
        instance.SetParent(modelParent);
        instance.localPosition = Vector3.zero;
        instance.localRotation = Quaternion.identity;
        instance.localScale = Vector3.one * 320f;
        _lastModel = instance.gameObject;
    }

    private void OnLanguageChanged()
    {
        UpdateTexts();
    }
    public void BuyRandom()
    {
        List<PlayerShopItem> shopItems = PlayerShopsManager.GetUnpurchasedItems(ShopType.Skin);
        int id = UnityEngine.Random.Range(0, shopItems.Count);
        id = shopItems[id].id;
        PlayerShopsManager.BuyItem(id, ShopType.Skin);
        UniversalShopBlock block = GetItem(id);
        block.SetLocked(false);
        block.transform.DOPunchScale(Vector3.one * 0.5f, 1f, 2);
        OnItemSelected(id);
        UpdateTexts();
    }

    private void ShowNotify(string text)
    {
        Kernel.UI.ShowUI<NotifyOverlay>().SetText(text);
    }
    public void OnBuyAdButtonClicked()
    {
        /*
        if (AdWrapper.Rewarded.IsReady)
        {
            closeButton.gameObject.SetActive(false);
            GameParametersHub.IsWatchingAd = true;
            AdWrapper.Rewarded.ClearCallbacks().Show("reward_ingame_rvSkin")
                .OnOpened(() =>
                {
                })
                .OnSuccess(() =>
                {
                    BuyRandom();
                    closeButton.gameObject.SetActive(true);
                })
                .OnClosed(() =>
                {
                    GameParametersHub.IsWatchingAd = false;
                    closeButton.gameObject.SetActive(true);
                });
        }
        else
        {
            ShowNotify(I2.Loc.LocalizationManager.GetTranslation("notify_advNotReady"));
        }
        */
    }
    public void OnItemSelected(int id)
    {
        //Debug.Log("Item selected: " + id);
        foreach (var item in items)
        {
            if (item.id == id) item.Select();
            else item.Deselect();
        }
        PlayerShopItem shopItem = PlayerShopsManager.SelectItem(ShopType.Skin, id);
        SetModelPreview(shopItem);
        onSkinSelected?.Invoke(shopItem.prefabs[1]);
        //Debug.Log($"[SkinShop] Selected: {shopItem.prefabs[1]}");
    }
    private UniversalShopBlock GetItem(int id)
    {
        foreach (var item in items)
        {
            if (item.id == id) return item;
        }
        return null;
    }
    protected override void OnStartShow()
    {
        shadow.SetActive(true);
        GameParametersHub.NeedToStopMoving();
        UpdateTexts();
        OnItemSelected(PlayerShopsManager.GetLastSelectedItem(ShopType.Skin));
        Kernel.UI.HideUI<InGameScreen>();
    }
    protected override void OnEndHide()
    {
        Kernel.UI.ShowUI<InGameScreen>();
    }
    protected override void OnStartHide()
    {
        shadow.SetActive(false);
        if (_lastModel) Destroy(_lastModel);
    }
}
