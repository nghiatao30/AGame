using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefDatetimeVariable", menuName = "HyrphusQ/PPrefSO/DateTime")]
public class PPrefDatetimeVariable : Variable<DateTime>
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.PropertyOrder(-1)]
#endif
    [SerializeField]
    protected string m_Key;
    [SerializeField]
    protected string m_InitialValueInSeconds;

    public virtual bool hasKey => PlayerPrefs.HasKey(key);
    public virtual string key => m_Key;
    public override DateTime value
    {
        get
        {
            long time = long.Parse(PlayerPrefs.GetString(m_Key, initialValueInTicks.ToString()));
            return new DateTime(time);
        }
        set
        {
            m_RuntimeValue = this.value;
            PlayerPrefs.SetString(m_Key, value.Ticks.ToString());
            base.value = value;
        }
    }
    protected virtual long initialValueInTicks
    {
        get
        {
            if (string.IsNullOrEmpty(m_InitialValueInSeconds)) return DateTime.Now.Ticks;
            if (!double.TryParse(m_InitialValueInSeconds, out var seconds)) return DateTime.Now.Ticks;
            var initialValue = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
            return initialValue.Ticks;
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