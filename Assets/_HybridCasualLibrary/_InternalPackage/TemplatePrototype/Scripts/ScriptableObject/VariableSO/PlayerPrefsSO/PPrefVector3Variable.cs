using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefVector3Variable", menuName = "HyrphusQ/PPrefSO/Vector3")]
public class PPrefVector3Variable : Vector3Variable
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.PropertyOrder(-1)]
#endif
    [SerializeField]
    protected string m_Key;

    public virtual bool hasKey => PlayerPrefs.HasKey(key);
    public virtual string key => m_Key;
    public override Vector3 value
    {
        get
        {
            var vector3AsString = PlayerPrefs.GetString(m_Key, m_InitialValue.ToString("R"));
            var values = vector3AsString.Replace("(", "").Replace(")", "").Replace(" ", "").Split(",");
            // Parse the values and create a new Vector3
            float x = float.Parse(values[0], CultureInfo.InvariantCulture);
            float y = float.Parse(values[1], CultureInfo.InvariantCulture);
            float z = float.Parse(values[2], CultureInfo.InvariantCulture);
            return new Vector3(x, y, z);
        }
        set
        {
            m_RuntimeValue = this.value;
            PlayerPrefs.SetString(m_Key, value.ToString("R"));
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
#if ODIN_INSPECTOR_3
    [Sirenix.OdinInspector.OnInspectorGUI]
    protected override void OnInspectorGUI()
    {
        GUI.enabled = false;
        UnityEditor.EditorGUILayout.Vector3Field($"Current value", value);
        GUI.enabled = true;
    }
#endif
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