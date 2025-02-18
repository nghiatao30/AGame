using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackDockUnlockTime : MonoBehaviour
{
    [SerializeField]
    protected TMP_Text timeTxt;
    [SerializeField]
    protected TimeSpanFormat timeSpanFormat;

    public virtual void UpdateView(GachaPackDockSlot slot)
    {
        if (slot.State == GachaPackDockSlotState.Unlocking)
        {
            timeTxt.text = timeSpanFormat.Convert(slot.remainingTimeFromSeconds);
        }
        else
        {
            timeTxt.text = timeSpanFormat.Convert(TimeSpan.FromSeconds(slot.GachaPack.UnlockedDuration));
        }
    }
}
