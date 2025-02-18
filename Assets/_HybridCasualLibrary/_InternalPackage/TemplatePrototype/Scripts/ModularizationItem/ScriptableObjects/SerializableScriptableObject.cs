using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SerializableScriptableObject : ScriptableObject
{
    [SerializeField, HideInInspector]
    protected string m_guid;
    public string guid => m_guid;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        var assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        m_guid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
    }
    protected virtual void Reset()
    {
        OnValidate();
    }
#endif
}