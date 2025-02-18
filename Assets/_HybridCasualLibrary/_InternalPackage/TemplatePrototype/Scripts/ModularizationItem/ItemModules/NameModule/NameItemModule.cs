using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomInspectorName(displayName: "NameModule")]
public class NameItemModule : ItemModule
{
    [SerializeField]
    protected string m_DisplayName;

    /// <summary>
    /// Internal name of item is name of ScriptableObject
    /// </summary>
    public string internalName => m_ItemSO.name;
    public string displayName { get => m_DisplayName; set => m_DisplayName = value; }
    public string analyticsName
    {
        get
        {
            if (int.TryParse(internalName.Substring(internalName.IndexOf(" ") + 1), out int itemIndex) && itemIndex == 0)
                return internalName.Replace(" ", "").Replace("0", "Default");
            return internalName.Replace(" ", "");
        }
    }
}