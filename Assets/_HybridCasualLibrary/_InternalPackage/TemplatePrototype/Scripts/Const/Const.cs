using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Const
{
    public static class ShaderProperty
    {
        public static readonly int MainColor_ID = Shader.PropertyToID("_Color");
        public static readonly int TintColor_ID = Shader.PropertyToID("_Tint");
        public static readonly int EmissionColor_ID = Shader.PropertyToID("_EmissionColor");
        public static readonly int MainTexture_ID = Shader.PropertyToID("_MainTex");
        public static readonly int MaskTexture_ID = Shader.PropertyToID("_MaskTex");
        public static readonly int SecondTexture_ID = Shader.PropertyToID("_SecondTex");
        public static readonly int EmissionTexture_ID = Shader.PropertyToID("_EmissionMap");
        public static readonly int NormalTexture_ID = Shader.PropertyToID("_BumpMap");
    }
    public static class UnityTag
    {
        public const string Default = "Untagged";
        public const string Player = "Player";
        public const string MainCamera = "MainCamera";
        public const string Character = "Character";
    }
    public static class UnityLayerMask
    {
        public static readonly int Default = LayerMask.NameToLayer(nameof(Default));
        public static readonly int UI = LayerMask.NameToLayer(nameof(UI));
        public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static readonly int Player = LayerMask.NameToLayer("Player");
        public static readonly int RenderTextureTarget = LayerMask.NameToLayer("RenderTextureTarget");
        public static readonly int Character = LayerMask.NameToLayer("Character");
    }
    public static class IntValue
    {
        public const int Invalid = -1;
        public const int Zero = 0;
        public const int One = 1;
    }
    public static class FloatValue
    {
        public const float Invalid = -1f;
        public const float ZeroF = 0f;
        public const float Half = 0.5f;
        public const float OneF = 1f;
    }
    public static class StringValue
    {
        public const string NonAvailable = "N/A";
        public const string PlaceholderValue = "{value}";
    }
    public static class ColorValue
    {
        public static readonly Color DefaultHSVColor = new Color(0.5f, 0.25f, 0.25f, 0.5f);
    }
}