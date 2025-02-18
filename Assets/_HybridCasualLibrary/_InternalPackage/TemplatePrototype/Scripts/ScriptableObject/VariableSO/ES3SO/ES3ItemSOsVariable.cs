using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ES3ItemSOsVariable", menuName = "HyrphusQ/ES3SO/List/ItemSO")]
public class ES3ItemSOsVariable : Variable<List<ItemSO>>
{
    protected static Dictionary<string, ItemSO> s_IdItemSODictionary = new Dictionary<string, ItemSO>();

    [SerializeField, PropertyOrder(-1)]
    protected string m_Key;
    [SerializeField]
    protected List<ItemListSO> m_ItemSOLists;

    public virtual bool hasKey => ES3.KeyExists(key);
    public virtual string key => m_Key;
    [ShowInInspector, BoxGroup("EDITOR"), LabelText("Current value"), ReadOnly, ListDrawerSettings(IsReadOnly = true)]
    public override List<ItemSO> value
    {
        get
        {
            if (m_RuntimeValue == null)
            {
                if (m_ItemSOLists != null && m_ItemSOLists.Count > 0)
                {
                    foreach (var itemSOList in m_ItemSOLists)
                    {
                        var itemSOs = itemSOList.value;
                        foreach (var itemSO in itemSOs)
                        {
                            if (!s_IdItemSODictionary.ContainsKey(itemSO.guid))
                                s_IdItemSODictionary.Add(itemSO.guid, itemSO);
                        }
                    }
                    var itemSOIds = ES3.Load(key, new List<string>());
                    if (itemSOIds != null && itemSOIds.Count > 0)
                    {
                        m_RuntimeValue = new List<ItemSO>();
                        foreach (var itemSOId in itemSOIds)
                        {
                            var itemSO = s_IdItemSODictionary.Get(itemSOId);
                            // Handle rare case which this itemSO was removed out of game by design
                            if (itemSO == null)
                            {
                                continue;
                            }
                            m_RuntimeValue.Add(itemSO);
                        }
                    }
                }
                m_RuntimeValue ??= new List<ItemSO>(m_InitialValue);
            }
            return m_RuntimeValue;
        }
        set
        {
            base.value = value;
            SaveData();
        }
    }

#if UNITY_EDITOR
    [Button("Set Value"), BoxGroup("EDITOR")]
    private void SetValue(List<ItemSO> itemSOs)
    {
        value = new List<ItemSO>(itemSOs);
    }
    [Button("Clear"), BoxGroup("EDITOR")]
    private void ClearValue()
    {
        Clear();
    }
    protected virtual void OnValidate()
    {
        GenerateSaveKey();
    }
    protected virtual void GenerateSaveKey()
    {
        if (string.IsNullOrEmpty(m_Key) && !string.IsNullOrEmpty(name))
        {
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
            m_Key = $"{name}_{guid}";
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#if ODIN_INSPECTOR_3
    [OnInspectorGUI]
    protected override void OnInspectorGUI()
    {
        // Do nothing
    }
#endif
#endif

    protected virtual void SaveData()
    {
        var itemSOs = value;
        var itemSOIds = itemSOs.Select(itemSO => itemSO.guid).ToList();
        ES3.Save(key, itemSOIds);
    }

    public virtual List<T> GetItems<T>() where T : ItemSO
    {
        return value.Cast<T>().ToList();
    }

    public virtual void Clear()
    {
        ES3.DeleteKey(key);
        m_RuntimeValue = null;
    }

    public override void OnAfterDeserialize()
    {
        // Do nothing
    }

    public override void OnBeforeSerialize()
    {
        // Do nothing
    }
}