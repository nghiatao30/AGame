using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsMenuContainer : SerializedMonoBehaviour
{
    [SerializeField]
    Color m_OriginalBackgroundColor;
    [SerializeField]
    Color m_OrignalTextColor;
    [SerializeField]
    Color m_OnSelectedBackgroundColor;
    [SerializeField]
    Color m_OnSelectedTextColor;
    [SerializeField]
    Dictionary<Button, List<GameObject>> m_Tabs = new Dictionary<Button, List<GameObject>>();
    private void Awake()
    {
        if (m_Tabs.Count > 0)
        {
            foreach (var i in m_Tabs)
            {
                i.Key.onClick.AddListener(() => OnTabButtonClick(i.Key));
                OnTabItemDisplayChange(i, false);
            }
            OnTabItemDisplayChange(m_Tabs.FirstOrDefault(), true);
        }
    }

    private void OnDestroy()
    {
        if (m_Tabs.Count > 0)
        {
            foreach (var i in m_Tabs)
                i.Key.onClick.RemoveListener(() => OnTabButtonClick(i.Key));
        }
    }

    private void OnTabButtonClick(Button button)
    {
        foreach (var i in m_Tabs)
        {
            OnTabItemDisplayChange(i, i.Key.Equals(button));
        }
    }

    private void OnTabItemDisplayChange(KeyValuePair<Button, List<GameObject>> keyValuePair, bool isSelected)
    {
        TextMeshProUGUI buttonText = keyValuePair.Key.GetComponentInChildren<TextMeshProUGUI>();
        Button currentButton = keyValuePair.Key;
        ColorBlock newColor = currentButton.colors;
        newColor.normalColor = isSelected ? m_OnSelectedBackgroundColor : m_OriginalBackgroundColor;
        currentButton.colors = newColor;
        buttonText.color = isSelected ? m_OnSelectedTextColor : m_OrignalTextColor;

        foreach (var i in keyValuePair.Value)
        {
            i.SetActive(isSelected);
        }
    }
}
