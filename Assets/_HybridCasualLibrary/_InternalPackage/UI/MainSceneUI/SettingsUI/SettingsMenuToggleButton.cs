using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using I2.Loc;

public class SettingsMenuToggleButton : MonoBehaviour
{
    [SerializeField]
    PPrefBoolVariable m_PPrefBool;
    [SerializeField]
    Toggle m_Toggle;
    [SerializeField]
    TextMeshProUGUI m_ToggleStateText;


    private void OnEnable()
    {
        m_Toggle.onValueChanged.AddListener(ToggleAction);
        m_Toggle.isOn = m_PPrefBool.value;
        if (m_PPrefBool.value)
            m_ToggleStateText.SetText(I2LHelper.TranslateTerm(I2LTerm.Settings_ON));
        else
            m_ToggleStateText.SetText(I2LHelper.TranslateTerm(I2LTerm.Settings_OFF));
    }

    private void ToggleAction(bool isOn)
    {
        m_PPrefBool.value = isOn;
        if (isOn)
            m_ToggleStateText.SetText(I2LHelper.TranslateTerm(I2LTerm.Settings_ON));
        else
            m_ToggleStateText.SetText(I2LHelper.TranslateTerm(I2LTerm.Settings_OFF));
    }

    //private void OnDestroy()
    //{
    //    m_Toggle.onValueChanged.RemoveListener(ToggleAction);
    //}
}
