Shader "UI/HorizontalGradient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Angle ("Rotate Angles", Range(-180, 180)) = 0.0
        _GradientBias("Gradient Bias", Range(0,1)) = 0.2
        _LeftColor ("Left Color", Color) = (0, 0, 0, 0)
        _RightColor ("Right Color", Color) = (1, 1, 1, 1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #define Deg2Rad 0.0174532925

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            half _GradientBias, _Angle;
            fixed4 _LeftColor, _RightColor;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float2 _ClipSoftness;

            float2 rotateVector(float2 uv, float2 pivot, float rotation)
            {
                float cosa = cos(rotation);
                float sina = sin(rotation);
                uv -= pivot;
                return float2 (
                    cosa * uv.x - sina * uv.y,
                    cosa * uv.y + sina * uv.x
                    ) + pivot;
            }

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = v.texcoord;

                OUT.color = v.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Get main color shape
                fixed4 mainColor = tex2D(_MainTex, IN.texcoord.xy);
                float2 uv = rotateVector(IN.texcoord.xy, float2(0.5, 0.5), _Angle * Deg2Rad);
                fixed r = min(_GradientBias, 1.0 - _GradientBias);
                fixed4 color = lerp(_LeftColor, _RightColor, smoothstep(_GradientBias - r, _GradientBias + r, uv.x));
                color.rgb *= IN.color.rgb;
                color.a *= IN.color.a * mainColor.a;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                if (color.a > 0.001)
                {
                    float2 softness = float2(_ClipSoftness.x, _ClipSoftness.y);
                    float distanceToEdgeX = min(abs(IN.worldPosition.x - _ClipRect.x), abs(IN.worldPosition.x - _ClipRect.z));
                    float distanceToEdgeY = min(abs(IN.worldPosition.y - _ClipRect.y), abs(IN.worldPosition.y - _ClipRect.w));
                    float alphaX = smoothstep(0, softness.x * 0.575, distanceToEdgeX);
                    float alphaY = smoothstep(0, softness.y * 0.575, distanceToEdgeY);
                    color.a *= alphaX * alphaY;
                }
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}