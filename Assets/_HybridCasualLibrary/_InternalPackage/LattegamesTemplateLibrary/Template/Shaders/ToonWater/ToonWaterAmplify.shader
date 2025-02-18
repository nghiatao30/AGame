// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ToonWaterAmplify"
{
	Properties
	{
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325,0.807,0.971,0.7254902)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.086,0.407,1,0.7490196)
		_DepthMaximumDistance("Depth Maximum Distance", Float) = 1
		_FoamColor("Foam Color", Color) = (1,1,1,1)
		_SurfaceNoise("Surface Noise", 2D) = "white" {}
		_SurfaceNoiseScrollAmount("Surface Noise Scroll Amount", Vector) = (0.03,0.03,0,0)
		_Vector0("Vector 0", Vector) = (0.03,0.03,0,0)
		_SurfaceNoiseCutoff("Surface Noise Cutoff", Range( 0 , 1)) = 0.777
		_SurfaceDistortion("Surface Distortion", 2D) = "white" {}
		_SurfaceDistortionAmount("Surface Distortion Amount", Range( 0 , 1)) = 0.27
		_FoamMaximumDistance("Foam Maximum Distance", Float) = 0.4
		_FoamMinimumDistance("Foam Minimum Distance", Float) = 0.04
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float3 viewDir;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float4 _FoamColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _FoamMaximumDistance;
		uniform float _FoamMinimumDistance;
		uniform sampler2D _CameraDepthNormalsTexture;
		uniform float _SurfaceNoiseCutoff;
		uniform sampler2D _SurfaceNoise;
		uniform sampler2D _SurfaceDistortion;
		uniform float4 _SurfaceDistortion_ST;
		uniform float _SurfaceDistortionAmount;
		uniform float2 _SurfaceNoiseScrollAmount;
		uniform float4 _SurfaceNoise_ST;
		uniform float2 _Vector0;
		uniform float4 _DepthGradientShallow;
		uniform float4 _DepthGradientDeep;
		uniform float _DepthMaximumDistance;


		float2 UnStereo( float2 UV )
		{
			#if UNITY_SINGLE_PASS_STEREO
			float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex ];
			UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
			#endif
			return UV;
		}


		float3 InvertDepthDir72_g1( float3 In )
		{
			float3 result = In;
			#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
			result *= float3(1,1,-1);
			#endif
			return result;
		}


		float4 AlphaBlend( float4 top, float4 bottom )
		{
			float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
			float alpha = top.a + bottom.a * (1 - top.a);
			return float4(color, alpha);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 UV22_g3 = ase_screenPosNorm.xy;
			float2 localUnStereo22_g3 = UnStereo( UV22_g3 );
			float2 break64_g1 = localUnStereo22_g3;
			float clampDepth69_g1 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
			#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g1 = ( 1.0 - clampDepth69_g1 );
			#else
				float staticSwitch38_g1 = clampDepth69_g1;
			#endif
			float3 appendResult39_g1 = (float3(break64_g1.x , break64_g1.y , staticSwitch38_g1));
			float4 appendResult42_g1 = (float4((appendResult39_g1*2.0 + -1.0) , 1.0));
			float4 temp_output_43_0_g1 = mul( unity_CameraInvProjection, appendResult42_g1 );
			float3 temp_output_46_0_g1 = ( (temp_output_43_0_g1).xyz / (temp_output_43_0_g1).w );
			float3 In72_g1 = temp_output_46_0_g1;
			float3 localInvertDepthDir72_g1 = InvertDepthDir72_g1( In72_g1 );
			float4 appendResult49_g1 = (float4(localInvertDepthDir72_g1 , 1.0));
			float dotResult120 = dot( ( float4( ase_worldPos , 0.0 ) - mul( unity_CameraToWorld, appendResult49_g1 ) ) , float4( i.viewDir , 0.0 ) );
			float depthDifference93 = dotResult120;
			float depthDecodedVal29 = 0;
			float3 normalDecodedVal29 = float3(0,0,0);
			DecodeDepthNormal( tex2D( _CameraDepthNormalsTexture, ase_screenPosNorm.xy ), depthDecodedVal29, normalDecodedVal29 );
			float3 ase_worldNormal = i.worldNormal;
			float dotResult32 = dot( normalDecodedVal29 , ase_worldNormal );
			float lerpResult35 = lerp( _FoamMaximumDistance , _FoamMinimumDistance , saturate( dotResult32 ));
			float distortSample96 = ( saturate( ( depthDifference93 / lerpResult35 ) ) * _SurfaceNoiseCutoff );
			float4 temp_cast_5 = (( distortSample96 - 0.01 )).xxxx;
			float4 temp_cast_6 = (( distortSample96 + 0.01 )).xxxx;
			float2 uv_SurfaceDistortion = i.uv_texcoord * _SurfaceDistortion_ST.xy + _SurfaceDistortion_ST.zw;
			float2 uv_SurfaceNoise = i.uv_texcoord * _SurfaceNoise_ST.xy + _SurfaceNoise_ST.zw;
			float2 appendResult58 = (float2(( ( _Time.y * _SurfaceNoiseScrollAmount.x ) + uv_SurfaceNoise.x ) , ( uv_SurfaceNoise.y + ( _Time.y * _Vector0.y ) )));
			float4 smoothstepResult63 = smoothstep( temp_cast_5 , temp_cast_6 , tex2D( _SurfaceNoise, ( ( ( ( (tex2D( _SurfaceDistortion, uv_SurfaceDistortion )).rg * float2( 2,2 ) ) - float2( 1,1 ) ) * _SurfaceDistortionAmount ) + appendResult58 ) ));
			float4 appendResult66 = (float4((_FoamColor).rgb , ( _FoamColor.a * smoothstepResult63 ).r));
			float4 surfaceNoiseColor99 = appendResult66;
			float4 top89 = surfaceNoiseColor99;
			float4 lerpResult24 = lerp( _DepthGradientShallow , _DepthGradientDeep , saturate( ( depthDifference93 / _DepthMaximumDistance ) ));
			float4 waterColor98 = lerpResult24;
			float4 bottom89 = waterColor98;
			float4 localAlphaBlend89 = AlphaBlend( top89 , bottom89 );
			o.Albedo = localAlphaBlend89.xyz;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float3 worldNormal : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
896;75;1104;899;4268.107;1880.936;8.487956;True;False
Node;AmplifyShaderEditor.CommentaryNode;103;-1003.866,1014.478;Inherit;False;2107.492;706.0387;Comment;15;31;30;33;29;32;34;11;10;95;35;37;39;7;40;96;Shore Foam;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;122;-923.4329,169.2907;Inherit;False;2006.259;713.0554;Comment;14;118;119;21;121;120;93;3;94;22;1;23;2;24;98;Water Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;31;-953.8664,1286.921;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;119;-703.4326,320.4233;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;118;-873.4328,465.4234;Inherit;False;Reconstruct World Position From Depth;-1;;1;e7094bcbcc80eb140b2a3dbe6a861de8;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;30;-752.9783,1269.835;Inherit;True;Global;_CameraDepthNormalsTexture;_CameraDepthNormalsTexture;11;0;Create;True;0;0;0;False;0;False;-1;None;;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;33;-515.1403,1543.517;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;121;-494.4324,446.4234;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DecodeDepthNormalNode;29;-446.9473,1285.4;Inherit;False;1;0;FLOAT4;0,0,0,0;False;2;FLOAT;0;FLOAT3;1
Node;AmplifyShaderEditor.SimpleSubtractOpNode;21;-477.5346,337.2209;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DotProductOpNode;32;-156.8839,1331.236;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;120;-309.4322,412.4235;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-113.87,1152.907;Inherit;False;Property;_FoamMaximumDistance;Foam Maximum Distance;10;0;Create;True;0;0;0;False;0;False;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-156.9929,409.9419;Inherit;False;depthDifference;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-114.3883,1238.011;Inherit;False;Property;_FoamMinimumDistance;Foam Minimum Distance;12;0;Create;True;0;0;0;False;0;False;0.04;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;34;4.049286,1329.151;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;105;-1445.303,2194.094;Inherit;False;1726.158;1076.653;Comment;19;8;57;41;48;55;6;50;43;54;49;9;56;45;51;47;58;53;5;15;Foam Particles;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;130.5402,1064.478;Inherit;False;93;depthDifference;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-1227.819,2275.656;Inherit;True;Property;_SurfaceDistortion;Surface Distortion;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;35;158.2691,1266.121;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;57;-957.9764,2957.43;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;48;-930.7976,2571.331;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;41;-907.4857,2273.478;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;6;-1039.048,2682.183;Inherit;False;Property;_SurfaceNoiseScrollAmount;Surface Noise Scroll Amount;5;0;Create;True;0;0;0;False;0;False;0.03,0.03;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;37;361.8848,1089.741;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;55;-973.379,3093.406;Inherit;False;Property;_Vector0;Vector 0;6;0;Create;True;0;0;0;False;0;False;0.03,0.03;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-722.4669,2632.667;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;50;-765.7617,2865.256;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;39;497.8069,1101.859;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;384.4774,1199.621;Inherit;False;Property;_SurfaceNoiseCutoff;Surface Noise Cutoff;7;0;Create;True;0;0;0;False;0;False;0.777;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-680.682,2272.295;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;2,2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-718.5496,3043.889;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-475.1744,2949.027;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;45;-513.1895,2273.988;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-632.1796,2377.469;Inherit;False;Property;_SurfaceDistortionAmount;Surface Distortion Amount;9;0;Create;True;0;0;0;False;0;False;0.27;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;675.8067,1101.859;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-489.7845,2722.49;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-360.5282,2278.123;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;870.1257,1101.204;Inherit;False;distortSample;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;58;-307.0017,2835.166;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;106;677.1682,1922.298;Inherit;False;1413.734;808.7924;Comment;10;62;60;4;63;64;65;66;99;12;97;Surface Foam;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-170.2651,2275.913;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;97;727.6425,2457.502;Inherit;False;96;distortSample;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;727.1682,2604.59;Inherit;False;Constant;_SMOOTHSTEP_AA;SMOOTHSTEP_AA;11;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;1084.071,2589.045;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;-24.30327,637.262;Inherit;False;93;depthDifference;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-37.64477,2244.094;Inherit;True;Property;_SurfaceNoise;Surface Noise;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;62;1077.551,2461.763;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-60.47768,767.8458;Inherit;False;Property;_DepthMaximumDistance;Depth Maximum Distance;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;1104.26,1974.917;Inherit;False;Property;_FoamColor;Foam Color;3;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;63;1310.644,2263.004;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;225.5128,652.8427;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;64;1327.36,1972.298;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;2;282.8927,436.4581;Inherit;False;Property;_DepthGradientDeep;Depth Gradient Deep;1;0;Create;True;0;0;0;False;0;False;0.086,0.407,1,0.7490196;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;1498.105,2069.113;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1;267.0355,219.2907;Inherit;False;Property;_DepthGradientShallow;Depth Gradient Shallow;0;0;Create;True;0;0;0;False;0;False;0.325,0.807,0.971,0.7254902;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;23;370.9803,659.0872;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;66;1649.488,1977.579;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;24;623.2964,532.5632;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;849.3266,538.1998;Inherit;False;waterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;1850.402,1978.483;Inherit;False;surfaceNoiseColor;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;101;1370.928,765.4366;Inherit;False;98;waterColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;100;1333.506,670.0364;Inherit;False;99;surfaceNoiseColor;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureTransformNode;15;-1198.518,2472.194;Inherit;False;8;False;1;0;SAMPLER2D;;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.CustomExpressionNode;89;1574.832,709.8771;Inherit;False;float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a))@$float alpha = top.a + bottom.a * (1 - top.a)@$return float4(color, alpha)@;4;False;2;True;top;FLOAT4;0,0,0,0;In;;Inherit;False;True;bottom;FLOAT4;0,0,0,0;In;;Inherit;False;AlphaBlend;False;False;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1824.656,627.3345;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;ToonWaterAmplify;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;1;31;0
WireConnection;29;0;30;0
WireConnection;21;0;119;0
WireConnection;21;1;118;0
WireConnection;32;0;29;1
WireConnection;32;1;33;0
WireConnection;120;0;21;0
WireConnection;120;1;121;0
WireConnection;93;0;120;0
WireConnection;34;0;32;0
WireConnection;35;0;10;0
WireConnection;35;1;11;0
WireConnection;35;2;34;0
WireConnection;41;0;8;0
WireConnection;37;0;95;0
WireConnection;37;1;35;0
WireConnection;49;0;48;0
WireConnection;49;1;6;1
WireConnection;39;0;37;0
WireConnection;43;0;41;0
WireConnection;54;0;57;0
WireConnection;54;1;55;2
WireConnection;56;0;50;2
WireConnection;56;1;54;0
WireConnection;45;0;43;0
WireConnection;40;0;39;0
WireConnection;40;1;7;0
WireConnection;51;0;49;0
WireConnection;51;1;50;1
WireConnection;47;0;45;0
WireConnection;47;1;9;0
WireConnection;96;0;40;0
WireConnection;58;0;51;0
WireConnection;58;1;56;0
WireConnection;53;0;47;0
WireConnection;53;1;58;0
WireConnection;60;0;97;0
WireConnection;60;1;12;0
WireConnection;5;1;53;0
WireConnection;62;0;97;0
WireConnection;62;1;12;0
WireConnection;63;0;5;0
WireConnection;63;1;62;0
WireConnection;63;2;60;0
WireConnection;22;0;94;0
WireConnection;22;1;3;0
WireConnection;64;0;4;0
WireConnection;65;0;4;4
WireConnection;65;1;63;0
WireConnection;23;0;22;0
WireConnection;66;0;64;0
WireConnection;66;3;65;0
WireConnection;24;0;1;0
WireConnection;24;1;2;0
WireConnection;24;2;23;0
WireConnection;98;0;24;0
WireConnection;99;0;66;0
WireConnection;89;0;100;0
WireConnection;89;1;101;0
WireConnection;0;0;89;0
ASEEND*/
//CHKSM=2B5784D293144FD760F44CDFA75EB9321FFB2044