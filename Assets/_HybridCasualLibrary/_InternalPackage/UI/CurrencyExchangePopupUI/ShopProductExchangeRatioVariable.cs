using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopProductExchangeRatioVariable", menuName = "LatteGames/CurrencyExchangePopup/ShopProductExchangeRatioVariable")]
public class ShopProductExchangeRatioVariable : FloatVariableReference
{
    [SerializeField]
    protected CurrencyProductSO m_StandardPackProduct;

    public override float value
    {
        get => m_StandardPackProduct == null ? 1f : m_StandardPackProduct.price / m_StandardPackProduct.currencyItems[CurrencyType.Standard].value;
        set
        {
            // Do nothing
        }
    }
}