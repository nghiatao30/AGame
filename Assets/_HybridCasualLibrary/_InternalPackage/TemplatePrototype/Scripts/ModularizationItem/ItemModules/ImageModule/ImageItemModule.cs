using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CustomInspectorName(displayName: "ImageItemModule")]
public abstract class ImageItemModule : ItemModule
{
    public abstract Sprite thumbnailImage { get; }

    public virtual Image CreateThumbnailImage(Image itemImage)
    {
        // Destroy item image
        if (itemImage.transform.childCount > 0)
            Object.DestroyImmediate(itemImage.transform.GetChild(0).gameObject);

        return itemImage;
    }
    public virtual Image CreateButtonImage(Image buttonImage)
    {
        return CreateThumbnailImage(buttonImage);
    }
}