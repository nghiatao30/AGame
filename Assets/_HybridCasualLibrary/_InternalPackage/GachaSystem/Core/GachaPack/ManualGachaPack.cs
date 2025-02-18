using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GachaSystem.Core
{
    public abstract class ManualGachaPack : GachaPack
    {
        [SerializeField]
        protected ShopProductSO m_ShopProductSO;

        public override List<GachaCard> GenerateCards()
        {
            return GachaCardGenerator.Instance.Generate(m_ShopProductSO);
        }
    }
}