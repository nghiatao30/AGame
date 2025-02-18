using System;
using System.Collections;
using System.Collections.Generic;
using LatteGames.Monetization;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopProductSO", menuName = "LatteGames/Shop/ShopProductSO")]
public class ShopProductSO : SerializedScriptableObject
{
    [HideInInspector]
    public Action<ShopProductSO> OnProductPurchased;
    [TitleGroup("Info"), PropertyOrder(0)]
    public ProductType productType;
    [TitleGroup("Info"), PropertyOrder(0)]
    public ShopPackType shopPackType;
    [TitleGroup("Info"), PropertyOrder(0)]
    public string productName;
    [PreviewField(ObjectFieldAlignment.Left), TitleGroup("Info"), PropertyOrder(0)]
    public Sprite icon;
    [TextArea, TitleGroup("Info"), PropertyOrder(0)]
    public string description;
    [OnValueChanged("OnPriceChange"), TitleGroup("Price"), PropertyOrder(100)]
    public float price;
    [OnValueChanged("OnOriginalPriceChange"), TitleGroup("Price"), PropertyOrder(100)]
    public float originalPrice;
    [OnValueChanged("OnDiscountRateChange"), TitleGroup("Price"), PropertyOrder(100)]
    public int discountRate;
    [TitleGroup("Product Values"), PropertyOrder(200)]
    public Dictionary<CurrencyType, DiscountableValue> currencyItems;
    [TitleGroup("Product Values"), PropertyOrder(200)]
    public Dictionary<ItemSO, DiscountableValue> generalItems;
    [TitleGroup("Product Values"), PropertyOrder(200)]
    public Dictionary<PPrefIntVariable, DiscountableValue> consumableItems;

    public int PurchasedTime
    {
        get
        {
            return PlayerPrefs.GetInt($"{this.name}PurchasedTime", 0);
        }
        set
        {
            PlayerPrefs.SetInt($"{this.name}PurchasedTime", value);
        }
    }

    public bool IsPurchased
    {
        get
        {
            if (productType == ProductType.Consumable)
            {
                return false;
            }
            return PlayerPrefs.GetInt($"{this.name}_IsPurchased", 0) == 1 ? true : false;
        }
        set
        {
            if (value)
            {
                PurchasedTime++;
                OnProductPurchased?.Invoke(this);
            }
            if (productType == ProductType.Consumable)
            {
                return;
            }
            PlayerPrefs.SetInt($"{this.name}_IsPurchased", value == true ? 1 : 0);
        }
    }
    void OnPriceChange()
    {
        originalPrice = Mathf.Max(originalPrice, price);
        float discount = originalPrice - price;
        discountRate = Mathf.RoundToInt(discount / originalPrice * 100);
    }
    void OnOriginalPriceChange()
    {
        float discount = originalPrice - price;
        discountRate = Mathf.RoundToInt(discount / originalPrice * 100);
    }
    void OnDiscountRateChange()
    {
        originalPrice = (float)price / (1 - (float)discountRate / 100);
    }
    [Serializable]
    public class DiscountableValue
    {
        public float value;
        public float originalValue;
        public float discountRate => Mathf.RoundToInt((originalValue - value) / originalValue * 100);
    }

    [Button]
    void ResetData()
    {
        PlayerPrefs.DeleteKey($"{this.name}PurchasedTime");
        PlayerPrefs.DeleteKey($"{this.name}_IsPurchased");
    }
}
public enum ProductType
{
    /// <summary>
    /// Consumables may be purchased more than once.
    ///
    /// Purchase history is not typically retained by store
    /// systems once consumed.
    /// </summary>
    Consumable,

    /// <summary>
    /// Non consumables cannot be repurchased and are owned indefinitely.
    /// </summary>
    NonConsumable,

    /// <summary>
    /// Subscriptions have a finite window of validity.
    /// </summary>
    Subscription
}
