using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EventCode]
public enum GachaPackDockEventCode
{
    /// <summary>
    /// This event will be raised when changing the state.
    /// <para> <typeparamref name="GachaPackDockSlot"/>: slot </para>
    /// <para> <typeparamref name="GachaPackDockSlotState"/>: oldState </para>
    /// <para> <typeparamref name="GachaPackDockSlotState"/>: newState </para>
    /// </summary>
    OnSlotStateChanged,
    /// <summary>
    /// This event will be raised when adding the pack.
    /// <para> <typeparamref name="GachaPackDockSlot"/>: slot </para>
    /// <para> <typeparamref name="PackDockSlotUI"/>: slot </para>
    /// </summary>
    OnAddPackToDock,
    /// <summary>
    /// This event will be raised when updating the gacha pack dock.
    /// </summary>
    OnGachaPackDockUpdated,
    /// <summary>
    /// This event will be raised when clicking on the gacha pack dock slot.
    /// <para> <typeparamref name="GachaPackDockSlot"/>: slot </para>
    /// </summary>
    OnGachaPackDockSlotClicked,

    /// <summary>
    /// This event will be raised when showing the replace UI.
    /// <para> <typeparamref name="GachaPackDockSlot"/>: slot </para>
    /// </summary>
    OnShowReplaceUI,
    /// <summary>
    /// This event will be raised when open gacha pack in game over UI.
    /// </summary>
    OnOpenPackNow,
}