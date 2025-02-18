using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnumIconDictionarySO", menuName = "HyrphusQ/CollectionSO/Dictionary/EnumIconDictionary")]
public class EnumIconDictionarySO : DictionaryVariable<SerializedEnum<SerializeEnumAttribute>, Sprite>
{
    public virtual Sprite Get(Enum enumValue) => value.Get(new SerializedEnum<SerializeEnumAttribute>(enumValue));
    public virtual void Set(Enum enumValue, Sprite icon) => value.Set(new SerializedEnum<SerializeEnumAttribute>(enumValue), icon);
}