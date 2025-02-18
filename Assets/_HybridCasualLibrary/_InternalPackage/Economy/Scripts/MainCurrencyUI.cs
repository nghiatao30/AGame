using System.Collections;
using System.Collections.Generic;
using HyrphusQ.SerializedDataStructure;
using LatteGames;
using UnityEngine;

public class MainCurrencyUI : StackSingleton<MainCurrencyUI>
{
    [SerializeField] bool isWorldSpace;
    public SerializedDictionary<CurrencyType, RectTransform> currencyIcons = new SerializedDictionary<CurrencyType, RectTransform>();

    public Vector3 GetIconPostion(CurrencyType currencyType)
    {
        if (isWorldSpace) return Camera.main.WorldToScreenPoint(currencyIcons[currencyType].position);
        return currencyIcons[currencyType].position;
    }
    
    [SerializeField]
    CanvasGroupVisibility m_CanvasGroupVisibility;

    public void Show()
    {
        m_CanvasGroupVisibility.Show();
    }

    public void Hide()
    {
        m_CanvasGroupVisibility.Hide();
    }
}
