// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Unlit Transparent Alpha Fade" {
	Properties {
	  _MainTex ("Base (RGBA)", 2D) = "white" {}
	  _Color ("Color", Color) = (1,1,1,1)
	  _Fade ("Fade", Float) = 0.5
	  _FadeRange ("Sharpness", Range(0,10)) = 1.0
	}
	SubShader {
	  Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

	  Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
			
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv_MainTex : TEXCOORD0;
				};
			
				float4 _MainTex_ST;
				
				v2f vert(appdata_base v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
			
				sampler2D _MainTex;
				float _Fade;
				float _FadeRange;
				half4 _Color;
			
				float4 frag(v2f IN) : COLOR {
					half4 c = tex2D (_MainTex, IN.uv_MainTex);
					float fade = _Fade * _FadeRange;
					c.w = clamp(lerp(fade - _FadeRange, fade + _FadeRange, c.w), 0.0, 1.0);
					return c * _Color;
				}
			ENDCG
		}
	}
}