using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GachaSystem.Core
{
    public class GachaCardGenerator : Singleton<GachaCardGenerator>
    {
        [SerializeField] protected GachaCardTemplates cardTemplates;
        public virtual List<GachaCard> Generate(ShopProductSO shopProductSO)
        {
            return null;
        }

        public IEnumerable<T> GenerateRepeat<T>(int count, params object[] _params) where T : GachaCard
        {
            for (int i = 0; i < count; i++)
            {
                yield return cardTemplates.Generate<T>(_params);
            }
        }
    }
}
