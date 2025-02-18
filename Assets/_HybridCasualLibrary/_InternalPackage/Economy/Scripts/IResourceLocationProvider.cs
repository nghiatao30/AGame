using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceLocationProvider
{
    string GetItemId();
    ResourceLocation GetLocation();
}
[Serializable]
public class ResourceLocationProvider : IResourceLocationProvider
{
    public ResourceLocationProvider(ResourceLocation resourceLocation, string itemId)
    {
        m_ItemId = itemId;
        m_ResourceLocation = resourceLocation;
    }

    [SerializeField]
    private string m_ItemId;
    [SerializeField]
    private ResourceLocation m_ResourceLocation;

    public string GetItemId()
    {
        return m_ItemId;
    }

    public ResourceLocation GetLocation()
    {
        return m_ResourceLocation;
    }
}