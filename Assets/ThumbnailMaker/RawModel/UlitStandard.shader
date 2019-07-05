// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ProvisGames/LitStandard" {
	Properties {
		[PerRendererData]_MainTex("Texture", 2D) = "white" {}
		_ProvisGames_InputColor("Color", Color) = (1,1,1,1)

		_TintColorRGB ("Tint Color RGB", Color) = (1,1,1,1)
		_TintColorPower("Tint Color Power", Range(0.0, 1.0)) = 0.0

		_ambientPower("Ambient Power", Range(1, 10)) = 1
		_ambientSky("Ambient Sky", Color) = (1,1,1,1)
		_ambientEquator("Ambient Equator", Color) = (1,1,1,1)
		_ambientGround("Ambient Ground", Color) = (1,1,1,1)

		[MaterialToggle] _OutlineActive ("Outline On/Off", Float) = 0.0
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		_OutlineWidth ("Outline Width", Range(0, 10)) = 0.0
		
		// Alpha Texture For Animating
		[NoScaleOffset] _TransparentAnimationAlphaTexture("Animation Alpha Texture For Transparent", 2D) = "Black" {}
		_TransparentAnimationCutOffValue ("Transparent Animation CutOff Value", Float) = 1.0

		_Glossiness("Smoothness", Range(0,1)) = 0.0
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags {
			"RenderType" = "Geometry"
			"Queue" = "Geometry"
			//"RenderType" = "Transparent"
			//"Queue" = "Transparent"
		}

		Pass {
			ZWrite On
			ColorMask 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				return 0;
			}
			ENDCG
		}
		
		// 아웃라인을 이런 식으로 그리면 안됨.
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "../ProvisTransform.cginc"
			#include "../ProvisHelper.cginc"

			uniform half4 invisible = half4(0, 0, 0, 0);

			struct v2f {
				float4 pos : SV_POSITION;
			};

			half _OutlineWidth;
			v2f vert(appdata_base v)
			{
				v2f o;

				//float4 pos = UnityObjectToClipPos(v.vertex);
				//float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				//o.pos = pos + float4((normalize(worldNormal) * _OutlineWidth).xyz, 0);

				//o.pos = UnityObjectToClipPos(v.vertex);
				//float3 norm = UnityObjectToWorldNormal(v.normal);
				//float2 offset = TransformViewToProjection(norm.xy);

				//o.pos.xy += offset * o.pos.z * _OutlineWidth;

				o.pos = UnityObjectToClipPos(Scaling(v.vertex, _OutlineWidth, _OutlineWidth, _OutlineWidth));

				return o;
			}

			half _OutlineActive;
			fixed4 _OutlineColor;
			half4 frag(v2f i) : COLOR
			{
				return PROVIS_BoolIf(invisible, _OutlineColor, _OutlineActive);
			}

			ENDCG
		}

		ZWrite Off
		Lighting On
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGBA
		LOD 100
		
		CGPROGRAM
		#include "../ProvisLightHelper.cginc"
                    
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:blend addshadow //fullforwardshadows //addshadow

		//// 서피스 커스텀 라이팅은 surf 의 값을 토대로 커스텀 라이팅을 실시하여 최종 픽셀 컬러를 얻어낸다.
		// fixed4 LightingUlit(SurfaceOutput s, fixed3 lightDir, fixed atten)
		// {
		//	fixed4 c;
		//	c.rgb = s.Albedo * 0.5f;
		//	c.a = s.Alpha;
		//	return c;
		//	
		//	return fixed4(0, 0, 0, s.Alpha);
		// }

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			fixed4 screenPos;
			float3 worldPos;
		};

		// Property Arrays
		//UNITY_DECLARE_TEX2DARRAY(_Textures);

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
			//UNITY_DEFINE_INSTANCED_PROP(fixed4, _ProvisGames_InputColor)
		UNITY_INSTANCING_BUFFER_END(Props)
		
		sampler2D _MainTex;
		fixed4 _ProvisGames_InputColor;
		fixed4 _TintColorRGB;
		half _ambientPower;
		fixed3 _ambientGround, _ambientSky, _ambientEquator;

		sampler2D _TransparentAnimationAlphaTexture;
		fixed4 _TransparentAnimationCutOffValue;

		half _TintColorPower;
		half _Metallic, _Glossiness;

		fixed3 blendAmbient(fixed3 src, fixed3 ambient, fixed ratio)
		{
			return ComputeAlphaBlend(src, ambient, saturate(ratio));
		}
		float2 GetScreenUV(float2 clipPos, float UVscaleFactor)
		{
			float4 SSobjectPosition = UnityObjectToClipPos(float4(0, 0, 0, 1.0));
			float2 screenUV = float2(clipPos.x / _ScreenParams.x, clipPos.y / _ScreenParams.y);
			float screenRatio = _ScreenParams.y / _ScreenParams.x;

			screenUV.y -= 0.5;
			screenUV.x -= 0.5;

			screenUV.x -= SSobjectPosition.x / (2 * SSobjectPosition.w);
			screenUV.y += SSobjectPosition.y / (2 * SSobjectPosition.w); //switch sign depending on camera
			screenUV.y *= screenRatio;

			screenUV *= 1 / UVscaleFactor;
			screenUV *= SSobjectPosition.z;

			return screenUV;
		}


		void surf (Input IN, inout SurfaceOutputStandard o) {

			// Albedo comes from a texture tinted by color
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			//fixed4 c = fixed4(tex.rgb * _ProvisGames_InputColor.rgb, lerp((tex.a), (tex.a * _ProvisGames_InputColor.a), step(tex2D(_TransparentAnimationAlphaTexture, /*GetScreenUV(IN.worldPos, 1)*/IN.uv_MainTex).a, 0.1)));
			fixed4 c = fixed4(tex.rgb * _ProvisGames_InputColor.rgb, tex.a * _ProvisGames_InputColor.a);

			// Legacy Ambient Color
			//fixed3 ambientRGB = ComputeGradientAmbient(o.Normal.rgb, _ambientGround, _ambientSky, _ambientEquator);
			//c.rgb = (c.rgb + ambientRGB * pow(0.5, _ambientPower)) * 0.5;

			c.rgb = c.rgb + lerp(0, _TintColorRGB.rgb, _TintColorPower);
			//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) + lerp(0, _ProvisGames_InputColor, _TintColorPower); //* UNITY_ACCESS_INSTANCED_PROP(Props, _ProvisGames_InputColor);
			//fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_Textures, float3(IN.uv_Textures, UNITY_ACCESS_INSTANCED_PROP(_TextureIndex_arr, _TextureIndex))) * UNITY_ACCESS_INSTANCED_PROP(Props, _ProvisGames_InputColor);

			o.Albedo = c.rgb;
			o.Normal = o.Normal;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//o.Emission = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}