using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LatteGames.Monetization
{
    [CreateAssetMenu(fileName = "IAPProductSOContainer", menuName = "LatteGames/IAP/IAPProductSOContainer", order = 0)]

    public class IAPProductSOContainer : SerializedScriptableObject
    {
        public List<IAPProductSO> list = new List<IAPProductSO>();
        public IAPProductSO GetIAPProductSO(string productID)
        {
            var result = list.Find((item) =>
            {
                return item.itemID == productID;
            });
            if (result == null)
            {
                Debug.LogError("Can't get product SO (please check the using IAPProductSOContainer)");
            }
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
            list = EditorUtils.FindAssetsOfType<IAPProductSO>(productsFolderPath);
        }
#endif
    }
}

