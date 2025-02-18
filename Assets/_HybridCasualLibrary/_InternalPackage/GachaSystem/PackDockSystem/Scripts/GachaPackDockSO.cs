using System;
using System.Collections;
using System.Collections.Generic;
using GachaSystem.Core;
using HyrphusQ.DataStructure;
using HyrphusQ.Events;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(fileName = "GachaPackDockSO", menuName = "LatteGames/Gacha/GachaPackDockSystem/GachaPackDockSO")]
public class GachaPackDockSO : SavedDataSO<GachaPackDockData>
{
    [SerializeField] protected GachaPacksList gachaPacksList;
    [SerializeField] protected int slotAmount;
    public List<GachaPackDockSlot> gachaPackDockSlots;

    public override void Load()
    {
        base.Load();
        for (var i = data.gachaPackDockSlots.Count; i < slotAmount; i++)
        {
            data.gachaPackDockSlots.Add(new());
        }
        gachaPackDockSlots = data.gachaPackDockSlots;
        foreach (var slot in gachaPackDockSlots)
        {
            if (slot.GachaPack == null && !slot.gachaPackGUID.IsNullOrWhitespace())
            {
                var pack = gachaPacksList.Packs.Find(x => x.guid == slot.gachaPackGUID);
                if (pack != null)
                {
                    var packInstance = Instantiate(pack);
                    packInstance.UnlockedDuration = slot.gachaPackUnlockedDuration;
                    slot.GachaPack = packInstance;
                }
            }
        }
    }

    public override void Save()
    {
        base.Save();
    }
}

[Serializable]
public class GachaPackDockData : SavedData
{
    public List<GachaPackDockSlot> gachaPackDockSlots = new();
    public System.Collections.Generic.Queue<int> gachaPackDockSlotIndexQueue = new();

    public virtual void Enqueue(GachaPackDockSlot gachaPackDockSlot)
    {
        gachaPackDockSlotIndexQueue.Enqueue(gachaPackDockSlots.IndexOf(gachaPackDockSlot));
    }

    public virtual GachaPackDockSlot Dequeue()
    {
        return gachaPackDockSlots[gachaPackDockSlotIndexQueue.Dequeue()];
    }
}

[Serializable]
public class GachaPackDockSlot
{
    [NonSerialized]
    public Action<GachaPackDockSlotState, GachaPackDockSlotState> OnStateChanged;

    [NonSerialized]
    protected GachaPack gachaPack;
    [NonSerialized]
    protected bool hasPlayedFillAnim = true;
    [ES3Serializable]
    protected string startUnlockTime;
    public string gachaPackGUID;
    public int gachaPackUnlockedDuration;

    [ES3Serializable]
    protected GachaPackDockSlotState state = GachaPackDockSlotState.Empty;

    public virtual GachaPack GachaPack
    {
        get
        {
            return gachaPack;
        }
        set
        {
            gachaPack = value;
            if (value != null)
            {
                gachaPackGUID = value.guid;
                gachaPackUnlockedDuration = value.UnlockedDuration;
            }
            else
            {
                gachaPackGUID = null;
                gachaPackUnlockedDuration = -1;
            }
        }
    }
    public virtual bool HasPlayedFillAnim
    {
        get => hasPlayedFillAnim;

        set => hasPlayedFillAnim = value;
    }
    public virtual DateTime StartUnlockTime
    {
        get
        {
            long time = long.Parse(startUnlockTime);
            return new DateTime(time);
        }
        set => startUnlockTime = value.Ticks.ToString();
    }
    public virtual TimeSpan remainingTimeFromSeconds
    {
        get
        {
            var remainingSeconds = GachaPack.UnlockedDuration - Interval.TotalSeconds;
            return TimeSpan.FromSeconds(remainingSeconds);
        }
    }
    public virtual TimeSpan Interval => DateTime.Now - StartUnlockTime;
    public virtual bool CanGetReward => Interval.TotalSeconds > GachaPack.UnlockedDuration;
    public virtual bool CanOpenNowByAds => State == GachaPackDockSlotState.Unlocking ? remainingTimeFromSeconds.TotalSeconds <= GachaPackDockConfigs.UNLOCK_TIME_PER_RV :
                                            gachaPack.UnlockedDuration <= GachaPackDockConfigs.UNLOCK_TIME_PER_RV;
    public virtual GachaPackDockSlotState State => state;

    public virtual void SetState(GachaPackDockSlotState value)
    {
        if (state == value)
        {
            return;
        }
        var oldState = state;
        state = value;
        if (value == GachaPackDockSlotState.Empty)
        {
            GachaPack = null;
        }
        OnStateChanged?.Invoke(oldState, value);
    }
}
