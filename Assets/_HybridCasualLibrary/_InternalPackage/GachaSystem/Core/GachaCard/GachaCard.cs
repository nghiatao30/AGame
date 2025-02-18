using UnityEngine;

namespace GachaSystem.Core
{
    public abstract class GachaCard : ItemSO, IReward
    {
        public Sprite Icon => TryGetModule<ImageItemModule>(out var imageItemModule) ? imageItemModule.thumbnailImage : null;

        public virtual void GrantReward()
        {
            if (TryGetModule<RewardModule>(out var rewardModule))
            {
                rewardModule.GrantReward();
            }
        }

        public virtual GachaCard Clone(params object[] _params)
        {
            return Instantiate(this);
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            // Must have ImageItem
            if (!TryGetModule<MonoImageItemModule>(out _))
            {
                AddModule(ItemModule.CreateModule<MonoImageItemModule>(this));
            }
        }
#endif
    }
}
