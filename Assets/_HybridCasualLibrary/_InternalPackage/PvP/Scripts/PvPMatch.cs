using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.PvP
{
    public class PvPMatch
    {
        public enum Status
        {
            Waiting,
            InProgress,
            Completed
        }
        public class EndgameData
        {
            #region Constructors
            public EndgameData()
            {

            }
            public EndgameData (PvPMatch match, PlayerInfo winner, bool isAnyContestantAFK = false)
            {
                this.match = match;
                this.winner = winner;
                this.isAnyContestantAFK = isAnyContestantAFK;
            }
            #endregion

            public virtual PvPMatch match { get; set; }
            public virtual bool isAnyContestantAFK { get; set; }
            public virtual PlayerInfo contestantAFK => isAnyContestantAFK ? loser : null;
            public virtual PlayerInfo winner { get; set; } = null;
            public virtual PlayerInfo loser => winner == null ? null : (winner == match.contestantA ? match.contestantB : match.contestantA);
        }
        public class Round
        {
            #region Constructors
            public Round (PvPMatch match, EndgameData endgameData)
            {
                this.match = match;
                this.winner = endgameData.winner;
            }
            #endregion

            public virtual int round => roundIndex + 1;
            public virtual int roundIndex => match.rounds.IndexOf(this);
            public virtual PlayerInfo winner { get; set; }
            public virtual PlayerInfo loser => winner == null ? null : (winner == match.contestantA ? match.contestantB : match.contestantA);
            public virtual PvPMatch match { get; set; }
        }

        #region Events
        public event Action onMatchPrepared;
        public event Action onMatchStarted;
        public event Action onMatchCompleted;
        public event Action onRoundCompleted;
        #endregion

        #region Fields
        public virtual Status status { get; set; } = Status.Waiting;
        public virtual int totalNumOfRounds => arenaSO.numOfRounds;
        public virtual int currentNumOfRounds => rounds.Count;
        public virtual List<Round> rounds { get; set; } = new List<Round>();
        public virtual bool isAbleToComplete
        {
            get
            {
                if (currentNumOfRounds == totalNumOfRounds)
                    return true;
                int numOfWonRounds_ContestantA = GetTotalNumOfWonRounds(contestantA);
                int numOfWonRounds_ContestantB = GetTotalNumOfWonRounds(contestantB);
                var numOfRoundsToWin = Mathf.CeilToInt(totalNumOfRounds / 2f);
                return numOfWonRounds_ContestantA == numOfRoundsToWin || numOfWonRounds_ContestantB == numOfRoundsToWin;
            }
        }
        public virtual bool isCompleted => status == Status.Completed;
        public virtual bool isTournament => arenaSO.isTournament;
        public virtual bool isContainsLocalPlayer => (contestantA?.isLocal ?? false) || (contestantB?.isLocal ?? false);
        public virtual bool isVictory => endgameData != null && endgameData.winner == GetLocalPlayerInfo();
        public virtual PvPArenaSO arenaSO { get; set; }
        public virtual PlayerInfo contestantA { get; set; }
        public virtual PlayerInfo contestantB { get; set; }
        public virtual EndgameData endgameData { get; set; } = null;
        #endregion

        protected virtual void NotifyEventMatchPrepared()
        {
            onMatchPrepared?.Invoke();
        }

        protected virtual void NotifyEventMatchStarted()
        {
            onMatchStarted?.Invoke();
        }

        protected virtual void NotifyEventMatchCompleted()
        {
            onMatchCompleted?.Invoke();
        }

        protected virtual void NotifyEventRoundCompleted()
        {
            onRoundCompleted?.Invoke();
        }

        public virtual T Cast<T>() where T : PvPMatch
        {
            return this as T;
        }

        public virtual PlayerInfo GetLocalPlayerInfo()
        {
            if (contestantA.isLocal)
                return contestantA;
            if (contestantB.isLocal)
                return contestantB;
            return null;
        }

        public virtual PlayerInfo GetOpponentInfo()
        {
            if (!contestantA.isLocal)
                return contestantA;
            if (!contestantB.isLocal)
                return contestantB;
            return null;
        }

        public virtual void PrepareMatch()
        {
            if (contestantA == null || contestantB == null)
            {
                Debug.LogError("Bruhhh???");
                return;
            }

            NotifyEventMatchPrepared();
        }

        public virtual void StartMatch()
        {
            status = Status.InProgress;
            NotifyEventMatchStarted();
        }

        public virtual void EndMatch()
        {
            status = Status.Completed;
            NotifyEventMatchCompleted();
        }

        public virtual void EndRound(EndgameData endgameData)
        {
            if (endgameData.isAnyContestantAFK)
            {
                var numOfRounds = Mathf.CeilToInt(totalNumOfRounds / 2f) - GetTotalNumOfWonRounds(endgameData.winner);
                for (int i = 0; i < numOfRounds; i++)
                {
                    rounds.Add(new Round(this, endgameData));
                }
            }
            else
                rounds.Add(new Round(this, endgameData));

            if (isAbleToComplete)
            {
                this.endgameData = endgameData;
            }

            NotifyEventRoundCompleted();
        }

        public virtual int GetTotalNumOfWonRounds(PlayerInfo contestant)
        {
            int total = 0;
            foreach (var round in rounds)
            {
                if (contestant == round.winner)
                    total++;
            }
            return total;
        }

        public virtual void RemoveAllListeners()
        {
            onMatchPrepared = null;
            onMatchStarted = null;
            onMatchCompleted = null;
            onRoundCompleted = null;
        }

        public static PvPMatch CreateMatch(PvPArenaSO arenaSO, PlayerInfo contestantA, PlayerInfo contestantB)
        {
            return CreateMatch<PvPMatch>(arenaSO, contestantA, contestantB);
        }

        public static T CreateMatch<T>(PvPArenaSO arenaSO, PlayerInfo contestantA, PlayerInfo contestantB) where T : PvPMatch, new()
        {
            return new T()
            {
                status = Status.Waiting,
                arenaSO = arenaSO,
                contestantA = contestantA,
                contestantB = contestantB,
            };
        }

        public static EndgameData CreateEndgameData(PvPMatch match, PlayerInfo winnerContestant, bool isAnyContestantAFK = false)
        {
            return CreateEndgameData<EndgameData>(match, winnerContestant, isAnyContestantAFK);
        }

        public static T CreateEndgameData<T>(PvPMatch match, PlayerInfo winnerContestant, bool isAnyContestantAFK = false) where T : EndgameData, new()
        {
            return new T()
            {
                match = match,
                winner = winnerContestant,
                isAnyContestantAFK = isAnyContestantAFK
            };
        }
    }
}