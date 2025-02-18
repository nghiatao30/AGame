using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefItemSOVariable", menuName = "HyrphusQ/PPrefSO/ItemSO")]
public class PPrefItemSOVariable : ItemSOVariable
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.PropertyOrder(-1)]
#endif
    [SerializeField]
    protected string m_Key;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.PropertyOrder(-1)]
#endif
    [SerializeField]
    protected ItemListSO m_ItemSOList;

    public virtual bool hasKey => PlayerPrefs.HasKey(key);
    public virtual string key => m_Key;
    public override ItemSO value
    {
        get
        {
            if(m_RuntimeValue == null)
            {
                if (m_ItemSOList == null || m_ItemSOList.value == null)
                    return m_InitialValue;
                var itemGuid = PlayerPrefs.GetString(m_Key, m_InitialValue?.guid ?? string.Empty);
                foreach (var item in m_ItemSOList.value)
                {
                    if (item.guid == itemGuid)
                    {
                        m_RuntimeValue = item;
                        break;
                    }
                }
            }
            return m_RuntimeValue ?? m_InitialValue;
        }
        set
        {
            PlayerPrefs.SetString(m_Key, value == null ? string.Empty : value.guid);
            base.value = value;
        }
    }

#if UNITY_EDITOR
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
#endif
    public virtual void Clear()
    {
        ResetValue();
        PlayerPrefs.DeleteKey(m_Key);
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