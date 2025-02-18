using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CustomInspectorName(displayName: "MonoImageItemModule")]
public class MonoImageItemModule : ImageItemModule
{
    [SerializeField]
    protected Color m_ThumbnailTintColor = Color.white;
    // An alias for unlock image
    [SerializeField]
    protected Sprite m_ThumbnailImage;

    public virtual Color thumbnailTintColor => m_ThumbnailTintColor;
    public override Sprite thumbnailImage => m_ThumbnailImage;

    public override Image CreateThumbnailImage(Image itemImage)
    {
        // Destroy item image
        itemImage = base.CreateThumbnailImage(itemImage);
        itemImage.color = m_ThumbnailTintColor;
        itemImage.sprite = m_ThumbnailImage;
        return itemImage;
    }

    public virtual void SetThumbnailSprite(Sprite thumbnailSprite)
    {
        m_ThumbnailImage = thumbnailSprite;
    }
}