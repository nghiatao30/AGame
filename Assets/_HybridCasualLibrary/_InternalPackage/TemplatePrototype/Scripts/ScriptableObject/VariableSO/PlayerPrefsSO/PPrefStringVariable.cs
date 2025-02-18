using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefStringVariable", menuName = "HyrphusQ/PPrefSO/String")]
public class PPrefStringVariable : StringVariable
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.PropertyOrder(-1)]
#endif
    [SerializeField]
    protected string m_Key;

    public virtual bool hasKey => PlayerPrefs.HasKey(key);
    public virtual string key => m_Key;
    public override string value
    {
        get => PlayerPrefs.GetString(m_Key, m_InitialValue);
        set
        {
            m_RuntimeValue = this.value;
            PlayerPrefs.SetString(m_Key, value);
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