using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HyrphusQ.Events;
using HyrphusQ.GUI;

public abstract class ValueTextUI<T> : MonoBehaviour
{
    [SerializeField]
    protected TextAdapter m_TextAdapter;
    [SerializeField]
    protected Variable<T> m_ValueVariable;
    [SerializeField]
    protected ContentSizeFitter m_ContentSizeFitter;
    [SerializeField]
    protected CultureTextFormatConfig m_TextFormatConfig;

    protected virtual void Awake()
    {
        if (m_ContentSizeFitter == null)
            m_ContentSizeFitter = GetComponent<ContentSizeFitter>();
        m_TextAdapter.Init();
    }
    protected virtual void OnEnable()
    {
        m_ValueVariable.onValueChanged += OnValueChanged;
        OnValueChanged(new ValueDataChanged<T>(m_ValueVariable.initialValue, m_ValueVariable.value));
    }
    protected virtual void OnDisable()
    {
        m_ValueVariable.onValueChanged -= OnValueChanged;
    }

    protected abstract void OnValueChanged(ValueDataChanged<T> data);
}