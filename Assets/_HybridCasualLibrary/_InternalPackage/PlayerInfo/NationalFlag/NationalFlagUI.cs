using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;
using UnityEngine.UI;

public class NationalFlagUI : MonoBehaviour
{
    [SerializeField]
    private NationalFlagManagerSO m_NationalFlagManagerSO;
    [SerializeField]
    private Image m_NationalFlagImage;

    private void Awake()
    {
        m_NationalFlagManagerSO.playerCountryCode.onValueChanged += OnCountryCodeChanged;
        OnCountryCodeChanged(default(ValueDataChanged<string>));
    }

    private void OnDestroy()
    {
        m_NationalFlagManagerSO.playerCountryCode.onValueChanged -= OnCountryCodeChanged;
    }

    private void OnCountryCodeChanged(ValueDataChanged<string> _)
    {
        m_NationalFlagImage.sprite = m_NationalFlagManagerSO.GetCountryInfoOfLocalPlayer().nationalFlag;
    }
}