using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using HyrphusQ.SerializedDataStructure;
using LatteGames;
using LatteGames.Monetization;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackDockSlotInfoUI : MonoBehaviour
{
    public UnityEvent<GachaPackDockSlot> OnInit;
    public UnityEvent<GachaPackDockSlot> OnUpdateView;
    public UnityEvent<GachaPackDockSlot> OnUpdateRemainingTimeEvent;

    [SerializeField] protected CanvasGroupVisibility canvasGroupVisibility;
    [SerializeField] protected Image thumbnailImg;
    [SerializeField] protected TMP_Text nameTxt;
    [SerializeField] protected Button closeBtn;
    [SerializeField, BoxGroup("Footer Button Group")] protected Button waitToUnlockButton, waitToQueueButton;
    [SerializeField, BoxGroup("Footer Button Group")] protected PackDockOpenByGemButton packDockOpenByGemButton;
    [SerializeField, BoxGroup("Footer Button Group")] protected PackDockOpenByAdsButton packDockOpenByAdsButton;
    [SerializeField, BoxGroup("Footer Button Group")] protected RVButtonBehavior packDockOpenNowByAdsButton;

    protected PackDockSlotUI packDockSlotUI;
    protected GachaPackDockSlot gachaPackDockSlot;

    protected virtual void Awake()
    {
        GameEventHandler.AddActionEvent(GachaPackDockEventCode.OnGachaPackDockSlotClicked, OnGachaPackDockSlotClicked);
        GameEventHandler.AddActionEvent(GachaPackDockEventCode.OnSlotStateChanged, OnGachaPackDockSlotClicked);
        closeBtn.onClick.AddListener(OnCloseBtnClicked);
        waitToUnlockButton.onClick.AddListener(OnUnlockBtnClicked);
        waitToQueueButton.onClick.AddListener(OnQueueBtnClicked);
        canvasGroupVisibility.GetOnStartHideEvent().Subscribe(GetOnStartHideEvent);
        packDockOpenNowByAdsButton.OnRewardGranted += OnRewardGranted;
    }

    protected virtual void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(GachaPackDockEventCode.OnGachaPackDockSlotClicked, OnGachaPackDockSlotClicked);
        GameEventHandler.RemoveActionEvent(GachaPackDockEventCode.OnSlotStateChanged, OnGachaPackDockSlotClicked);
        closeBtn.onClick.RemoveListener(OnCloseBtnClicked);
        waitToUnlockButton.onClick.RemoveListener(OnUnlockBtnClicked);
        waitToQueueButton.onClick.RemoveListener(OnQueueBtnClicked);
        canvasGroupVisibility.GetOnStartHideEvent().Unsubscribe(GetOnStartHideEvent);
        packDockOpenNowByAdsButton.OnRewardGranted -= OnRewardGranted;
    }

    protected virtual void OnRewardGranted(RVButtonBehavior.RewardGrantedEventData data)
    {
        PackDockManager.Instance.UnlockNow(gachaPackDockSlot);
    }

    protected virtual void GetOnStartHideEvent()
    {
        packDockSlotUI.OnUpdateRemainingTime.RemoveListener(OnUpdateRemainingTime);
        gachaPackDockSlot.OnStateChanged -= OnSlotStateChanged;
    }

    protected virtual void OnCloseBtnClicked()
    {
        canvasGroupVisibility.Hide();
    }

    protected virtual void OnUnlockBtnClicked()
    {
        canvasGroupVisibility.Hide();
        PackDockManager.Instance.StartUnlock(gachaPackDockSlot);
    }

    protected virtual void OnQueueBtnClicked()
    {
        canvasGroupVisibility.Hide();
        PackDockManager.Instance.Enqueue(gachaPackDockSlot);
    }

    protected virtual void OnGachaPackDockSlotClicked(object[] _params)
    {
        gachaPackDockSlot = (GachaPackDockSlot)_params[0];
        packDockSlotUI = (PackDockSlotUI)_params[1];

        if (gachaPackDockSlot.State == GachaPackDockSlotState.Unlocked || gachaPackDockSlot.State == GachaPackDockSlotState.Empty)
        {
            return;
        }

        InitView();
        UpdateView();

        canvasGroupVisibility.Show();
    }

    protected virtual void InitView()
    {
        var gachaPack = gachaPackDockSlot.GachaPack;
        if (gachaPack != null)
        {
            thumbnailImg.sprite = gachaPack.GetThumbnailImage();
            nameTxt.text = gachaPack.GetDisplayName();
        }

        packDockOpenByGemButton.Setup(gachaPackDockSlot);
        packDockOpenByAdsButton.Setup(gachaPackDockSlot);

        packDockSlotUI.OnUpdateRemainingTime.AddListener(OnUpdateRemainingTime);
        gachaPackDockSlot.OnStateChanged += OnSlotStateChanged;

        OnUpdateRemainingTimeEvent?.Invoke(gachaPackDockSlot);
        OnInit?.Invoke(gachaPackDockSlot);
    }

    protected virtual void UpdateView()
    {
        DisableAllFooterButtons();
        if (gachaPackDockSlot.State == GachaPackDockSlotState.WaitToUnlock)
        {
            WaitToUnlockUpdateView();
        }
        else if (gachaPackDockSlot.State == GachaPackDockSlotState.WaitForAnotherUnlock)
        {
            WaitForAnotherUpdateView();
        }
        else if (gachaPackDockSlot.State == GachaPackDockSlotState.Queued)
        {
            QueuedUpdateView();
        }
        else if (gachaPackDockSlot.State == GachaPackDockSlotState.WaitToQueue)
        {
            WaitToQueueUpdateView();
        }
        else if (gachaPackDockSlot.State == GachaPackDockSlotState.Unlocking)
        {
            UnlockingUpdateView();
        }
        OnUpdateView?.Invoke(gachaPackDockSlot);
    }

    protected virtual void DisableAllFooterButtons()
    {
        waitToUnlockButton.gameObject.SetActive(false);
        waitToQueueButton.gameObject.SetActive(false);
        packDockOpenByGemButton.gameObject.SetActive(false);
        packDockOpenByAdsButton.gameObject.SetActive(false);
        packDockOpenNowByAdsButton.gameObject.SetActive(false);
    }

    protected virtual void WaitToUnlockUpdateView()
    {
        packDockOpenByGemButton.gameObject.SetActive(!gachaPackDockSlot.CanOpenNowByAds);
        packDockOpenNowByAdsButton.gameObject.SetActive(gachaPackDockSlot.CanOpenNowByAds);
        waitToUnlockButton.gameObject.SetActive(true);
    }

    protected virtual void WaitForAnotherUpdateView()
    {
        packDockOpenByGemButton.gameObject.SetActive(true);
        packDockOpenNowByAdsButton.gameObject.SetActive(gachaPackDockSlot.CanOpenNowByAds);
    }

    protected virtual void QueuedUpdateView()
    {
        packDockOpenByGemButton.gameObject.SetActive(true);
        packDockOpenNowByAdsButton.gameObject.SetActive(gachaPackDockSlot.CanOpenNowByAds);
    }

    protected virtual void WaitToQueueUpdateView()
    {
        packDockOpenByGemButton.gameObject.SetActive(true);
        waitToQueueButton.gameObject.SetActive(true);
    }

    protected virtual void UnlockingUpdateView()
    {
        packDockOpenByAdsButton.gameObject.SetActive(!gachaPackDockSlot.CanOpenNowByAds);
        packDockOpenNowByAdsButton.gameObject.SetActive(gachaPackDockSlot.CanOpenNowByAds);
        packDockOpenByGemButton.gameObject.SetActive(true);
    }

    protected virtual void OnUpdateRemainingTime(GachaPackDockSlot gachaPackDockSlot)
    {
        packDockOpenByGemButton.UpdateView();
        UnlockingUpdateView();
        OnUpdateRemainingTimeEvent?.Invoke(gachaPackDockSlot);
    }

    protected virtual void OnSlotStateChanged(GachaPackDockSlotState oldState, GachaPackDockSlotState newState)
    {
        if (newState == GachaPackDockSlotState.Unlocked || newState == GachaPackDockSlotState.Empty)
        {
            canvasGroupVisibility.Hide();
        }
        else
        {
            UpdateView();
        }
    }
}
