using System;
using System.Collections;
using System.Collections.Generic;
using LatteGames.Monetization;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyProductSO", menuName = "LatteGames/Economy/CurrencyProductSO")]
public class CurrencyProductSO : ShopProductSO
{
    [TitleGroup("Price"), PropertyOrder(99)]
    public CurrencyType currencyType;

#if UNITY_EDITOR
    [Button]
    void GetItemIDFromName()
    {
        productName = this.name;
        EditorUtility.SetDirty(this);
    }
#endif
}
