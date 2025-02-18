using System;
using System.Collections;
using System.Collections.Generic;
using GachaSystem.Core;
using HyrphusQ.Events;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackDockOpenByGemButton : MonoBehaviour
{
    public UnityEvent OnEnoughGem;
    public UnityEvent OnNotEnoughGem;
    public UnityEvent OnEnoughGemClicked;
    public UnityEvent OnNotEnoughGemClicked;
    public UnityEvent OnOpenNow;

    [SerializeField] protected TMP_Text priceToOpen;
    [SerializeField] protected string priceToOpenFormat = "{value} <size=28><voffset=0>{sprite}";
    [SerializeField] protected Button button;
    [SerializeField] protected ResourceLocation resourceLocation;
    [SerializeField] protected string itemID;

    protected GachaPackDockSlot gachaPackDockSlot;
    protected GachaPack gachaPack;
    protected int price;
    protected CurrencySO gemCurrencySO;

    protected virtual void Awake()
    {
        button.onClick.AddListener(OnButtonClicked);
        gemCurrencySO = CurrencyManager.Instance.GetCurrencySO(GachaPackDockConfigs.SPEED_UP_CONVERT_CURRENCY_TYPE);
        gemCurrencySO.onValueChanged += OnGemChanged;
    }

    protected virtual void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
        gemCurrencySO.onValueChanged -= OnGemChanged;
    }

    protected virtual void OnButtonClicked()
    {
        if (CurrencyManager.Instance.IsAffordable(GachaPackDockConfigs.SPEED_UP_CONVERT_CURRENCY_TYPE, price))
        {
            CurrencyManager.Instance.Spend(GachaPackDockConfigs.SPEED_UP_CONVERT_CURRENCY_TYPE, price, resourceLocation, itemID);
            OpenNow();
            OnEnoughGemClicked?.Invoke();
        }
        else
        {
            OnNotEnoughGemClicked?.Invoke();
        }
    }

    public virtual void OpenNow()
    {
        if (gachaPackDockSlot != null)
        {
            PackDockManager.Instance.UnlockNow(gachaPackDockSlot);
        }
        else if (gachaPack != null)
        {
            GameEventHandler.Invoke(UnpackEventCode.OnUnpackStart, null, new List<GachaPack>
                {
                    gachaPack
                }, null);
        }
        OnOpenNow?.Invoke();
    }

    public virtual void Setup(GachaPackDockSlot gachaPackDockSlot)
    {
        this.gachaPackDockSlot = gachaPackDockSlot;
        this.gachaPack = null;
        UpdateView();
    }

    public virtual void Setup(GachaPack gachaPack)
    {
        this.gachaPack = gachaPack;
        this.gachaPackDockSlot = null;
        UpdateView();
    }

    public virtual void UpdateView()
    {
        if (gachaPackDockSlot != null)
        {
            if (gachaPackDockSlot.State == GachaPackDockSlotState.Unlocking)
            {
                price = Mathf.RoundToInt((float)gachaPackDockSlot.remainingTimeFromSeconds.TotalSeconds / GachaPackDockConfigs.SPEED_UP_TIME_AMOUNT_PER_CURRENCY_UNIT);
            }
            else
            {
                price = Mathf.RoundToInt((float)gachaPackDockSlot.GachaPack.UnlockedDuration / GachaPackDockConfigs.SPEED_UP_TIME_AMOUNT_PER_CURRENCY_UNIT);
            }
        }
        else if (gachaPack != null)
        {
            price = Mathf.RoundToInt((float)gachaPack.UnlockedDuration / GachaPackDockConfigs.SPEED_UP_TIME_AMOUNT_PER_CURRENCY_UNIT);
        }
        priceToOpen.text = priceToOpenFormat.Replace("{value}", price.ToString()).Replace("{sprite}", CurrencyManager.Instance.GetCurrencySO(GachaPackDockConfigs.SPEED_UP_CONVERT_CURRENCY_TYPE).TMPSprite);
        if (CurrencyManager.Instance.IsAffordable(GachaPackDockConfigs.SPEED_UP_CONVERT_CURRENCY_TYPE, price))
        {
            OnEnoughGem?.Invoke();
        }
        else
        {
            OnNotEnoughGem?.Invoke();
        }
    }

    protected virtual void OnGemChanged(HyrphusQ.Events.ValueDataChanged<float> valueData)
    {
        if (CurrencyManager.Instance.IsAffordable(GachaPackDockConfigs.SPEED_UP_CONVERT_CURRENCY_TYPE, price))
        {
            OnEnoughGem?.Invoke();
        }
        else
        {
            OnNotEnoughGem?.Invoke();
        }
    }
}