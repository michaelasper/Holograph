Shader "Custom/Window" {
		Properties{
			_MainColor("Main Color", Color) = (1,1,1,1)
			_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
			_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
			_Fade("Fade",Range(0,1)) = 1
			_Pulse("Pulse",Range(0,1)) = 1
			[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", int) = 1 //"One"
			[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", int) = 0 //"Zero"
		}
			SubShader{
				Tags{ "Queue" = "Transparent"}
			Blend[_SrcBlend][_DstBlend]
			CGPROGRAM
			#pragma surface surf Lambert
			struct Input {
			float3 viewDir;
		};
		float4 _MainColor;
		float4 _RimColor;
		float _RimPower;
		float _Fade;
		float _Pulse;

		int _SrcBlend;
		int _DstBlend;
		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _MainColor.rgb*_Fade*_Pulse;
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb *(.25f+ pow(rim, _RimPower))*_Fade*_Pulse;
		}
		ENDCG
		}
			Fallback "Diffuse"
	}