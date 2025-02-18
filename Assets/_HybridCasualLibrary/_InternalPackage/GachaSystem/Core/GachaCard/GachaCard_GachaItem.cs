using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GachaSystem.Core
{
    [CreateAssetMenu(fileName = "GachaCard_GachaItem", menuName = "LatteGames/Gacha/Card/GachaItem")]
    public class GachaCard_GachaItem : GachaCard
    {
        public virtual GachaItemSO GachaItemSO
        {
            get => TryGetModule<GachaItemCardRewardModule>(out var gachaItemCardRewardModule) ? gachaItemCardRewardModule.GachaItemSO : null;
            set
            {
                if (TryGetModule<GachaItemCardRewardModule>(out var gachaItemCardRewardModule))
                {
                    gachaItemCardRewardModule.GachaItemSO = value;
                }
                if (TryGetModule<NameItemModule>(out var nameItemModule))
                {
                    nameItemModule.displayName = value.GetDisplayName();
                }
                if (TryGetModule<MonoImageItemModule>(out var imageItemModule))
                {
                    imageItemModule.SetThumbnailSprite(value.GetThumbnailImage());
                }
                if (TryGetModule<RarityItemModule>(out var rarityItemModule))
                {
                    rarityItemModule.rarityType = GachaItemSO.GetRarityType();
                }
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            // Must have GachaItemCardReward Module
            if (!TryGetModule<GachaItemCardRewardModule>(out _))
            {
                AddModule(ItemModule.CreateModule<GachaItemCardRewardModule>(this));
            }
            // Must have RarityItem Module
            if (!TryGetModule<RarityItemModule>(out _))
            {
                AddModule(ItemModule.CreateModule<RarityItemModule>(this));
            }
        }
#endif
        public override GachaCard Clone(params object[] _params)
        {
            var instance = Instantiate(this);
            if (_params[0] is GachaItemSO gachaItemSO)
            {
                instance.GachaItemSO = gachaItemSO;
            }
            return instance;
        }

        public override string ToString()
        {
            return $"GachaItemCard_{GachaItemSO?.GetInternalName() ?? "Null"}";
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (GachaItemSO == null) return false;
            if (other is GachaCard_GachaItem otherCard)
            {
                return GachaItemSO.GetInternalName() == otherCard.GachaItemSO.GetInternalName();
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (GachaItemSO == null)
                return Const.IntValue.Invalid;
            return GachaItemSO.GetInternalName().GetHashCode();
        }
    }
}