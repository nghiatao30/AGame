using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEventChannelSO<T> : BaseEventChannelSO
{
    public event Action<T> onEventRaised;

    public void RaiseEvent(T parameter)
    {
        onEventRaised?.Invoke(parameter);
    }
}