using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LatteGames.Monetization
{
    [CreateAssetMenu(fileName = "CurrencyProductSOContainer", menuName = "LatteGames/Economy/CurrencyProductSOContainer", order = 0)]

    public class CurrencyProductSOContainer : SerializedScriptableObject
    {
        public List<CurrencyProductSO> list = new List<CurrencyProductSO>();
        public CurrencyProductSO GetProductSO(string productName)
        {
            var result = list.Find((item) =>
            {
                return item.productName == productName;
            });
            return result;
        }
#if UNITY_EDITOR
        [BoxGroup("Editor Only")]
        [FolderPath]
        public string productsFolderPath;
        [BoxGroup("Editor Only")]
        [Button("GetProducts")]
        void GetProducts()
        {
            if (list != null)
            {
                list.Clear();
            }
            list = EditorUtils.FindAssetsOfType<CurrencyProductSO>(productsFolderPath);
        }
#endif
    }
}

