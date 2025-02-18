using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModelPrefabItemModule : ItemModule
{
    public virtual GameObject modelPrefabAsGameObject => GetModelPrefabAsGameObject();

    public virtual GameObject GetModelPrefabAsGameObject() => GetModelPrefab<Component>().gameObject;
    public abstract T GetModelPrefab<T>() where T : Component;
}
public abstract class ModelPrefabItemModule<T> : ModelPrefabItemModule where T : Component
{
    [SerializeField]
    protected T m_ModelPrefab;

    public override TComponent GetModelPrefab<TComponent>()
    {
        return m_ModelPrefab as TComponent;
    }
}