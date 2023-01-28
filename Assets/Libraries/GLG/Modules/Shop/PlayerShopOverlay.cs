using GLG;
using GLG.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerShopOverlay : PlayerShopUI
{
    [SerializeField] private SpriteAtlas _playerShopAtlas;
    [SerializeField] private List<UniversalShopBlock> _shopBlocks = new List<UniversalShopBlock>();
    private PlayerShopsManager _playerShopsManager;
    private PlayerShopsManager PlayerShopsManager
    {
        get
        {
            if(_playerShopsManager == null)
            {
                _playerShopsManager = Kernel.Economic.PlayerShopsManager;
            }
            return _playerShopsManager;
        }
    }


    private void Start()
    {
        foreach (var item in _shopBlocks)
        {
            item.onBuyButtonClicked += OnItemBought;
            item.onAdButtonClicked += OnAdItemBought;
        }
    }
    private void OnDestroy()
    {
        //GameParametersHub.onLanguageChanged -= OnLanguageChanged;
    }
    protected override void OnStartShow()
    {
        //GameParametersHub.NeedToStopMoving();
        //PlayerMoney.onUpdateMoney.AddListener(UpdateTexts);
        UpdateTexts();
    }
    protected override void OnStartHide()
    {
        //PlayerMoney.onUpdateMoney.RemoveListener(UpdateTexts);
    }

    private void UpdateTexts()
    {
        foreach (var item in _shopBlocks)
        {
            PlayerShopItem shopItem = PlayerShopsManager.GetItem(item.id, ShopType.Upgrade);
            item.SetHeader(shopItem.DisplayName);
            item.SetIcon(_playerShopAtlas.GetSprite(shopItem.sprite));
            item.SetCurrentLevel(I2.Loc.LocalizationManager.GetTranslation("upgrMenu_lvl") + " " + shopItem.CurrentDisplayLevel);
            item.SetNextLevel((shopItem.IsMaxLevel ? "" : (I2.Loc.LocalizationManager.GetTranslation("upgrMenu_lvl") + " ")) + shopItem.NextDisplayLevel);
            if (shopItem.CurrentCost == 0)
            {
                item.buyButton.SetHeader(I2.Loc.LocalizationManager.GetTranslation("shopBlockController_free"), false, true);
                item.SetAdButtonVisible(false);
            }
            else if (shopItem.CurrentCost == int.MaxValue)
            {
                item.buyButton.SetHeader(I2.Loc.LocalizationManager.GetTranslation("shopBlockController_max"), false, true);
                item.SetAdButtonVisible(false);
            }
            else
            {
                item.buyButton.SetHeader(shopItem.CurrentCost.ToString(), false, true);
                item.SetAdButtonVisible(true);
            }
            item.buyButton.SetLocked(Economic.i.PlayerMoney.Money >= shopItem.CurrentCost ? false : true, false);
            bool isMaxLevel = shopItem.IsMaxLevel;
            item.SetNextLevelVisible(!isMaxLevel);
            item.SetLevelArrowVisible(!isMaxLevel);
        }
    }

    public void OnItemBought(int id)
    {
        PlayerShopItem item = PlayerShopsManager.GetItem(id, ShopType.Upgrade);
        if (Economic.i.PlayerMoney.TrySpendMoney(item.CurrentCost))
        {
            PlayerShopsManager.BuyItem(id, ShopType.Upgrade);
        }
        UpdateTexts();
    }
    public void OnAdItemBought(int id) //TODO
    {
        PlayerShopItem item = PlayerShopsManager.GetItem(id, ShopType.Upgrade);
        /*
        if (AdWrapper.Rewarded.IsReady)
        {
            GameParametersHub.IsWatchingAd = true;
            AdWrapper.Rewarded.ClearCallbacks().Show($"reward_ingame_{item.rvName}_{item.rvValue}")
                .OnSuccess(() =>
                {
                    PlayerShopsManager.BuyItem(id, ShopType.Upgrade);
                    UpdateTexts();
                })
                .OnClosed(() =>
                {
                    GameParametersHub.IsWatchingAd = false;
                });
        }
        else
        {
            ShowNotify(I2.Loc.LocalizationManager.GetTranslation("notify_advNotReady"));
        }
        */
    }

    private void OnLanguageChanged()
    {
        UpdateTexts();
    }
    private void ShowNotify(string text)
    {
        Kernel.UI.ShowUI<NotifyOverlay>().SetText(text);
    }
}
