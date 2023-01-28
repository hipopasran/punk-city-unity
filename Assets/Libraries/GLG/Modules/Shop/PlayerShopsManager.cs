using GLG;
using GLG.UI;
using System.Collections.Generic;
using UnityEngine;

public enum ShopType { Upgrade, Skin }
[System.Serializable]
public class ShopItemConfig
{
    public string name;
    public int[] costs;
    public int maxLevel;
}
[System.Serializable]
public class PlayerShopItem
{
    [SerializeField] private string displayNameTranslationKey;
    public string configName;
    public string sprite;
    public int id;
    public string remoteConfigName;
    public string rvName;
    public string rvValue;
    public string[] prefabs;
    [HideInInspector]
    public int currentLevel = 0;
    [HideInInspector]
    public int maxLevel = 0;
    [HideInInspector]
    public List<int> costs;
    [HideInInspector]
    public bool isSelected = false;
    /// <summary>
    /// Локализованное имя предмета.
    /// </summary>
    public string DisplayName => I2.Loc.LocalizationManager.GetTranslation(displayNameTranslationKey);
    /// <summary>
    /// Текущая стоимость покупки. Если куплен последний уровень - int.MaxValue
    /// </summary>
    public int CurrentCost => IsMaxLevel ? int.MaxValue : costs[currentLevel];
    /// <summary>
    /// True если куплен последний уровень.
    /// </summary>
    public bool IsMaxLevel => currentLevel >= maxLevel;
    /// <summary>
    /// Текущая стоимость. "MAX" если куплен максимальный уровень.
    /// </summary>
    public string CurrentDisplayCost => IsMaxLevel ? I2.Loc.LocalizationManager.GetTranslation("shopBlockController_max") : CurrentCost.ToString();
    /// <summary>
    /// Уровень + 1
    /// </summary>
    public string CurrentDisplayLevel => (currentLevel + 1).ToString();
    /// <summary>
    /// Максимальный уровень + 1
    /// </summary>
    public string MaxDisplayLevel => (maxLevel + 1).ToString();
    /// <summary>
    /// Уровень + 2. Если купили максимальный уровень - пустая строка.
    /// </summary>
    public string NextDisplayLevel => IsMaxLevel ? "" : (currentLevel + 2).ToString();
    public void Select()
    {
        if (isSelected) return;
        isSelected = true;
        onSelectionChanged?.Invoke(true);
    }
    public void Deselect()
    {
        if (!isSelected) return;
        isSelected = false;
        onSelectionChanged?.Invoke(false);
    }
    public IntEvent onBoughtEvent;
    public BoolEvent onSelectionChanged;
}
[System.Serializable]
public class PlayerShop
{
    public string shopName;
    public ShopType shopType;
    public bool selectableItems;
    public UIController relatedUI;
    public PlayerShopItem[] items;
}
[System.Serializable]
public abstract class PlayerShopUI : UIController
{

}

public class PlayerShopsManager : MonoBehaviour
{
    public static PlayerShopsManager i;
    public PlayerShop[] shops;
    private bool _configured = false;
    public bool Configured => _configured;
    private void Awake()
    {
        i = this;
    }
    public int GetLastSelectedItem(ShopType shopType)
    {
        return PlayerPrefs.GetInt("shop_selection_" + shopType.ToString(), 0);
    }
    public int GetLastPurchasedItem(ShopType shopType)
    {
        return PlayerPrefs.GetInt("shop_purchased_" + shopType.ToString(), 0);
    }
    public void OpenShop(ShopType shopType)
    {
        Kernel.UI.ShowUI(GetShop(shopType).relatedUI);
    }
    public PlayerShopItem BuyItem(int id, ShopType shopType)
    {
        SetLastPurchasedItem(shopType, id);
        PlayerShopItem item = GetItem(id, shopType);
        if (item.IsMaxLevel) return null;
        item.currentLevel++;
        item.onBoughtEvent.Invoke(item.currentLevel - 1);
        PlayerPrefs.SetInt(item.configName, item.currentLevel);
        return item;
    }
    public PlayerShopItem GetItem(int id, ShopType shopType)
    {
        PlayerShopItem[] currentItems = GetShop(shopType).items;
        for (int i = 0; i < currentItems.Length; i++)
        {
            if (currentItems[i].id == id) return currentItems[i];
        }
        return null;
    }
    public void ConfigureByConfigs(ShopType shopType, ShopItemConfig[] itemsConfigs)
    {
        PlayerShop shop = GetShop(shopType);
        PlayerShopItem currentItem;
        for (var i = 0; i < itemsConfigs.Length; i++)
        {
            currentItem = shop.items[i];
            currentItem.costs = new List<int>(itemsConfigs[i].costs);
            currentItem.maxLevel = itemsConfigs[i].maxLevel;
        }
    }

    public void Load()
    {
        foreach (var shop in shops)
        {
            foreach (var item in shop.items)
            {
                item.currentLevel = PlayerPrefs.GetInt(item.configName, 0);
                for (int i = 0; i < item.currentLevel; i++)
                {
                    item.onBoughtEvent.Invoke(i);
                }
            }
            if (shop.selectableItems)
            {
                int lastSelected = GetLastSelectedItem(shop.shopType);
                SelectItem(shop.shopType, lastSelected);
            }
        }
    }
    public PlayerShopItem SelectItem(ShopType shopType, int id, bool deselectOthers = true)
    {
        PlayerShopItem[] items = GetShop(shopType).items;
        PlayerShopItem result = null;
        SetLastSelectedItem(shopType, id);
        foreach (var item in items)
        {
            if (item.id == id)
            {
                item.Select();
                result = item;
            }
            else if (deselectOthers)
            {
                item.Deselect();
            }
        }
        return result;
    }
    public List<PlayerShopItem> GetUnpurchasedItems(ShopType shopType)
    {
        PlayerShopItem[] items = GetShop(shopType).items;
        List<PlayerShopItem> result = new List<PlayerShopItem>();
        foreach (var item in items)
        {
            if (item.currentLevel == 0)
            {
                result.Add(item);
            }
        }
        return result.Count > 0 ? result : null;
    }
    private void SetLastSelectedItem(ShopType shopType, int id)
    {
        PlayerPrefs.SetInt("shop_selection_" + shopType.ToString(), id);
    }
    private void SetLastPurchasedItem(ShopType shopType, int id)
    {
        PlayerPrefs.SetInt("shop_purchased_" + shopType.ToString(), id);
    }
    private PlayerShop GetShop(ShopType shopType)
    {
        foreach (var item in shops)
        {
            if (item.shopType == shopType) return item;
        }
        return null;
    }
}
