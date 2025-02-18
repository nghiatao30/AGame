using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GachaSystem.Core
{
    [CreateAssetMenu(fileName = "GachaCard_Booster", menuName = "LatteGames/Gacha/Card/Booster")]
    public class GachaCard_Booster : GachaCard
    {
        [SerializeField]
        protected List<BoosterTypeMap> boosterTypeMapping = new();

        public PvPBoosterType BoosterType
        {
            get => TryGetModule<PvPBoosterRewardModule>(out var boosterRewardModule) ? boosterRewardModule.BoosterType : default;
            set
            {
                if (TryGetModule<PvPBoosterRewardModule>(out var boosterRewardModule))
                {
                    var boosterMap = boosterTypeMapping.Find(map => map.boosterType == value);
                    boosterRewardModule.BoosterType = value;
                    boosterRewardModule.BoosterSavingSO = boosterMap.boosterSavingSO;
                    // Update icon
                    if (TryGetModule<MonoImageItemModule>(out var imageItemModule))
                    {
                        imageItemModule.SetThumbnailSprite(boosterMap.icon);
                    }
                }
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            // Must have BoosterReward
            if (!TryGetModule<PvPBoosterRewardModule>(out _))
            {
                AddModule(ItemModule.CreateModule<PvPBoosterRewardModule>(this));
            }
        }
#endif

        public Sprite GetIconByType()
        {
            return FindBoosterByType(BoosterType).icon;
        }

        public BoosterTypeMap FindBoosterByType(PvPBoosterType boosterType)
        {
            return boosterTypeMapping.Find(x => x.boosterType == boosterType);
        }

        public override GachaCard Clone(params object[] _params)
        {
            var instance = base.Clone(_params) as GachaCard_Booster;
            if (_params[0] is PvPBoosterType boosterType)
            {
                instance.BoosterType = boosterType;
            }
            return instance;
        }

        public override string ToString()
        {
            return $"BoosterCard{BoosterType}";
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other is GachaCard_Booster otherCard)
            {
                return BoosterType == otherCard.BoosterType;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return BoosterType.GetHashCode();
        }

        [Serializable]
        public class BoosterTypeMap
        {
            public PvPBoosterType boosterType;
            public Sprite icon;
            public PPrefIntVariable boosterSavingSO;
        }
    }
}