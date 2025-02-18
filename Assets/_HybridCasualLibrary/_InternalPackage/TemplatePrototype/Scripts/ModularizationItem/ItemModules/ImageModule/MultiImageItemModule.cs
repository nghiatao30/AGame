using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CustomInspectorName(displayName: "MultiImageItemModule")]
public class MultiImageItemModule : ImageItemModule
{
    [System.Serializable]
    public struct SpriteBlock
    {
        public Color m_Tint;
        public Sprite m_Sprite;
    }

    [SerializeField]
    protected List<SpriteBlock> m_ThumbnailImageBlocks;

    public override Sprite thumbnailImage => m_ThumbnailImageBlocks[0].m_Sprite;

    public override Image CreateThumbnailImage(Image itemImage)
    {
        // Destroy item image
        itemImage = base.CreateThumbnailImage(itemImage);

        var instanceImage = itemImage;
        for (int i = 0; i < m_ThumbnailImageBlocks.Count; i++)
        {
            var spriteBlock = m_ThumbnailImageBlocks[i];
            instanceImage = i == 0 ? itemImage : Object.Instantiate(instanceImage, instanceImage.transform.position, instanceImage.transform.rotation, instanceImage.transform);
            instanceImage.color = spriteBlock.m_Tint;
            instanceImage.sprite = spriteBlock.m_Sprite;

            // Strech out image
            if(i != 0)
            {
                var parentRectTransform = instanceImage.rectTransform.parent as RectTransform;
                var parentRectSize = parentRectTransform.rect.size;
                instanceImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRectSize.x);
                instanceImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentRectSize.y);
            }
        }
        return itemImage;
    }
}