using System;
using System.Collections.Generic;
using UnityEngine;

namespace GachaSystem.Core
{
    [CreateAssetMenu(fileName = "GachaCard_Currency", menuName = "LatteGames/Gacha/Card/Currency")]
    public class GachaCard_Currency : GachaCard
    {
        [SerializeField] List<CurrencyTypeMap> currencyTypeMapping;

        public int Amount
        {
            get => TryGetModule<CurrencyRewardModule>(out var currencyRewardModule) ? currencyRewardModule.Amount : 0;
            set
            {
                if (TryGetModule<CurrencyRewardModule>(out var currencyRewardModule))
                {
                    currencyRewardModule.Amount = value;
                }
            }
        }

        public CurrencyType CurrencyType
        {
            get => TryGetModule<CurrencyRewardModule>(out var currencyRewardModule) ? currencyRewardModule.CurrencyType : CurrencyType.Standard;
            set
            {
                if (TryGetModule<CurrencyRewardModule>(out var currencyRewardModule))
                {
                    currencyRewardModule.CurrencyType = value;
                }
                if (TryGetModule<MonoImageItemModule>(out var imageModule))
                {
                    imageModule.SetThumbnailSprite(currencyTypeMapping.Find(map => map.currencyType == value).thumbnail);
                }
            }
        }

        public IResourceLocationProvider ResourceLocationProvider
        {
            set
            {
                if (TryGetModule<CurrencyRewardModule>(out var currencyRewardModule))
                {
                    currencyRewardModule.resourceLocationProvider = value;
                }
            }
        }

        public override GachaCard Clone(params object[] _params)
        {
            var instance = base.Clone(_params) as GachaCard_Currency;
            if (_params[0] is int amount)
            {
                instance.Amount = amount;
            }
            if (_params[1] is CurrencyType type)
            {
                instance.CurrencyType = type;
            }
            if (_params.Length >= 3 && _params[2] is IResourceLocationProvider resourceLocationProvider)
            {
                instance.ResourceLocationProvider = resourceLocationProvider;
            }
            return instance;
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            // Must have CurrencyReward
            {
                if (!TryGetModule<CurrencyRewardModule>(out _))
                {
                    var currencyRewardModule = ItemModule.CreateModule<CurrencyRewardModule>(this);
                    currencyRewardModule.Amount = 0;
                    AddModule(currencyRewardModule);
                }
            }
        }
#endif
        public override string ToString()
        {
            return $"{CurrencyType}Card{Amount}";
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other is GachaCard_Currency otherCard)
            {
                return CurrencyType == otherCard.CurrencyType;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return CurrencyType.GetHashCode();
        }

        [Serializable]
        private class CurrencyTypeMap
        {
            public CurrencyType currencyType;
            public Sprite thumbnail;
        }
    }
}
