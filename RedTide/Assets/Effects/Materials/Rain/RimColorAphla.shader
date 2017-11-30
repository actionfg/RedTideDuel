Shader "Custom/RimColorAphla" {
		Properties {
		_RimColor ("Rim Color", Color) = (1,1,0,0.0)
	   _RimPower ("Rim Power", Range(0.5,8.0)) = 2.0
	   _Alpha("Alpha",Range(0,1)) = 1
	}
	SubShader {
		Tags { "Queue" ="Transparent" "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		float4 _RimColor;
		float _RimPower;
		float _Alpha;
		struct Input {
			float3 viewDir;
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			
			o.Albedo = _RimColor.rgb;
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), IN.worldNormal));
			o.Alpha = rim*_Alpha;
			o.Emission = o.Albedo * pow (rim, _RimPower);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}