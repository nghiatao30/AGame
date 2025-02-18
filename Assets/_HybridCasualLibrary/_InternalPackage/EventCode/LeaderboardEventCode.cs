using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EventCode]
public enum LeaderboardEventCode
{
    /// <summary>
    /// This event is raised when player passed top milestone rank
    /// <para> <typeparamref name="int"/>: milestoneRank (ex: 50, 20, 10,...) </para>
    /// </summary>
    OnTopMilestoneRankPassed
}