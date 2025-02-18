using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DayBasedRewardSO", menuName = "LatteGames/Misc/TimeBasedRewardSO/DayBasedRewardSO")]
public class DayBasedRewardSO : TimeBasedRewardSO
{
    [SerializeField] PPrefIntVariable coolDownIntervalPPref;
    public override int CoolDownInterval { get => coolDownIntervalPPref.value; set => coolDownIntervalPPref.value = (value); }

    public override void GetReward()
    {
        base.GetReward();
        var now = DateTime.Now;
        var endDayTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
        var timeSpan = endDayTime - now;
        CoolDownInterval = (int)timeSpan.TotalSeconds;
    }
}
