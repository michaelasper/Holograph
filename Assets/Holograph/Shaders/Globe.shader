Shader "Custom/Globe" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		_Fade("Fade",Range(0,1)) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", int) = 1 //"One"
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", int) = 0 //"Zero"

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		//Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend[_SrcBlend][_DstBlend]
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		fixed4 _Color;
		float4 _RimColor;
		float _RimPower;
		float _Fade;

		int _SrcBlend;
		int _DstBlend;

		void surf(Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color*_Fade;
			o.Albedo = c.rgb;
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb *( pow(rim, _RimPower))*_Fade;
			// Metallic and smoothness come from slider variables
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
