using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyrphusQ.Events;

public class StringValueTextUI : ValueTextUI<string>
{
    protected override void OnValueChanged(ValueDataChanged<string> data)
    {
        m_TextAdapter.SetText(m_TextAdapter.blueprintText.Replace(Const.StringValue.PlaceholderValue, data.newValue));
        m_ContentSizeFitter?.SetLayoutHorizontal();
    }
}