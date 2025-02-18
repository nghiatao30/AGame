using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

public static class ExtensionMethods
{
    public static bool CaseInsensitiveContains(this string text, string value,
    StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
        return text.IndexOf(value, stringComparison) >= 0;
    }
    public static void FocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint)
    {
        scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(focusPoint);
    }

    public static void FocusOnItem(this ScrollRect scrollView, RectTransform item)
    {
        scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(item);
    }

    public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint)
    {
        Vector2 contentSize = scrollView.content.rect.size;
        Vector2 viewportSize = ((RectTransform)scrollView.content.parent).rect.size;
        Vector2 contentScale = scrollView.content.localScale;

        contentSize.Scale(contentScale);
        focusPoint.Scale(contentScale);

        Vector2 scrollPosition = scrollView.normalizedPosition;
        if (scrollView.horizontal && contentSize.x > viewportSize.x)
            scrollPosition.x = Mathf.Clamp01((focusPoint.x - viewportSize.x * 0.5f) / (contentSize.x - viewportSize.x));
        if (scrollView.vertical && contentSize.y > viewportSize.y)
            scrollPosition.y = Mathf.Clamp01((focusPoint.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y));

        return scrollPosition;
    }

    public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, RectTransform item)
    {
        Vector2 itemCenterPoint = scrollView.content.InverseTransformPoint(item.transform.TransformPoint(item.rect.center));

        Vector2 contentSizeOffset = scrollView.content.rect.size;
        contentSizeOffset.Scale(scrollView.content.pivot);

        return scrollView.CalculateFocusedScrollPosition(itemCenterPoint + contentSizeOffset);
    }

    public static T GetRandom<T>(this List<T> list) where T : IRandomizable
    {
        float rValue = Random.value;
        float prob = 0f;
        foreach (var randomizable in list)
        {
            prob += randomizable.Probability;
            if (rValue <= prob)
            {
                return randomizable;
            }
        }
        return default;
    }

    public static T GetRandomRedistribute<T>(this List<T> list) where T : IRandomizable
    {
        float rValue = Random.value;
        float prob = 0f;
        float sumOfProbs = 0;
        foreach (var element in list)
        {
            sumOfProbs += element.Probability;
        }
        foreach (var randomizable in list)
        {
            prob += randomizable.Probability.Remap(0, sumOfProbs, 0, 1);
            if (rValue <= prob)
            {
                return randomizable;
            }
        }
        return default;
    }

    public static void ResetAllTriggers(this Animator animator)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type != AnimatorControllerParameterType.Trigger) continue;
            animator.ResetTrigger(param.name);
        }
    }

    public static Vector3 ScreenPointOnPlane(this Plane plane, Camera camera)
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }

    public static float FindClosest(this List<float> numbers, float target, out int index)
    {
        var closest = Mathf.Infinity;
        index = -1;

        for (int i = 0; i < numbers.Count; i++)
        {
            var difference = Mathf.Abs(numbers[i] - target);
            if (difference < closest)
            {
                closest = difference;
                index = i;
            }
        }
        if (index == -1) return 0;
        return numbers[index];
    }

    public static DateTime ToDateTime(this long timestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
        return dateTime;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static bool Approximate(this Color color, Color other, bool ignoreAlpha = false)
    {
        return Mathf.Approximately(color.r, other.r) &&
            Mathf.Approximately(color.g, other.g) &&
            Mathf.Approximately(color.b, other.b) &&
            (ignoreAlpha || Mathf.Approximately(color.a, other.a));
    }

    public static Color ToGrayscale(this Color color)
    {
        var grayscale = color.r * 0.3f + color.g * 0.59f + color.b * 0.11f;
        return new Color(grayscale, grayscale, grayscale, color.a);
    }

    public static Color ChangeBrightness(this Color color, float brightness)
    {
        Color.RGBToHSV(color, out var h, out var s, out var v);
        v *= brightness;
        return Color.HSVToRGB(h, s, v);
    }

    public static void FadeIn(this CanvasGroup canvasGroup, float duration = 0.5f, TweenCallback callBack = null, bool isIndependentUpdate = false)
    {
        DOTween.Kill(canvasGroup);
        var tween = canvasGroup.DOFade(1f, duration).SetUpdate(isIndependentUpdate);
        if (callBack != null)
        {
            tween.OnComplete(callBack);
        }
    }

    public static void FadeOut(this CanvasGroup canvasGroup, float duration = 0.5f, TweenCallback callBack = null, bool isIndependentUpdate = false)
    {
        DOTween.Kill(canvasGroup);
        var tween = canvasGroup.DOFade(0f, duration).SetUpdate(isIndependentUpdate);
        if (callBack != null)
        {
            tween.OnComplete(callBack);
        }
    }

    public static void ScaleUp(this Transform transform, float duration = 0.5f, TweenCallback callback = null, float delay = 0f)
    {
        DOTween.Kill(transform);
        var tween = transform.DOScale(Vector3.one, duration);
        if (callback != null)
        {
            tween.OnComplete(callback);
        }
        if (delay > 0f)
        {
            tween.SetDelay(delay);
        }
    }

    public static void ScaleDown(this Transform transform, float duration = 0.5f, TweenCallback callback = null, float delay = 0f)
    {
        DOTween.Kill(transform);
        var tween = transform.DOScale(Vector3.zero, duration);
        if (callback != null)
        {
            tween.OnComplete(callback);
        }
        if (delay > 0f)
        {
            tween.SetDelay(delay);
        }
    }

    public static bool IsEqualTo(this Color color, Color other)
    {
        return color.r == other.r && color.g == other.g && color.b == other.b;
    }

    public static string ToRoundedText(this int number)
    {
        if (number < 0)
            return "0";
        // Ensure number has max 3 significant digits (no rounding up can happen)
        ulong i = (ulong)Math.Pow(10, (int)Math.Max(0, Math.Log10(number) - 3));
        ulong numb = (ulong)number / i * i;

        if (numb >= 1000000000000000)
            return (numb / 1000000000000000D).ToString("0.##") + "Q";
        if (numb >= 1000000000000)
            return (numb / 1000000000000D).ToString("0.##") + "T";
        if (numb >= 1000000000)
            return (numb / 1000000000D).ToString("0.##") + "B";
        if (numb >= 1000000)
            return (numb / 1000000D).ToString("0.##") + "M";
        if (numb >= 10000)
            return (numb / 1000D).ToString("0.##") + "K";

        return numb.ToString("#,0");
    }

    public static string ToRoundedText(this float number)
    {
        if (number < 0)
            return "0";
        // Ensure number has max 3 significant digits (no rounding up can happen)
        ulong i = (ulong)Math.Pow(10, (int)Math.Max(0, Math.Log10(number) - 3));
        float numb = number / i * i;

        if (numb >= 1000000000000000)
            return (numb / 1000000000000000D).ToString("0.##") + "Q";
        if (numb >= 1000000000000)
            return (numb / 1000000000000D).ToString("0.##") + "T";
        if (numb >= 1000000000)
            return (numb / 1000000000D).ToString("0.##") + "B";
        if (numb >= 1000000)
            return (numb / 1000000D).ToString("0.##") + "M";
        if (numb >= 10000)
            return (numb / 1000D).ToString("0.##") + "K";

        return numb.ToString("#,0");
    }

    public static int RoundToInt(this float value, int maxValue = int.MaxValue)
    {
        var clampedValue = Mathf.Min(value, maxValue);
        return clampedValue == maxValue ? maxValue : Mathf.RoundToInt(clampedValue);
    }

    public static string GetNameTruncate(this string name, int maxLetter)
    {
        bool isNeedTruncate = name.Length > maxLetter;
        var truncateName = name.Substring(0, Mathf.Min(name.Length, maxLetter)) + (isNeedTruncate ? "..." : string.Empty);
        return truncateName;
    }

    public static void ChangeParticleScalingMode(this ParticleSystem particle, ParticleSystemScalingMode scalingMode)
    {
        var childParticles = particle.GetComponentsInChildren<ParticleSystem>();
        foreach (var particleSystem in childParticles)
        {
            var mainModule = particleSystem.main;
            mainModule.scalingMode = scalingMode;
        }
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static void DOFontSize(this Text text, int originalFontSize, int destinationFontSize = 300)
    {
        var sequence = DOTween.Sequence();
        sequence
            .Append(DOTween.To(getter, setter, destinationFontSize, AnimationDuration.ZERO))
            .Append(text.DOFade(1f, AnimationDuration.ZERO))
            .Append(DOTween.To(getter, setter, originalFontSize, AnimationDuration.TINY))
            .OnComplete(() =>
            {
                var textClone = GameObject.Instantiate(text, text.transform.parent);
                DOTween.To(getter_Clone, setter_Clone, destinationFontSize, 0.4f);
                textClone.DOFade(0f, 0.4f);

                int getter_Clone() => textClone.fontSize;
                void setter_Clone(int size) => textClone.fontSize = size;

                GameObject.Destroy(textClone.gameObject, AnimationDuration.SSHORT);
            })
            .Play();

        int getter() => text.fontSize;
        void setter(int size) => text.fontSize = size;
    }

    public static Color ToTransparent(this Color color)
    {
        color.a = 0;
        return color;
    }

    public static Color32 ToTransparent(this Color32 color)
    {
        color.a = 0;
        return color;
    }

    public static void ApplyRectTransformData(this RectTransform rectTransform, RectTransformData rectTransformData)
    {
        rectTransformData.ApplyDataTo(rectTransform);
    }

    public static T ToEnumOfType<T>(this string name) where T : struct, Enum
    {
        if (Enum.TryParse(name, out T result))
        {
            return result;
        }
        return default;
    }

    public static void DOCount(this TextMeshProUGUI textMeshPro, float from, float to, float duration = AnimationDuration.TINY, Func<float, string> formatNumberFunc = null)
    {
        float number = from;
        DOTween.To(Get, Set, to, duration);

        float Get()
        {
            return number;
        }
        void Set(float _number)
        {
            number = _number;
            textMeshPro.SetText(formatNumberFunc?.Invoke(number) ?? number.ToString());
        }
    }
}