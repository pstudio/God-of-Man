// Taken from http://www.alanzucconi.com/2016/01/27/arrays-shaders-heatmaps-in-unity3d/ and then modified by me
Shader "Hidden/Heatmap" {
	Properties{
		_HeatTex("Texture", 2D) = "white" {}
	}
		SubShader{
		Tags{ "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha // Alpha blend

		Pass{
		CGPROGRAM
#pragma vertex vert             
#pragma fragment frag

#include "UnityCG.cginc"

		struct vertInput {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct vertOutput {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	sampler2D _HeatTex;
	float4 _HeatTex_ST;

	vertOutput vert(vertInput input) {
		vertOutput o;
		o.pos = UnityObjectToClipPos(input.pos);
		o.uv = TRANSFORM_TEX(input.uv, _HeatTex);
		return o;
	}

	uniform int _Data_Array_Width = 30;
	uniform int _Data_Array_Height = 20;
	uniform float _Data_Array[600];
	uniform float _Alpha = 0.5;



	half4 frag(vertOutput output) : COLOR{
		half h = _Data_Array[int((_Data_Array_Width * int(output.uv.y * _Data_Array_Height)) + int(output.uv.x * _Data_Array_Width))];

	half4 color = tex2D(_HeatTex, fixed2(h, 0.5));
	color.a = _Alpha;
	return color;
	}
		ENDCG
	}
	}
		Fallback "Diffuse"
}