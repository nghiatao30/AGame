using System;
using UnityEngine;

/// <summary>
/// Dirty workaround create base class SerializedEnum to prevent CustomPropertyDrawer not working when turn on useForChildren flag.
/// Usually create base class SerializedEnum is not necessary.
/// </summary>
[Serializable]
public abstract class SerializedEnum { }
/// <summary>
/// This class is able to select any enum value on Inspector and represent for any enum value with attribute (use as tag) on EnumType.
/// </summary>
/// <typeparam name="TAttribute">Attribute (use as tag) for EnumType</typeparam>
[Serializable]
public class SerializedEnum<TAttribute> : SerializedEnum where TAttribute : Attribute
{
    #region Constructors
    public SerializedEnum()
    {

    }
    public SerializedEnum(string enumName, string enumType)
    {
        m_EnumName = enumName;
        m_EnumType = enumType;
    }
    public SerializedEnum(Enum enumValue)
    {
        m_EnumName = enumValue.ToString();
        m_EnumType = enumValue.GetType().AssemblyQualifiedName;
    }
    #endregion

    [SerializeField]
    protected string m_EnumType;
    [SerializeField]
    protected string m_EnumName;

    public Enum value => this;

    public override bool Equals(object obj)
    {
        if (value == null)
            return base.Equals(obj);
        if (obj is not SerializedEnum<TAttribute> serializedEnum)
            return false;
        return value.Equals(serializedEnum.value);
    }

    public override int GetHashCode()
    {
        if (value == null)
            return base.GetHashCode();
        return value.GetHashCode();
    }

    public static implicit operator Enum(SerializedEnum<TAttribute> serializedEnum)
    {
        if (serializedEnum == null || string.IsNullOrEmpty(serializedEnum.m_EnumType) || string.IsNullOrEmpty(serializedEnum.m_EnumName))
            return null;
        return Enum.Parse(Type.GetType(serializedEnum.m_EnumType), serializedEnum.m_EnumName) as Enum;
    }
}