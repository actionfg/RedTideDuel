Shader "Custom/GroundWithSplash" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		g_splashXDisplace("SplashXDisplace", Float) = 1.0
		g_splashYDisplace("SplashYDisplace", Float) = 1.0
		g_timeCycle("TimeCycle", Float) = 1.0
		
        _SplashBumpTex("Splash Bump", 3D)="" { }
		_SplashDiffuseTex("Splash Diffuse", 3D)="" { }		
        _SplashXDisplace("XDisplace", float)=1
        _SplashYDisplace("YDisplace", float)=1        
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler3D _SplashBumpTex;
		sampler3D _SplashDiffuseTex;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		float g_splashXDisplace;
		float g_splashYDisplace;
		float g_timeCycle;
		
		static float g_KsDir = 10;

        struct vertdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
        };
            
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 wNormal;
			float3 wTangent;
			
			float4 worldToTangent0;
            float4 worldToTangent1;
            float4 worldToTangent2;  
        };

        void vert(inout vertdata v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.uv_MainTex = v.texcoord;
            o.worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
            o.wNormal = mul(unity_ObjectToWorld, v.normal);
            o.wTangent = mul(unity_ObjectToWorld, v.tangent.xyz);
            
            TANGENT_SPACE_ROTATION;
            float3x3 worldToTangent = mul(rotation, (float3x3)unity_WorldToObject);
            o.worldToTangent0 = float4(worldToTangent[0], o.wNormal.x);
            o.worldToTangent1 = float4(worldToTangent[1], o.wNormal.y);
            o.worldToTangent2 = float4(worldToTangent[2], o.wNormal.z);

         } 

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
            float3 InNormal = normalize(IN.wNormal);
            float3 Tan = normalize(IN.wTangent);
            float wetSurf = saturate(g_KsDir/2.0*saturate(InNormal.y));
            float3 binorm = normalize( cross( InNormal, Tan ) );
            if( dot( normalize(IN.worldPos) ,binorm) < 0 )
                binorm = -binorm;
//            //add the normal map from the rain bumps
//            //based on the direction of the surface and the amount of rainyness   
            float4 BumpMapVal = tex3D(_SplashBumpTex,
                           float3(IN.worldPos.x/2.0 + g_splashXDisplace, IN.worldPos.z/2.0 + g_splashYDisplace, g_timeCycle)) - 0.5;
            float3 wNorm2 = IN.wNormal + wetSurf * 2 * (BumpMapVal.x * Tan + BumpMapVal.y * binorm);
            wNorm2 = normalize(wNorm2);      
                  
            
            float3 splashDiffuse = wetSurf * tex3D(_SplashDiffuseTex, float3(IN.worldPos.xz, g_timeCycle));
			
			o.Normal = normalize(
                float3(
                    dot(wNorm2, IN.worldToTangent0.xyz),
                    dot(wNorm2, IN.worldToTangent1.xyz),
                    dot(wNorm2, IN.worldToTangent2.xyz)
                    )
                );
			//o.Albedo = c.rgb + splashDiffuse;
			o.Albedo = o.Normal;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
