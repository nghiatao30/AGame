using System;
using UnityEngine;
using Sirenix.OdinInspector;
using HyrphusQ.Events;

[CreateAssetMenu(fileName = "CurrencySO", menuName = "LatteGames/Economy/CurrencySO")]
public class CurrencySO : PPrefFloatVariable
{
    public Action OnHiddenValueChange;

    [PropertyOrder(-2), PreviewField(100, ObjectFieldAlignment.Left)]
    public Sprite icon;
    [PropertyOrder(-2), PreviewField(100, ObjectFieldAlignment.Left)]
    public Sprite emittedIcon;

    [SerializeField, PropertyOrder(-1)]
    protected CurrencyType currencyType;
    [SerializeField, PropertyOrder(-1)]
    protected string spriteAssetID;

    [NonSerialized]
    protected float hiddenValue;

    public float HiddenValue
    {
        get => hiddenValue;
        set
        {
            hiddenValue = value;
            OnHiddenValueChange?.Invoke();
        }
    }
    public float ShowingValue => value - hiddenValue;
    public string TMPSprite => $"<sprite name={spriteAssetID}>";
    public CurrencyType CurrencyType => currencyType;

    public virtual bool IsAffordable(float amount)
    {
        return value >= amount;
    }

    public virtual bool SpendWithoutLogEvent(float amount)
    {
        return Spend(amount, ResourceLocation.None, string.Empty, false);
    }

    public virtual bool Spend(float amount, ResourceLocation location, string itemId, bool logEvent = true)
    {
        if (value < amount)
            return false;
        value -= amount;
        if (logEvent)
        {
            GameEventHandler.Invoke(EconomyEventCode.ConsumeResource, currencyType, amount, location, itemId);
        }

        return true;
    }

    public virtual void AcquireWithoutLogEvent(float amount)
    {
        Acquire(amount, ResourceLocation.None, string.Empty, false);
    }

    public virtual void Acquire(float amount, ResourceLocation location, string itemId, bool logEvent = true)
    {
        value += amount;
        if (logEvent)
        {
            GameEventHandler.Invoke(EconomyEventCode.AcquireResource, currencyType, amount, location, itemId);
        }
    }
}