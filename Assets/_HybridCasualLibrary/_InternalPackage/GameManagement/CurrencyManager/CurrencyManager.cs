using System.Collections;
using System.Collections.Generic;
using LatteGames.Template;
using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>
{
    [SerializeField]
    protected CurrencyIconsEmitter currencyIconsEmitter;

    public virtual CurrencySO GetCurrencySO(CurrencyType currencyType) => GetCurrencyDictionarySO()[currencyType];
    public virtual CurrencyDictionarySO GetCurrencyDictionarySO() => currencyIconsEmitter.CurrencySODictionary;
    public virtual CurrencyEmitterConfigSO GetEmitterConfigSO() => currencyIconsEmitter.currencyEmitterConfigSO;

    public virtual CurrencyIconsEmitter.Emission CreateEmission(CurrencyType currencyType, float amountOfCurrency, Vector3 from, Vector3 to, float? emitDuration = null, float? moveDuration = null, float? startRadius = null, float? endRadius = null)
    {
        var iconAmount = Mathf.RoundToInt(currencyIconsEmitter.currencyEmitterConfigSO.currencyIconAmountCurve[currencyType].Evaluate(amountOfCurrency));
        return CreateEmission(currencyType, iconAmount, from, to, emitDuration, moveDuration, startRadius, endRadius);
    }

    public virtual CurrencyIconsEmitter.Emission CreateEmission(CurrencyType currencyType, int iconAmount, Vector3 from, Vector3 to, float? emitDuration = null, float? moveDuration = null, float? startRadius = null, float? endRadius = null)
    {
        var _emitDuration = emitDuration ?? GetEmitterConfigSO().emitDuration;
        var _moveDuration = moveDuration ?? GetEmitterConfigSO().moveDuration;
        var _startRadius = startRadius ?? GetEmitterConfigSO().startRadius;
        var _endRadius = endRadius ?? GetEmitterConfigSO().endRadius;
        var emission = currencyIconsEmitter.CreateEmission(currencyType, from, to, iconAmount, _emitDuration, _moveDuration, _startRadius, _endRadius);
        emission.OnEachMoveComplete(OnEachMoveComplete);

        void OnEachMoveComplete()
        {
            switch (currencyType)
            {
                case CurrencyType.Standard:
                    {
                        SoundManager.Instance.PlaySFX(GeneralSFX.UIFillUpStandardCurrency);
                        HapticManager.Instance.PlayFlashHaptic(HapticTypes.HeavyImpact);
                        break;
                    }
                case CurrencyType.Premium:
                    {
                        SoundManager.Instance.PlaySFX(GeneralSFX.UIFillUpPremiumCurrency);
                        HapticManager.Instance.PlayFlashHaptic(HapticTypes.HeavyImpact);
                        break;
                    }
            }
        }
        return emission;
    }

    public virtual void PlayAcquireAnimation(CurrencyType currencyType, float amountOfCurrency, Vector3 from, System.Action callback = null, System.Action<float> onStartEmission = null, System.Action<float> onEachMoveComplete = null)
    {
        var targetPos = MainCurrencyUI.Instance.GetIconPostion(currencyType);
        PlayAcquireAnimation(currencyType, amountOfCurrency, from, targetPos, callback, onStartEmission, onEachMoveComplete);
    }

    public virtual void PlayAcquireAnimation(CurrencyType currencyType, float amountOfCurrency, Vector3 from, System.Action callback = null)
    {
        var targetPos = MainCurrencyUI.Instance.GetIconPostion(currencyType);
        var currencySO = GetCurrencySO(currencyType);
        PlayAcquireAnimation(currencyType, amountOfCurrency, from, targetPos, callback, (amountOfCurrency) =>
        {
            currencySO.HiddenValue += amountOfCurrency;
        }, (currencyValuePerMove) =>
        {
            currencySO.HiddenValue -= currencyValuePerMove;
        });
    }

    public virtual void PlayAcquireAnimation(CurrencyType currencyType, float amountOfCurrency, Vector3 from, Vector3 to, System.Action callback = null, System.Action<float> onStartEmission = null, System.Action<float> onEachMoveComplete = null)
    {
        var iconAmount = Mathf.RoundToInt(currencyIconsEmitter.currencyEmitterConfigSO.currencyIconAmountCurve[currencyType].Evaluate(amountOfCurrency));
        var currencySO = GetCurrencySO(currencyType);
        var currencyValuePerMove = (float)amountOfCurrency / iconAmount;

        onStartEmission?.Invoke(amountOfCurrency);
        var emission = CreateEmission(currencyType, iconAmount, from, to);
        emission.OnEachMoveComplete(OnEachMoveComplete);
        emission.OnAllMoveComplete(OnAllMoveComplete);
        emission.RadiateEmit();

        void OnEachMoveComplete()
        {
            onEachMoveComplete?.Invoke(currencyValuePerMove);
        }
        void OnAllMoveComplete()
        {
            callback?.Invoke();
        }
    }

    public virtual void AcquireWithAnimation(CurrencyType currencyType, float amount, ResourceLocation location, string itemId, Vector3 from, System.Action callback = null, System.Action<float> onStartEmission = null, System.Action<float> onEachMoveComplete = null)
    {
        Acquire(currencyType, amount, location, itemId);
        var currencySO = GetCurrencySO(currencyType);
        PlayAcquireAnimation(currencyType, amount, from, callback, 
        (amountOfCurrency) =>
        {
            currencySO.HiddenValue += amountOfCurrency;
            onStartEmission?.Invoke(amountOfCurrency);
        }, 
        (currencyValuePerMove) =>
        {
            currencySO.HiddenValue -= currencyValuePerMove;
            onEachMoveComplete?.Invoke(currencyValuePerMove);
        });
    }

    public virtual bool IsAffordable(CurrencyType currencyType, float amount)
    {
        var currencySO = GetCurrencySO(currencyType);
        return currencySO.IsAffordable(amount);
    }

    public virtual void Acquire(CurrencyType currencyType, float amount, ResourceLocation location, string itemId)
    {
        var currencySO = GetCurrencySO(currencyType);
        currencySO.Acquire(amount, location, itemId);
    }

    public virtual void AcquireWithoutLogEvent(CurrencyType currencyType, float amount)
    {
        var currencySO = GetCurrencySO(currencyType);
        currencySO.AcquireWithoutLogEvent(amount);
    }

    public virtual bool Spend(CurrencyType currencyType, float amount, ResourceLocation location, string itemId)
    {
        var currencySO = GetCurrencySO(currencyType);
        return currencySO.Spend(amount, location, itemId);
    }

    public virtual bool SpendWithoutLogEvent(CurrencyType currencyType, float amount)
    {
        var currencySO = GetCurrencySO(currencyType);
        return currencySO.SpendWithoutLogEvent(amount);
    }

    public virtual CurrencySO this[CurrencyType currencyType]
    {
        get => GetCurrencySO(currencyType);
    }
}