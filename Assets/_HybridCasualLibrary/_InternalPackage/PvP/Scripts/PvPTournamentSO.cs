using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.PvP
{
    public abstract class PvPTournamentSO : ItemSO
    {
        public enum Mode
        {
            SingleMatch,
            Tournament
        }
        public enum TournamentBracket
        {
            SingleElimination,
            DoubleElimination,
            Round_Robin,
        }

        #region Fields
        [SerializeField]
        protected TournamentBracket m_TournamentBracket;
        [SerializeField]
        protected int m_NumOfContestant = 2;
        [SerializeField]
        protected List<PvPArenaSO> m_Arenas;
        #endregion

        #region Properties
        public virtual bool isTournament => mode == Mode.Tournament;
        public virtual bool isUnlocked => this.IsUnlocked();
        public virtual Mode mode => m_NumOfContestant == 2 ? Mode.SingleMatch : Mode.Tournament;
        public virtual TournamentBracket tournamentBracket => m_TournamentBracket;
        public virtual int numOfContestant => m_NumOfContestant;
        public virtual int totalOfMatches
        {
            get
            {
                var matches = 0;
                var rounds = totalOfRounds;
                for (int i = 0; i < rounds; i++)
                {
                    matches += (int)Mathf.Pow(2, i);
                }
                return matches;
            }
        }
        public virtual int totalOfRounds => Mathf.RoundToInt(Mathf.Log(m_NumOfContestant, 2));
        public virtual List<PvPArenaSO> arenas => m_Arenas;
        #endregion
    }
}