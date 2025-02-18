using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;

namespace LatteGames.PvP
{
    public class PvPMatchManager : MonoBehaviour
    {
        [SerializeField]
        protected EventCode m_MatchPreparedEventCode;
        [SerializeField]
        protected EventCode m_MatchStartedEventCode;
        [SerializeField]
        protected EventCode m_MatchCompletedEventCode;
        [SerializeField]
        protected EventCode m_RoundCompletedEventCode;
        [SerializeField]
        protected EventCode m_FinalRoundCompletedEventCode;
        [SerializeField]
        protected PlayerInfoVariable m_InfoOfOpponent;

        protected List<PvPMatch> m_InProgressMatches = new List<PvPMatch>();

        protected virtual void NotifyEvent(EventCode eventCode, params object[] parameters)
        {
            if (eventCode == null || eventCode.eventCode == null)
                return;
            GameEventHandler.Invoke(eventCode, parameters);
        }

        public virtual PvPMatch GetCurrentMatchOfPlayer()
        {
            return m_InProgressMatches.Find(match => match.GetLocalPlayerInfo() != null);
        }

        public virtual void PrepareMatch(PvPMatch match)
        {
            if (match.status == PvPMatch.Status.InProgress || match.status == PvPMatch.Status.Completed)
                return;
            match.PrepareMatch();
            m_InfoOfOpponent.value = match.GetOpponentInfo();

            NotifyEvent(m_MatchPreparedEventCode, match);
        }

        public virtual void StartMatch(PvPMatch match)
        {
            if (match.status == PvPMatch.Status.InProgress)
                return;
            match.StartMatch();
            m_InProgressMatches.Add(match);

            NotifyEvent(m_MatchStartedEventCode, match);
        }

        public virtual void EndMatch(PvPMatch match)
        {
            if (match.status == PvPMatch.Status.Completed)
                return;
            match.EndMatch();
            if (match.isVictory)
                match.arenaSO.totalNumOfWonMatches++;
            else if (!match.endgameData.isAnyContestantAFK)
                match.arenaSO.totalNumOfLostMatches++;
            else
                match.arenaSO.totalNumOfAbandonedMatches++;
            m_InProgressMatches.Remove(match);

            NotifyEvent(m_MatchCompletedEventCode, match);
        }

        public virtual void EndRound(PvPMatch match, PlayerInfo winner, bool isAnyContestantAFK = false)
        {
            if (match.status == PvPMatch.Status.Completed)
                return;
            match.EndRound(PvPMatch.CreateEndgameData(match, winner, isAnyContestantAFK));

            NotifyEvent(m_RoundCompletedEventCode, match);
            if (match.isAbleToComplete)
                EndFinalRound(match);
        }

        public virtual void EndFinalRound(PvPMatch match)
        {
            NotifyEvent(m_FinalRoundCompletedEventCode, match);
        }
    }
}