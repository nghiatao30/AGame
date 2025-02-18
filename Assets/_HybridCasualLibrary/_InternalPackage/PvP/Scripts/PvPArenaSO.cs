using System;
using System.Collections;
using System.Collections.Generic;
using GachaSystem.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LatteGames.PvP
{
    public abstract class PvPArenaSO : ItemSO
    {
        #region Fields
        /// <summary>
        /// Best of {num of rounds} (Bo1, Bo2,..., Bo{n},..)
        /// </summary>
        [SerializeField]
        protected int m_NumOfRounds = 1;
        [SerializeField]
        protected PvPTournamentSO m_Tournament;
        [SerializeReference, TabGroup("Entry Requirements")]
        protected Requirement m_EntryRequirementTree;
        [SerializeReference, TabGroup("Rewards")]
        protected List<IReward> m_RewardItems;
        [SerializeReference, TabGroup("Punishments")]
        protected List<IPunishment> m_Punishments;
        #endregion

        #region Properties
        public abstract float wonNumOfPoints { get; }
        public abstract float lostNumOfPoints { get; }
        public virtual int index => m_Tournament.arenas.IndexOf(this);
        public virtual int numOfRounds => m_NumOfRounds;
        public virtual int totalNumOfPlayedMatches
        {
            get
            {
                return totalNumOfWonMatches + totalNumOfLostMatches + totalNumOfAbandonedMatches;
            }
        }
        public virtual int totalNumOfWonMatches
        {
            get
            {
                return PlayerPrefs.GetInt($"{guid}_TotalNumOfWonMatches", 0);
            }
            set
            {
                PlayerPrefs.SetInt($"{guid}_TotalNumOfWonMatches", value);
            }
        }
        public virtual int totalNumOfLostMatches
        {
            get
            {
                return PlayerPrefs.GetInt($"{guid}_TotalNumOfLostMatches", 0);
            }
            set
            {
                PlayerPrefs.SetInt($"{guid}_TotalNumOfLostMatches", value);
            }
        }
        public virtual int totalNumOfAbandonedMatches
        {
            get
            {
                return PlayerPrefs.GetInt($"{guid}_TotalNumOfAbandonedMatches", 0);
            }
            set
            {
                PlayerPrefs.SetInt($"{guid}_TotalNumOfAbandonedMatches", value);
            }
        }
        public virtual bool isTournament => tournament.isTournament;
        public virtual PvPTournamentSO.Mode mode => tournament.mode;
        public virtual PvPTournamentSO tournament => m_Tournament;
        public virtual GachaPacksCollection gachaPackCollection => GetReward<RandomGachaPackRewardModule>()?.gachaPackCollection;
        public virtual List<IPunishment> punishments => m_Punishments;
        public virtual List<IReward> rewardItems => m_RewardItems;
        public virtual List<Requirement> entryRequirements => Requirement.GetAvailableLeafRequirements(m_EntryRequirementTree);
        #endregion

        // Entry Requirements
        public virtual bool TryGetEntryRequirement<T>(out T requirement, Predicate<T> predicate = null) where T : Requirement
        {
            requirement = GetEntryRequirement<T>();
            return requirement != null;
        }

        public virtual T GetEntryRequirement<T>(Predicate<T> predicate = null) where T : Requirement
        {
            if (m_EntryRequirementTree == null)
                return null;
            return entryRequirements.Find(item => item is T && (predicate?.Invoke(item as T) ?? true)) as T;
        }

        public virtual bool IsMeetEntryRequirements()
        {
            return entryRequirements.TrueForAll(item => item.IsMeetRequirement());
        }

        public virtual void ExecuteEntryRequirements()
        {
            entryRequirements.ForEach(requirement => requirement.ExecuteRequirement());
        }

        // Punishments
        public virtual bool TryGetPunishment<T>(out T punishment, Predicate<T> predicate = null) where T : class, IPunishment
        {
            punishment = GetPunishment(predicate);
            return punishment != null;
        }

        public virtual T GetPunishment<T>(Predicate<T> predicate = null) where T : class, IPunishment
        {
            if (m_Punishments == null)
                return null;
            return (T)m_Punishments.Find(item => item is T && (predicate?.Invoke(item as T) ?? true));
        }

        public virtual void TakePunishments()
        {
            foreach (var punishment in m_Punishments)
            {
                punishment.TakePunishment();
            }
        }

        // Rewards
        public virtual bool TryGetReward<T>(out T reward, Predicate<T> predicate = null) where T : class, IReward
        {
            reward = GetReward(predicate);
            return reward != null;
        }

        public virtual T GetReward<T>(Predicate<T> predicate = null) where T : class, IReward
        {
            if (m_RewardItems == null)
                return null;
            return (T)m_RewardItems.Find(item => item is T && (predicate?.Invoke(item as T) ?? true));
        }

        public virtual void GrantRewards(Predicate<IReward> predicate = null)
        {
            foreach (var rewardItem in m_RewardItems)
            {
                if (predicate?.Invoke(rewardItem) ?? true)
                {
                    rewardItem.GrantReward();
                }
            }
        }
    }
}