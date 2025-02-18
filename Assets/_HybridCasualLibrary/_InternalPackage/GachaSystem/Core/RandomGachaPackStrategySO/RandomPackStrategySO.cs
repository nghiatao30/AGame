using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GachaSystem.Core
{
    [CreateAssetMenu(fileName = "RandomPackStrategySO", menuName = "LatteGames/Gacha/RandomPackStrategySO/RandomPackStrategySO")]
    public class RandomPackStrategySO : SerializedScriptableObject
    {
        public virtual GachaPack GetRandom(List<GachaPacksCollection.PackRngInfo> packRngInfo)
        {
            return packRngInfo.GetRandom().pack;
        }
    }
}