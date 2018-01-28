// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Unlit Transparency From Texture" {
	Properties {
	  _MainTex ("Base (RGBA)", 2D) = "white" {}
	  _AlphaTex ("Alpha (RGBA)", 2D) = "white" {}
	  _Color("Color", Color) = (1,1,1,1)
	  _FadeValue ("Fade", float) = 0.5
	  _Sharpness ("Sharpness", Range(0,10)) = 0
	}
	SubShader {
	  Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
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
					float2 uv_AlphaTex : TEXCOORD1;
				};
			
				float4 _MainTex_ST, _AlphaTex_ST;
			
				v2f vert(appdata_base v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uv_AlphaTex = TRANSFORM_TEX(v.texcoord, _AlphaTex);
					return o;
				}
			
				sampler2D _MainTex, _AlphaTex;
				float _Sharpness, _FadeValue;
				half4 _Color;
			
				float4 frag(v2f IN) : COLOR {
					half4 c = tex2D (_MainTex, IN.uv_MainTex);
					c.a *= tex2D(_AlphaTex, IN.uv_AlphaTex).a;
					float sharpness = _Sharpness * 0.5;
					c.a = clamp(lerp(_FadeValue - sharpness, _FadeValue + sharpness, c.a), 0, 1);
					c.rgb *= _Color.rgb;
					return c;
				}
			ENDCG
		}
	}
}