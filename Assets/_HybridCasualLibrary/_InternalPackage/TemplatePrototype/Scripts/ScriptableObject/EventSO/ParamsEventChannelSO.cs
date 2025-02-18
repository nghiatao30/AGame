using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "ParamsEventChannelSO", menuName = "EventChannelSO/ParamsEventChannelSO")]
public class ParamsEventChannelSO : GenericEventChannelSO<object[]>
{
    protected virtual void OnEnable()
    {
        if (m_EventCode == null || m_EventCode.eventCode == null)
            return;
        GameEventHandler.AddActionEvent(m_EventCode, RaiseEvent);
    }
    protected virtual void OnDisable()
    {
        if (m_EventCode == null || m_EventCode.eventCode == null)
            return;
        GameEventHandler.RemoveActionEvent(m_EventCode, RaiseEvent);
    }
}