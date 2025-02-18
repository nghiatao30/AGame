using System;
using System.Collections;
using System.Collections.Generic;
using GachaSystem.Core;
using HyrphusQ.Events;
using UnityEngine;

/// <summary>
/// Manages the Gacha Pack Dock, handling the addition, unlocking, and queuing of Gacha Packs.
/// </summary>
public class PackDockManager : Singleton<PackDockManager>
{
    public static Queue<int> queue = new(new int[10]);
    public static Queue<int> queue2 = new(10);
    
    [SerializeField] protected GachaPackDockSO gachaPackDockSO;

    public GachaPackDockSO GachaPackDockSO => gachaPackDockSO;
    public GachaPackDockData gachaPackDockData => gachaPackDockSO.data;
    public bool IsQueueable => gachaPackDockData.gachaPackDockSlotIndexQueue.Count < GachaPackDockConfigs.NUMBER_PACK_CAN_QUEUE_AT_SAME_TIME;
    public bool IsFull => gachaPackDockData.gachaPackDockSlots.Find(slot => slot.State == GachaPackDockSlotState.Empty) == null;
    public bool IsFullUnlockedSlots => gachaPackDockData.gachaPackDockSlots.FindAll(slot => slot.State == GachaPackDockSlotState.Unlocking).Count >= GachaPackDockConfigs.NUMBER_PACK_CAN_UNLOCK_AT_SAME_TIME;

    protected virtual void Start()
    {
        CheckToUnlockSlots(true);
    }

    public virtual GachaPackDockSlot GetGachaPackDockSlot(int index)
    {
        return gachaPackDockData.gachaPackDockSlots[index];
    }

    public virtual void CheckToUnlockSlots(bool isAccumalation = false)
    {
        // Find the first slot that is currently unlocking
        var unlockingSlot = gachaPackDockData.gachaPackDockSlots.Find(x => x.State == GachaPackDockSlotState.Unlocking);

        if (unlockingSlot != null)
        {
            // Calculate the total seconds passed since the slot started unlocking
            var accumulatedPassedTotalSeconds = unlockingSlot.Interval.TotalSeconds;

            // Check if the accumulated time is sufficient to unlock the slot
            if (accumulatedPassedTotalSeconds - unlockingSlot.GachaPack.UnlockedDuration >= 0)
            {
                accumulatedPassedTotalSeconds -= unlockingSlot.GachaPack.UnlockedDuration;
                unlockingSlot.SetState(GachaPackDockSlotState.Unlocked);

                // Process the queue of slots waiting to be unlocked
                while (gachaPackDockData.gachaPackDockSlotIndexQueue.Count > 0)
                {
                    var slot = gachaPackDockData.Dequeue();

                    // Check if the accumulated time is sufficient to unlock the next slot in the queue
                    if (accumulatedPassedTotalSeconds - slot.GachaPack.UnlockedDuration >= 0)
                    {
                        accumulatedPassedTotalSeconds -= slot.GachaPack.UnlockedDuration;
                        slot.SetState(GachaPackDockSlotState.Unlocked);
                    }
                    else
                    {
                        // Set the start unlock time for the slot and start unlocking it
                        var unlockTime = DateTime.Now - TimeSpan.FromSeconds(isAccumalation ? accumulatedPassedTotalSeconds : 0);
                        slot.StartUnlockTime = unlockTime;
                        StartUnlock(slot, false);
                        break;
                    }
                }
            }
            // Update the states of all slots
            UpdateSlotStates();
        }
    }

    public virtual void StartUnlock(GachaPackDockSlot gachaPackDockSlot, bool isAutoSetUnlockTime = true)
    {
        if (isAutoSetUnlockTime)
        {
            gachaPackDockSlot.StartUnlockTime = DateTime.Now;
        }
        gachaPackDockSlot.SetState(GachaPackDockSlotState.Unlocking);
        UpdateSlotStates();
    }

    public virtual void UnlockNow(GachaPackDockSlot gachaPackDockSlot)
    {
        gachaPackDockSlot.SetState(GachaPackDockSlotState.Unlocked);
        UpdateSlotStates();
    }

    public virtual void ReduceUnlockTime(GachaPackDockSlot gachaPackDockSlot)
    {
        gachaPackDockSlot.StartUnlockTime -= TimeSpan.FromSeconds(GachaPackDockConfigs.UNLOCK_TIME_PER_RV);
        CheckToUnlockSlots();
    }

    public virtual void Enqueue(GachaPackDockSlot gachaPackDockSlot)
    {
        gachaPackDockSlot.SetState(GachaPackDockSlotState.Queued);
        gachaPackDockData.Enqueue(gachaPackDockSlot);
        UpdateSlotStates();
    }

    public virtual bool TryToAddPack(GachaPack gachaPack)
    {
        if (gachaPack == null || IsFull)
        {
            return false;
        }
        for (var i = 0; i < gachaPackDockData.gachaPackDockSlots.Count; i++)
        {
            var slot = gachaPackDockData.gachaPackDockSlots[i];
            if (slot.State == GachaPackDockSlotState.Empty)
            {
                slot.GachaPack = gachaPack;
                slot.SetState(GachaPackDockSlotState.WaitToUnlock);
                slot.HasPlayedFillAnim = false;
                GameEventHandler.Invoke(GachaPackDockEventCode.OnAddPackToDock, slot);
                UpdateSlotStates();
                return true;
            }
        }
        return false;
    }

    public virtual void UpdateSlotStates()
    {
        if (!IsFullUnlockedSlots)
        {
            foreach (var gachaPackDockSlot in gachaPackDockData.gachaPackDockSlots)
            {
                if (gachaPackDockSlot.State == GachaPackDockSlotState.WaitForAnotherUnlock || gachaPackDockSlot.State == GachaPackDockSlotState.WaitToQueue)
                {
                    gachaPackDockSlot.SetState(GachaPackDockSlotState.WaitToUnlock);
                }
            }
        }
        else if (IsQueueable)
        {
            foreach (var gachaPackDockSlot in gachaPackDockData.gachaPackDockSlots)
            {
                if (gachaPackDockSlot.State == GachaPackDockSlotState.WaitForAnotherUnlock || gachaPackDockSlot.State == GachaPackDockSlotState.WaitToUnlock)
                {
                    gachaPackDockSlot.SetState(GachaPackDockSlotState.WaitToQueue);
                }
            }
        }
        else if (!IsQueueable)
        {
            foreach (var gachaPackDockSlot in gachaPackDockData.gachaPackDockSlots)
            {
                if (gachaPackDockSlot.State == GachaPackDockSlotState.WaitToQueue || gachaPackDockSlot.State == GachaPackDockSlotState.WaitToUnlock)
                {
                    gachaPackDockSlot.SetState(GachaPackDockSlotState.WaitForAnotherUnlock);
                }
            }
        }
        GameEventHandler.Invoke(GachaPackDockEventCode.OnGachaPackDockUpdated);
    }

    public virtual void OpenSlot(GachaPackDockSlot gachaPackDockSlot)
    {
        GameEventHandler.Invoke(UnpackEventCode.OnUnpackStart, null, new List<GachaPack>
        {
            gachaPackDockSlot.GachaPack
        }, null);
        gachaPackDockSlot.SetState(GachaPackDockSlotState.Empty);
        GameEventHandler.Invoke(GachaPackDockEventCode.OnGachaPackDockUpdated);
    }

    public virtual void ReplacePack(GachaPackDockSlot gachaPackDockSlot, GachaPack gachaPack)
    {
        gachaPackDockSlot.GachaPack = gachaPack;
        gachaPackDockSlot.SetState(GachaPackDockSlotState.WaitToUnlock);
        UpdateSlotStates();
    }

    public virtual void CollectAndReplacePack(GachaPackDockSlot gachaPackDockSlot, GachaPack gachaPack)
    {
        GameEventHandler.Invoke(UnpackEventCode.OnUnpackStart, null, new List<GachaPack>
        {
            gachaPackDockSlot.GachaPack
        }, null);
        ReplacePack(gachaPackDockSlot, gachaPack);
    }
}