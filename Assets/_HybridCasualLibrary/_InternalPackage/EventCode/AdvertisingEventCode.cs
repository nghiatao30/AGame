using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EventCode]
public enum AdvertisingEventCode
{
    /// <summary>
    /// This event is raised when showing an Ad
    /// <para> <typeparamref name="AdsType"/>: adsType </para>
    /// <para> <typeparamref name="Location"/>: location </para>
    /// </summary>
    OnShowAd,
    /// <summary>
    /// This event is raised when closing an Ad
    /// <para> <typeparamref name="AdsType"/>: adsType </para>
    /// <para> <typeparamref name="Location"/>: location </para>
    /// <para> <typeparamref name="bool"/>: isSuccess </para>
    /// </summary>
    OnCloseAd,
    OnShowAdNotAvailableNotice
}