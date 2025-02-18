using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CustomToggle : MonoBehaviour
{
    [SerializeField]
    Toggle m_Toggle;
    [SerializeField]
    RectTransform m_UIHandleRectTransform;
    [SerializeField]
    Image m_Background;
    [SerializeField]
    Color m_BackgroundONColor;
    [SerializeField]
    Color m_BackgroundOFFColor;


    Vector2 m_HandlePosition;

    private void Awake()
    {
        m_Toggle = GetComponent<Toggle>();
        m_HandlePosition = m_UIHandleRectTransform.anchoredPosition;
        m_Toggle.onValueChanged.AddListener(OnSwitch);
        if (m_Toggle.isOn)
            OnSwitch(m_Toggle.isOn);
    }

    private void OnSwitch(bool on)
    {
        m_UIHandleRectTransform.anchoredPosition = on ? m_HandlePosition * -1 : m_HandlePosition;
        m_Background.color = on ? m_BackgroundONColor : m_BackgroundOFFColor;
    }

}
