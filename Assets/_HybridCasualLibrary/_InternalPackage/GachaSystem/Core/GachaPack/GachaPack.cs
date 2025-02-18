using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

namespace GachaSystem.Core
{
    public abstract class GachaPack : ItemSO
    {
        [BoxGroup("Pack Info"), SerializeField]
        protected GachaCardTemplates cardTemplates;
        [BoxGroup("Pack Info"), SerializeField]
        protected AbstractPack packPrefab;
        public AbstractPack PackPrefab => packPrefab;
        [BoxGroup("Pack Info"), SerializeField]
        protected int totalCardsCount;
        public virtual int TotalCardsCount { get => totalCardsCount; set => totalCardsCount = value; }
        [BoxGroup("Pack Info"), SerializeField]
        protected int unlockedDuration;
        public virtual int UnlockedDuration { get => unlockedDuration; set => unlockedDuration = value; }

        [SerializeField] List<GuaranteedCardsCount> guaranteedCardsCountList = DefaultGuaranteedCardsCountList();

        public virtual IResourceLocationProvider ResourceLocationProvider { get; set; }

        public virtual int GetGuaranteedCardsCount(RarityType rarity)
        {
            var founded = guaranteedCardsCountList.Find(count => count.rarity == rarity);
            return founded != null ? founded.count : 0;
        }

        public virtual void SetGuaranteedCardsCount(RarityType rarity, int count)
        {
            var founded = guaranteedCardsCountList.Find(count => count.rarity == rarity);
            if (founded != null)
            {
                founded.count = count;
            }
        }

        public virtual List<GachaCard> GenerateCards()
        {
            return new List<GachaCard>();
        }

        private static List<GuaranteedCardsCount> DefaultGuaranteedCardsCountList()
        {
            return new List<GuaranteedCardsCount>()
            {
                new GuaranteedCardsCount()
                {
                    rarity = RarityType.Epic,
                    count = 0
                },
                new GuaranteedCardsCount()
                {
                    rarity = RarityType.Legendary,
                    count = 0
                }
            };
        }

        public virtual string GetDisplayName()
        {
            if (TryGetModule<NameItemModule>(out NameItemModule module))
            {
                return module.displayName;
            }
            return "empty";
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            // Must have ItemImage
            if (!TryGetModule<ImageItemModule>(out _))
            {
                AddModule(ItemModule.CreateModule<MonoImageItemModule>(this));
            }
        }
#endif

        [Serializable]
        private class GuaranteedCardsCount
        {
            public RarityType rarity;
            public int count;
        }
    }
}
