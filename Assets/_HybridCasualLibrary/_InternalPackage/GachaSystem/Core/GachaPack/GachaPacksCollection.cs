using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GachaSystem.Core
{
    [CreateAssetMenu(fileName = "GachaPacksCollection", menuName = "LatteGames/Gacha/PacksCollection")]
    public class GachaPacksCollection : ScriptableObject
    {
        [SerializeField]
        protected List<PackRngInfo> packRngInfos = new();
        [SerializeField]
        protected RandomPackStrategySO randomPackStrategySO;

        public virtual List<PackRngInfo> PackRngInfos => packRngInfos;

        public virtual List<GachaPack> GetAllAvailablePacks()
        {
            return packRngInfos.Where(info => !info.Probability.Equals(0f)).Select(info => info.pack).ToList();
        }

        public virtual GachaPack GetRandom()
        {
            return randomPackStrategySO.GetRandom(packRngInfos);
        }

        [Serializable]
        public class PackRngInfo : IRandomizable
        {
            public GachaPack pack;
            [Range(0f, 1f)] public float probability;
            public float Probability { get => probability; set => probability = value; }
        }
    }
}
