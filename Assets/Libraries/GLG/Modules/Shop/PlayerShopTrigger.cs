using GLG.UI;
using UnityEngine;

public class PlayerShopTrigger : MonoBehaviour
{
    public ShopType shopType;

    public void OpenShop()
    {
        PlayerShopsManager.i.OpenShop(shopType);
    }
}
