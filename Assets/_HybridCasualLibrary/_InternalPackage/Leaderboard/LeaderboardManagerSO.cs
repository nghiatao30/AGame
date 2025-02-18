using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using LatteGames.PvP;
using HyrphusQ.Events;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

[Serializable]
public class FightTimesPerPeriod
{
    [field: SerializeField] public int Seconds { get; set; }
    [field: SerializeField] public int Times { get; set; }
}

[Serializable]
public class LeaderboardSaveData : SavedData
{
    public long lastOpenTime;
    public List<int> topMilestoneRanks = new List<int>();

    public int GetTopMilestoneRank(int index = 0)
    {
        return topMilestoneRanks[index];
    }
    public void SetTopMilestoneRank(int value, int index = 0)
    {
        topMilestoneRanks[index] = value;
    }
}

[CreateAssetMenu(fileName = "LeaderboardManagerSO", menuName = "LatteGames/LeaderboardManagerSO")]
public class LeaderboardManagerSO : SavedDataSO<LeaderboardSaveData>
{
    [Header("References")]
    [SerializeField] ListVariable<PersonalInfo> playerDatabaseSO;
    [SerializeField] FloatVariable playerNumOfPoints;
    [SerializeField] PvPArenaSO[] arenas;

    [Header("Properties")]
    [SerializeField] int[] trackingMilestones;
    [SerializeField] List<FightTimesPerPeriod> periods;

    [NonSerialized]
    private List<PersonalInfo> entries = null;

    // Props
    public int PlayerRank => Entries.FindIndex(item => item.isLocal);
    public long LastOpenTime
    {
        get => data.lastOpenTime;
        set => data.lastOpenTime = value;
    }
    public List<PersonalInfo> Entries
    {
        get
        {
            if (entries == null)
            {
                entries = playerDatabaseSO.GetItemsAlloc();
                entries.Sort((player1, player2) => player2.GetTotalNumOfPoints().CompareTo(player1.GetTotalNumOfPoints()));
            }
            return entries;
        }
    }
    public List<FightTimesPerPeriod> Periods => periods;

    public override LeaderboardSaveData defaultData
    {
        get
        {
            var defaultSaveData = new LeaderboardSaveData();
            defaultSaveData.lastOpenTime = 0;
            defaultSaveData.topMilestoneRanks = new List<int>() { trackingMilestones.First() + 1, trackingMilestones.First() + 1 };
            return defaultSaveData;
        }
    }

    private void OnEnable()
    {
        if (playerNumOfPoints != null)
            playerNumOfPoints.onValueChanged += OnPlayerNumOfPointsChanged;
    }

    private void OnDisable()
    {
        if (playerNumOfPoints != null)
            playerNumOfPoints.onValueChanged -= OnPlayerNumOfPointsChanged;
    }

    private void Sort()
    {
        Entries.Sort((player1, player2) => player2.GetTotalNumOfPoints().CompareTo(player1.GetTotalNumOfPoints()));
    }

    private void OnPlayerNumOfPointsChanged(ValueDataChanged<float> obj)
    {
        var previousPlayerRank = PlayerRank;
        Sort();
        var currentPlayerRank = PlayerRank;
        TrackPassingMilestone(previousPlayerRank, currentPlayerRank);
    }

    private void TrackPassingMilestone(int previousPlayerRank, int currentPlayerRank)
    {
        // only check if the new rank is "higher" (lower index)
        if (currentPlayerRank >= previousPlayerRank)
            return;
        try
        {
            // milestone's rank count from 1, our script's rank count from 0
            var milestoneRank = trackingMilestones.Last(milestoneRank => milestoneRank > currentPlayerRank);
            if (milestoneRank < data.GetTopMilestoneRank())
            {
                data.SetTopMilestoneRank(milestoneRank);
                GameEventHandler.Invoke(LeaderboardEventCode.OnTopMilestoneRankPassed, milestoneRank);
            }
        }
        catch (InvalidOperationException) { } // have not passed any milestone
    }

    private void Simulate(int fightTimes)
    {
        if (arenas == null || arenas.Length <= 0)
            return;
        foreach (var entry in Entries)
        {
            if (entry.isLocal)
                continue;
            for (int i = 0; i < fightTimes; i++)
            {
                var chosenArena = arenas[Random.Range(0, arenas.Length)];
                var victory = Random.value < .5f;
                var points = entry.GetTotalNumOfPoints();
                if (victory)
                    entry.SetTotalNumOfPoints(points + chosenArena.wonNumOfPoints);
                else
                    entry.SetTotalNumOfPoints(points + chosenArena.lostNumOfPoints);
            }
        }
        Sort();
    }

    public void TriggerSimulation()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (LastOpenTime == 0) LastOpenTime = now;
        else
        {
            var secOffset = now - LastOpenTime;
            if (secOffset < 0)
            {
                LastOpenTime = now;
                return;
            }
            try
            {
                var period = Periods.Last(period => secOffset > period.Seconds);
                var fightTimes = period.Times;
                LastOpenTime = now;
                Simulate(fightTimes);
#if UNITY_EDITOR
                Debug.Log($"It has been {secOffset}s. There was {fightTimes} fight times.");
#endif
            }
            catch (InvalidOperationException)
            {
                // open too soon: no update
            }
        }
    }

    public override void Load()
    {
        base.Load();
        Sort();
    }
}