using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class AdvancedSlider : Slider
{
    [SerializeField] TMP_Text denormalizedValueTMP;
    [SerializeField] Text denormalizedValueText;

    public void SetDenormalizedValue(float currentValue, float maxValue)
    {
        if (denormalizedValueTMP != null) denormalizedValueTMP.text = $"{Mathf.RoundToInt(currentValue)}/{maxValue}";
        else if(denormalizedValueText != null) denormalizedValueText.text = $"{Mathf.RoundToInt(currentValue)}/{maxValue}";
        value = currentValue / maxValue;
    }
    public void GraduallySetDenormalizedValue(float startValue, float targetValue, float maxValue, float duration, Ease easeType = Ease.OutQuad)
    {
        DOTween.To((x) => SetDenormalizedValue(x, maxValue), startValue, targetValue, duration).SetEase(easeType);
    }
}
