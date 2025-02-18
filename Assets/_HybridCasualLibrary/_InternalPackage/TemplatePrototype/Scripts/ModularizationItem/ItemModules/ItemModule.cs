using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemModule
{
    [SerializeField, ReadOnly]
    protected ItemSO m_ItemSO;
    public ItemSO itemSO => m_ItemSO;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {

    }
#endif

    /// <summary>
    /// Method to create a generic module (use this one instead of normal constructor)
    /// </summary>
    /// <typeparam name="T">Specificed type of module</typeparam>
    /// <param name="itemSO">ItemSO that module belongs to</param>
    /// <returns>Instance of module type T</returns>
    public static T CreateModule<T>(ItemSO itemSO) where T : ItemModule, new()
    {
        var itemModuleInstance = new T();
        itemModuleInstance.m_ItemSO = itemSO;
        return itemModuleInstance;
    }
}