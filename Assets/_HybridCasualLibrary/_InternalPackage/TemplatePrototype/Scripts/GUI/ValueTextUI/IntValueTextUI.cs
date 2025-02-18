using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyrphusQ.Events;

public class IntValueTextUI : ValueTextUI<int>
{
    protected override void OnValueChanged(ValueDataChanged<int> data)
    {
        var textValue = m_TextFormatConfig != null ? m_TextFormatConfig.Format(data.newValue) : $"{data.newValue}";
        m_TextAdapter.SetText(m_TextAdapter.blueprintText.Replace(Const.StringValue.PlaceholderValue, textValue));
        m_ContentSizeFitter?.SetLayoutHorizontal();
    }
}