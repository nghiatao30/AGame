using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EventCode]
public enum ItemManagementEventCode
{
    /// <summary>
    /// This event is raised when item in-use is changed
    /// <para> <typeparamref name="ItemManagerSO"/>: itemManagerSO </para>
    /// <para> <typeparamref name="ItemSO"/>: itemInUse </para>
    /// </summary>
    OnItemUsed,
    /// <summary>
    /// This event is raised when item is selected
    /// <para> <typeparamref name="ItemManagerSO"/>: itemManagerSO </para>
    /// <para> <typeparamref name="ItemSO"/>: currentSelectedItem </para>
    /// <para> <typeparamref name="ItemSO"/>: previousSelectedItem </para>
    /// </summary>
    OnItemSelected,
    /// <summary>
    /// This event is raised when item is unlocked
    /// <para> <typeparamref name="ItemManagerSO"/>: itemManagerSO </para>
    /// <para> <typeparamref name="ItemSO"/>: unlockedItem </para>
    /// </summary>
    OnItemUnlocked
}