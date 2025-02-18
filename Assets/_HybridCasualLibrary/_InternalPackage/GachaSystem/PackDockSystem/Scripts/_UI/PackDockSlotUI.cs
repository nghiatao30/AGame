using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using HyrphusQ.SerializedDataStructure;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackDockSlotUI : MonoBehaviour
{
    public UnityEvent<GachaPackDockSlot> OnUpdateRemainingTime;

    #region Fields
    // Protected & Private Modifier
    [SerializeField]
    protected SerializedDictionary<GachaPackDockSlotState, GameObject> stateContainers;
    [SerializeField]
    protected TMP_Text unlockedRemainingTimeTxt;
    [SerializeField]
    protected List<TMP_Text> unlockedDurationTxtList;
    [SerializeField]
    protected List<TMP_Text> namePackTxtList;
    [SerializeField]
    protected List<Image> thumbnailImgList;
    [SerializeField]
    protected TMP_Text speedUpPriceTxt;
    [SerializeField]
    protected TimeSpanFormat timeSpanFormat;
    [SerializeField]
    protected string speedUpPriceFormat = "{value} <size=28><voffset=0>{sprite}";

    protected Button m_Button;
    protected int gachaPackDockSlotIndex;
    protected Coroutine unlockingCoroutine;
    protected GachaPackDockSlotState currentGachaPackDockSlotState;

    protected GachaPackDockSlot gachaPackDockSlot => PackDockManager.Instance.GetGachaPackDockSlot(gachaPackDockSlotIndex);
    #endregion

    #region Properties
    // Public modifier
    // Protected & Private modifier
    #endregion

    // Protected & Private methods

    protected virtual void HandleBtnClicked()
    {
        if (gachaPackDockSlot.State == GachaPackDockSlotState.Unlocked)
        {
            PackDockManager.Instance.OpenSlot(gachaPackDockSlot);
        }
        GameEventHandler.Invoke(GachaPackDockEventCode.OnGachaPackDockSlotClicked, gachaPackDockSlot, this);
    }

    protected virtual void Awake()
    {
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(HandleBtnClicked);
        GameEventHandler.AddActionEvent(GachaPackDockEventCode.OnAddPackToDock, OnAddPackToDock);
        GameEventHandler.AddActionEvent(GachaPackDockEventCode.OnGachaPackDockUpdated, OnGachaPackDockUpdated);
    }

    protected virtual void OnDestroy()
    {
        m_Button.onClick.RemoveListener(HandleBtnClicked);
        GameEventHandler.RemoveActionEvent(GachaPackDockEventCode.OnAddPackToDock, OnAddPackToDock);
        GameEventHandler.RemoveActionEvent(GachaPackDockEventCode.OnGachaPackDockUpdated, OnGachaPackDockUpdated);
    }

    protected virtual void OnGachaPackDockUpdated()
    {
        if (IsCanUpdate())
        {
            UpdateView();
        }
    }

    protected virtual void OnAddPackToDock(object[] _params)
    {
        var slot = (GachaPackDockSlot)_params[0];
        if (slot == gachaPackDockSlot)
        {
            if (IsCanUpdate())
            {
                InitView();
                UpdateView();
            }
        }
    }

    // Public methods

    public virtual void Initialize(int gachaPackDockSlotIndex)
    {
        this.gachaPackDockSlotIndex = gachaPackDockSlotIndex;
        if (IsCanUpdate())
        {
            InitView();
            UpdateView();
        }
    }

    protected virtual void InitView()
    {
        if (gachaPackDockSlot.State != GachaPackDockSlotState.Empty)
        {
            var thumbnailSprite = gachaPackDockSlot.GachaPack.GetThumbnailImage();
            foreach (var item in thumbnailImgList)
            {
                item.sprite = thumbnailSprite;
            }
            var displayName = gachaPackDockSlot.GachaPack.GetDisplayName();
            foreach (var item in namePackTxtList)
            {
                item.text = displayName;
            }
            var unlockedTimeSpan = TimeSpan.FromSeconds(gachaPackDockSlot.GachaPack.UnlockedDuration);
            var unlockedTime = timeSpanFormat.Convert(unlockedTimeSpan);
            foreach (var item in unlockedDurationTxtList)
            {
                item.text = unlockedTime;
            }
        }
    }

    protected virtual void OnStateChanged(object[] _params)
    {
        if (IsCanUpdate())
        {
            UpdateView();
        }
    }

    protected virtual bool IsCanUpdate()
    {
        if (currentGachaPackDockSlotState != gachaPackDockSlot.State)
        {
            currentGachaPackDockSlotState = gachaPackDockSlot.State;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void UpdateView()
    {
        foreach (var item in stateContainers)
        {
            if (item.Key == gachaPackDockSlot.State)
            {
                item.Value.SetActive(true);
            }
            else
            {
                item.Value.SetActive(false);
            }
        }

        if (gachaPackDockSlot.State != GachaPackDockSlotState.Empty)
        {
            m_Button.interactable = true;
        }
        else
        {
            m_Button.interactable = false;
        }

        if (gachaPackDockSlot.State == GachaPackDockSlotState.Unlocking)
        {
            if (unlockingCoroutine != null)
            {
                StopCoroutine(unlockingCoroutine);
            }
            unlockingCoroutine = StartCoroutine(CR_Unlocking());
        }
        else
        {
            if (unlockingCoroutine != null)
            {
                StopCoroutine(unlockingCoroutine);
            }
        }
    }

    protected virtual IEnumerator CR_Unlocking()
    {
        while (!gachaPackDockSlot.CanGetReward)
        {
            var remainingTimeFromSeconds = gachaPackDockSlot.remainingTimeFromSeconds;
            var price = Mathf.CeilToInt((float)remainingTimeFromSeconds.TotalSeconds / GachaPackDockConfigs.SPEED_UP_TIME_AMOUNT_PER_CURRENCY_UNIT);
            speedUpPriceTxt.text = speedUpPriceFormat.Replace("{value}", price.ToString()).Replace("{sprite}", CurrencyManager.Instance.GetCurrencySO(GachaPackDockConfigs.SPEED_UP_CONVERT_CURRENCY_TYPE).TMPSprite);
            OnUpdateRemainingTime?.Invoke(gachaPackDockSlot);
            yield return null;
        }
        PackDockManager.Instance.CheckToUnlockSlots();
    }
}
