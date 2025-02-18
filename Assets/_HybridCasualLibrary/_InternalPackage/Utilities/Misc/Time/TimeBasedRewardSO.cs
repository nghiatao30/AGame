using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeBasedRewardSO", menuName = "LatteGames/Misc/TimeBasedRewardSO/TimeBasedRewardSO")]
public class TimeBasedRewardSO : PPrefStringVariable
{
    [SerializeField] int coolDownInterval;
    public virtual int CoolDownInterval { get => coolDownInterval; set => coolDownInterval = value; }

    public virtual string lastRewardTime
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
        }
    }
    public virtual string remainingTime
    {
        get
        {
            TimeSpan interval = DateTime.Now - LastRewardTime;
            var remainingSeconds = CoolDownInterval - interval.TotalSeconds;
            interval = TimeSpan.FromSeconds(remainingSeconds);
            return string.Format("{0:00}h{1:00}m{2:00}s", interval.Hours, interval.Minutes, interval.Seconds);
        }
    }
    public virtual string remainingTimeInMinute
    {
        get
        {
            TimeSpan interval = DateTime.Now - LastRewardTime;
            var remainingSeconds = CoolDownInterval - interval.TotalSeconds;
            interval = TimeSpan.FromSeconds(remainingSeconds);
            return string.Format("{0:00}:{1:00}", interval.Minutes, interval.Seconds);
        }
    }
    public virtual bool canGetReward
    {
        get
        {
            TimeSpan interval = DateTime.Now - LastRewardTime;
            return interval.TotalSeconds > CoolDownInterval;
        }
    }
    public virtual DateTime LastRewardTime
    {
        get
        {
            long time = long.Parse(lastRewardTime);
            return new DateTime(time);
        }
        private set => lastRewardTime = (value.Ticks.ToString());
    }
    public virtual DateTime Now => DateTime.Now;

    public virtual int GetRewardByAmount()
    {
        TimeSpan interval = DateTime.Now - LastRewardTime;
        int amount = Mathf.FloorToInt((float)(interval.TotalSeconds / CoolDownInterval));
        var remainingSeconds = interval.TotalSeconds % CoolDownInterval;
        LastRewardTime = DateTime.Now.AddSeconds(-remainingSeconds);
        return amount;
    }
    public virtual void GetReward()
    {
        LastRewardTime = DateTime.Now;
    }
    [ContextMenu("ResetTime")]
    public virtual void ResetTime()
    {
        LastRewardTime = DateTime.MinValue;
    }
    [ContextMenu("RemainingTime")]
    public virtual void RemainingTime()
    {
        Debug.Log(remainingTime);
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (string.IsNullOrEmpty(m_InitialValue))
        {
            m_InitialValue = "0";
        }
    }
#endif
}