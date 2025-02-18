using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using HyrphusQ.Helpers;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "HyrphusQ/ItemSO/ItemSO")]
public class ItemSO : SerializableScriptableObject
{
    [SerializeReference, Sirenix.OdinInspector.HideIf("@true")]
    protected List<ItemModule> m_ItemModules = new List<ItemModule>();
    public List<ItemModule> itemModules => m_ItemModules;

    public virtual bool TryGetModule<T>(out T module) where T : ItemModule
    {
        module = GetModule<T>();
        return module != null;
    }

    public virtual T GetModule<T>() where T : ItemModule
    {
        var itemModule = m_ItemModules.FirstOrDefault(itemModule => itemModule.GetType().Equals(typeof(T)) || itemModule.GetType().IsSubclassOf(typeof(T)));
        if (itemModule == null)
            return null;
        return itemModule as T;
    }

    public virtual void AddModule<T>(T module) where T : ItemModule
    {
        if (module == null)
            return;
        if (TryGetModule(out T _))
            return;
        itemModules.Add(module);
    }

    public virtual bool RemoveModule<T>(T module) where T : ItemModule
    {
        if (module == null)
            return false;
        return itemModules.Remove(module);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        // Call OnValidate for all modules
        for (int i = 0; i < m_ItemModules.Count; i++)
        {
            var itemModule = m_ItemModules[i];
            itemModule.InvokeMethod<object>("OnValidate");
        }
    }
#endif
}