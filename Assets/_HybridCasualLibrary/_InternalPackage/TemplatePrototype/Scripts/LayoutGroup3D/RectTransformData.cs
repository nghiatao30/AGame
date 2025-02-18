using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RectTransformData
{
    public RectTransformData(RectTransform rectTransform)
    {
        m_AnchoredPosition3D = rectTransform.anchoredPosition3D;
        m_AnchorMin = rectTransform.anchorMin;
        m_AnchorMax = rectTransform.anchorMax;
        m_SizeDelta = rectTransform.sizeDelta;
        m_Pivot = rectTransform.pivot;
        m_OffsetMin = rectTransform.offsetMin;
        m_OffsetMax = rectTransform.offsetMax;
    }

    [SerializeField]
    private Vector3 m_AnchoredPosition3D;
    [SerializeField]
    private Vector2 m_AnchorMin, m_AnchorMax;
    [SerializeField]
    private Vector2 m_SizeDelta;
    [SerializeField]
    private Vector2 m_Pivot;
    [SerializeField]
    private Vector2 m_OffsetMin, m_OffsetMax;

    public Vector3 anchoredPosition3D
    {
        get => m_AnchoredPosition3D;
        set => m_AnchoredPosition3D = value;
    }
    public Vector2 anchoredPosition
    {
        get => m_AnchoredPosition3D;
        set => m_AnchoredPosition3D = value;
    }
    public Vector2 anchorMin
    {
        get => m_AnchorMin;
        set => m_AnchorMin = value;
    }
    public Vector2 anchorMax
    {
        get => m_AnchorMax;
        set => m_AnchorMax = value;
    }
    public Vector2 sizeDelta
    {
        get => m_SizeDelta;
        set => m_SizeDelta = value;
    }
    public Vector2 pivot
    {
        get => m_Pivot;
        set => m_Pivot = value;
    }
    public Vector2 offsetMin
    {
        get => m_OffsetMin;
        set => m_OffsetMin = value;
    }
    public Vector2 offsetMax
    {
        get => m_OffsetMax;
        set => m_OffsetMax = value;
    }

    public void ApplyDataTo(RectTransform rectTransform)
    {
        rectTransform.anchoredPosition3D = anchoredPosition3D;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.pivot = pivot;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
    }

    public void RetrieveDataFrom(RectTransform rectTransform)
    {
        m_AnchoredPosition3D = rectTransform.anchoredPosition3D;
        m_AnchorMin = rectTransform.anchorMin;
        m_AnchorMax = rectTransform.anchorMax;
        m_SizeDelta = rectTransform.sizeDelta;
        m_Pivot = rectTransform.pivot;
        m_OffsetMin = rectTransform.offsetMin;
        m_OffsetMax = rectTransform.offsetMax;
    }
}