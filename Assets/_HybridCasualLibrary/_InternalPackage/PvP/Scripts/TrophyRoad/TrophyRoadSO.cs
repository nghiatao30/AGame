using UnityEngine;
using System;
using HyrphusQ.Events;
using GachaSystem.Core;
using System.Collections.Generic;

namespace LatteGames.PvP.TrophyRoad
{
    public class TrophyRoadSO : SavedDataSO<TrophyRoadSavedData>, IResourceLocationProvider
    {
        [SerializeField] protected PvPArenaVariable currentChosenArenaVariable;
        [SerializeField] protected CurrentHighestArenaVariable currentHighestArenaVariable; // will be auto-updated after unlocking a new arena
        [SerializeField] protected PPrefFloatVariable currentMedals;
        [SerializeField] protected HighestAchievedPPrefFloatTracker highestAchievedMedals;
        [SerializeField] protected List<ArenaSection> arenaSections = new();
        [Header("Resource")]
        [SerializeField] protected ResourceLocation resourceLocation;
        [SerializeField] protected string resourceItemId;

        public PvPArenaVariable CurrentChosenArena => currentChosenArenaVariable;
        public float CurrentMedals => currentMedals.value;
        public float HighestAchievedMedals => highestAchievedMedals.value;
        public List<ArenaSection> ArenaSections => arenaSections;

        public override TrophyRoadSavedData defaultData
        {
            get
            {
                var result = new TrophyRoadSavedData();
                foreach (var section in arenaSections)
                {
                    for (int i = 0; i < section.MilestonesCount; i++)
                    {
                        result.milestoneStates.Add(new TrophyRoadSavedData.MilestoneState());
                    }
                }
                return result;
            }
        }

        public override void Load()
        {
            base.Load();

            var index = 0;
            foreach (var arena in arenaSections)
            {
                foreach (var milestone in arena.milestones)
                {
                    milestone.Unlocked = false;
                    milestone.TryUnlock(highestAchievedMedals.value, false);
                    var savedState = data.milestoneStates[index];
                    milestone.Claimed = savedState.claimed;
                    milestone.ResourceLocationProvider = this;
                    milestone.OnClaimed += () => savedState.claimed = true;
                    index++;
                }
            }
        }

        public int GetClaimableCount()
        {
            var count = 0;
            foreach (var section in arenaSections)
            {
                foreach (var milestone in section.milestones)
                {
                    if (milestone.reward != null && milestone.Unlocked && !milestone.Claimed)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Calculate the current and the highest achieved progress values by the highest arenaSO
        /// </summary>
        /// <param name="currentValue">based on the current medals count, from 0f to 1f</param>
        /// <param name="highestAchievedValue">based on the highest achieved medals count, from 0f to 1f</param>
        /// <returns>true if there is a next arena, otherwise false</returns>
        public bool TryGetHighestArenaProgressValues(out float currentValue, out float highestAchievedValue)
        {
            return TryGetArenaProgressValues(currentHighestArenaVariable.value, out currentValue, out highestAchievedValue);
        }

        /// <summary>
        /// Calculate the current and the highest achieved progress values by the given arenaSO
        /// </summary>
        /// <param name="arenaSO"></param>
        /// <param name="currentValue">based on the current medals count, from 0f to 1f</param>
        /// <param name="highestAchievedValue">based on the highest achieved medals count, from 0f to 1f</param>
        /// <returns>true if there is a next arena, otherwise false</returns>
        public bool TryGetArenaProgressValues(PvPArenaSO arenaSO, out float currentValue, out float highestAchievedValue)
        {
            currentValue = 0f;
            highestAchievedValue = 0f;
            var sectionIndex = arenaSections.FindIndex(section => section.arenaSO == arenaSO);
            if (sectionIndex < 0 || sectionIndex >= arenaSections.Count - 1) return false;
            var curReq = arenaSections[sectionIndex].GetRequiredMedals();
            var nextReq = arenaSections[sectionIndex + 1].GetRequiredMedals();
            currentValue = Mathf.InverseLerp(curReq, nextReq, CurrentMedals);
            highestAchievedValue = Mathf.InverseLerp(curReq, nextReq, HighestAchievedMedals);
            return true;
        }

        private float GetArenaRequiredAmount(ArenaSection section)
        {
            var requirements = section.arenaSO.GetUnlockRequirements();
            if (requirements == null)
            {
                return 0f;
            }
            foreach (var req in requirements)
            {
                if (req is Requirement_Currency requirement_Currency && requirement_Currency.currencyType == CurrencyType.Medal)
                {
                    return requirement_Currency.requiredAmountOfCurrency;
                }
            }
            return 0f;
        }

        public void TryUnlockingArenas()
        {
            foreach (var section in arenaSections)
            {
                section.arenaSO.TryUnlockItem();
            }
            currentChosenArenaVariable.value = currentHighestArenaVariable.value;
        }

        public bool TryUnlockMilestones(bool notify = true)
        {
            var existUnlocked = false;
            foreach (var section in arenaSections)
            {
                foreach (var milestone in section.milestones)
                {
                    if (milestone.TryUnlock(highestAchievedMedals.value, notify))
                    {
                        existUnlocked = true;
                    }
                }
            }
            return existUnlocked;
        }

        /// <summary>
        /// Check if there is any milestone unlockable
        /// </summary>
        /// <returns></returns>
        public bool ExistsUnlockable()
        {
            foreach (var section in arenaSections)
            {
                foreach (var milestone in section.milestones)
                {
                    if (milestone.IsUnlockable(highestAchievedMedals.value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string GetItemId() => resourceItemId;

        public ResourceLocation GetLocation() => resourceLocation;

        [Serializable]
        public class ArenaSection
        {
            public PvPArenaSO arenaSO;
            public List<Milestone> milestones = new();
            public int MilestonesCount => milestones.Count;
            public float GetRequiredMedals()
            {
                if (!arenaSO.TryGetUnlockRequirement<Requirement_Currency>(
                    out var requirement_Currency,
                    requirement_Currency => requirement_Currency.currencyType == CurrencyType.Medal))
                {
                    return 0f;
                }
                return requirement_Currency.requiredAmountOfCurrency;
            }
            public bool IsUnlocked => arenaSO.IsUnlocked();
        }

        [Serializable]
        public class Milestone
        {
            public event Action OnUnlocked = delegate { };
            public event Action OnClaimed = delegate { };

            // Props
            [NonSerialized]
            bool unlocked = false;
            public bool Unlocked
            {
                get => unlocked;
                set
                {
                    unlocked = value;
                    if (unlocked) OnUnlocked();
                }
            }
            [NonSerialized]
            bool claimed = false;
            public bool Claimed
            {
                get => claimed;
                set
                {
                    claimed = value;
                    if (claimed) OnClaimed();
                }
            }
            public float requiredAmount;
            public ShopProductSO reward;
            public IResourceLocationProvider ResourceLocationProvider { get; set; }

            // Methods
            public bool IsUnlockable(float currentAmount)
            {
                if (Claimed) return false;
                if (Unlocked) return false;
                return currentAmount >= requiredAmount;
            }

            public bool TryUnlock(float currentAmount, bool notify = true)
            {
                if (Claimed) return false;
                if (Unlocked) return false;
                var value = currentAmount >= requiredAmount;
                if (notify)
                {
                    Unlocked = value;
                }
                else
                {
                    unlocked = value;
                }
                return Unlocked;
            }

            public bool TryClaim()
            {
                var cards = GetCards();
                return TryClaimCards(cards);
            }

            public bool TryClaimCards(List<GachaCard> cards)
            {
                if (!Unlocked) return false;
                if (Claimed) return false;
                foreach (var card in cards)
                {
                    if (card is GachaCard_Currency gachaCard_Currency)
                    {
                        gachaCard_Currency.ResourceLocationProvider = ResourceLocationProvider;
                    }
                }
                GameEventHandler.Invoke(UnpackEventCode.OnUnpackStart, cards, null, null);
                Claimed = true;
                return true;
            }

            public List<GachaCard> GetCards()
            {
                if (reward == null)
                {
                    return null;
                }
                return GachaCardGenerator.Instance.Generate(reward);
            }
        }
    }

    [Serializable]
    public class TrophyRoadSavedData : SavedData
    {
        public List<MilestoneState> milestoneStates = new();

        [Serializable]
        public class MilestoneState
        {
            public bool claimed = false;
        }
    }
}
