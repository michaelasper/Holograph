Shader "Custom/NodeGlowZoom"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Fade("Fade",Range(0,1)) = 1
		_Pulse("Pulse",Range(0,1)) = 1
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", int) = 1 //"One"
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", int) = 0 //"Zero"
		_ZWrite("ZWrite",int) = 0 //0 Off, 1 On
	}

		SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		Blend[_SrcBlend][_DstBlend]
		//ZWrite Off
		ZWrite[_ZWrite]
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		sampler2D _MainTex;
	int _SrcBlend;
	int _DstBlend;
	int _ZWrite;
	float4 _Color;
	float _Fade;
	float _Pulse;
	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0; // texture coordinate
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0; // texture coordinate
	};


		v2f vert(appdata v)
	{
		v2f o;
		o.uv = v.uv;
		o.vertex = UnityObjectToClipPos(v.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
	fixed4 col = tex2D(_MainTex, i.uv) * _Color * _Fade*_Pulse;
	return col;
	}
		ENDCG
	}
	}
}

