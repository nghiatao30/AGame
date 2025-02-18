using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace LatteGames.Monetization
{
    [CreateAssetMenu(fileName = "IAPProductSO", menuName = "LatteGames/IAP/IAPProductSO", order = 0)]

    public class IAPProductSO : ShopProductSO
    {
        [TitleGroup("Info"), PropertyOrder(-20)]
        public string itemID;
        [TitleGroup("Info"), PropertyOrder(-20)]
        public ItemType itemType;
        [TitleGroup("Info"), PropertyOrder(-10)]
        public bool isRemoveAds;

        public string defaultPrice => $"${price}";
        public string defaultOriginalPrice => $"${originalPrice}";
#if UNITY_EDITOR
        [Button]
        void GetItemIDFromName()
        {
            itemID = this.name;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}

