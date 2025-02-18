using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarItemSO", menuName = "HyrphusQ/ItemSO/AvatarItemSO")]
public class AvatarItemSO : ItemSO
{
    public Sprite thumbnail => TryGetModule(out ImageItemModule imageItemModule) ? imageItemModule.thumbnailImage : null;
}