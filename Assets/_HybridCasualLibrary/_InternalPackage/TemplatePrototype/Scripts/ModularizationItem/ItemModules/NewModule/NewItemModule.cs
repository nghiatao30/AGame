using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomInspectorName("NewModule")]
public class NewItemModule : ItemModule
{
    public event Action<NewItemModule> onNewStateChanged;

    [SerializeField]
    protected bool m_DefaultIsNew;

    protected string saveKey => $"{nameof(NewItemModule)}_{m_ItemSO.guid}";

    public virtual bool isNew
    {
        get
        {
            return PlayerPrefs.GetInt(saveKey, m_DefaultIsNew ? 1 : 0) == 1;
        }
        set
        {
            var previousValue = isNew;
            var currentValue = value;
            PlayerPrefs.SetInt(saveKey, value ? 1 : 0);
            if (currentValue != previousValue)
                onNewStateChanged?.Invoke(this);
        }
    }
}