using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CustomInspectorName("GameObjectModelPrefabModule")]
public class GameObjectModelPrefabItemModule : ModelPrefabItemModule
{
    [SerializeField]
    protected GameObject m_ModelPrefab;

    public virtual void SetModelPrefab(GameObject modelPrefab)
    {
        m_ModelPrefab = modelPrefab;
    }

    public override T GetModelPrefab<T>()
    {
        return m_ModelPrefab.transform as T;
    }
}