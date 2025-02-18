using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;

public abstract class BaseEventChannelSO : ScriptableObject
{
    [TextArea]
    [SerializeField]
    protected string m_Description;
    [SerializeField]
    protected EventCode m_EventCode;
}