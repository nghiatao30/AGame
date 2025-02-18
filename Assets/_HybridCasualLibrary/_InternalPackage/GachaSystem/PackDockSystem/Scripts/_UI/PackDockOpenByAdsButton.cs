using System.Collections;
using System.Collections.Generic;
using LatteGames.Monetization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackDockOpenByAdsButton : MonoBehaviour
{
    [SerializeField] protected RVButtonBehavior button;

    protected GachaPackDockSlot gachaPackDockSlot;

    protected virtual void Awake()
    {
        button.OnRewardGranted += OnRewardGranted;
    }

    protected virtual void OnDestroy()
    {
        button.OnRewardGranted -= OnRewardGranted;
    }

    public virtual void Setup(GachaPackDockSlot gachaPackDockSlot)
    {
        this.gachaPackDockSlot = gachaPackDockSlot;
    }

    protected virtual void OnRewardGranted(RVButtonBehavior.RewardGrantedEventData data)
    {
        PackDockManager.Instance.ReduceUnlockTime(gachaPackDockSlot);
    }
}
