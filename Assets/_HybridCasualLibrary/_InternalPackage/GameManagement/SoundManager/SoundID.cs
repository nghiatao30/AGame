using System;

public class SoundIDAttribute : Attribute
{

}
[Serializable]
public class SoundID : SerializedEnum<SoundIDAttribute>
{
    public SoundID() { }
    public SoundID(string enumName, string enumType) : base(enumName, enumType) { }
}