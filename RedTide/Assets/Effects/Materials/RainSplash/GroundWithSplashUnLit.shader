Shader "Unlit/GroundWithSplashUnLit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		g_splashXDisplace("SplashXDisplace", Float) = 1.0
		g_splashYDisplace("SplashYDisplace", Float) = 1.0
		g_timeCycle("TimeCycle", Float) = 1.0
		
        _SplashBumpTex("Splash Bump", 3D)="" { }
		_SplashDiffuseTex("Splash Diffuse", 3D)="" { }		
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				
				float3 worldPos : TEXCOORD1;
				float3 wNormal : TEXCOORD2;
				float3 wTangent : TEXCOORD3;
				
			};

			sampler2D _MainTex;
			sampler3D _SplashBumpTex;
			sampler3D _SplashDiffuseTex;
			
			float4 _MainTex_ST;
			float g_splashXDisplace;
			float g_splashYDisplace;
			float g_timeCycle;
			
			static float g_KsDir = 10;
			static float3 g_beta = float3(0.04, 0.04, 0.04);
			static float3 g_lightPos = float3(0,3,0); //the directional light in world space 
			static float g_Kd = 0.1;
			static float dirLightIntensity = 1.0;
			static float g_specPower = 20;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				
				o.worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
				o.wNormal = mul(unity_ObjectToWorld, v.normal);
				o.wTangent = mul(unity_ObjectToWorld, v.tangent.xyz);
								
				return o;
			}
			
			float3 phaseFunctionSchlick(float cosTheta)
			{
			   float k = -0.2; 
			   float p = (1-k*k)/(pow(1+k*cosTheta,2) );
			   return float3(p,p,p);
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, IN.uv);
				// apply fog
				UNITY_APPLY_FOG(IN.fogCoord, col);
//							return col;
	
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
				float3 N = normalize(wNorm2);      					  
				
				float3 splashDiffuse = wetSurf * tex3D(_SplashDiffuseTex, float3(IN.worldPos.xz, g_timeCycle));
				col = float4(splashDiffuse, 1.0);
				// TODO 进行光照计算
				    //reflection of the scene-----------------------------------------------------------
				float3 viewVec = IN.worldPos - _WorldSpaceCameraPos;
				float Dvp = length(viewVec);
				float3 V =  viewVec/Dvp; 
				float3 exDir = float3( exp(-g_beta.x*Dvp),  exp(-g_beta.y*Dvp),  exp(-g_beta.z*Dvp)  );
				float3 reflVect = reflect(V, N);
				
				//directional light-----------------------------------------------------------------
				float3 lightDir = g_lightPos - IN.worldPos;
				float3 lightDirNorm = normalize(lightDir);
				float3 SDir = normalize( g_lightPos - _WorldSpaceCameraPos);
				float cosGammaDir = dot(SDir, V);
				float dirLighting = g_Kd*dirLightIntensity*saturate( dot( N,lightDirNorm ) );
				//diffuse
				float3 diffuseDirLight = dirLighting*exDir;        
				//airlight
				float3 dirAirLight = phaseFunctionSchlick(cosGammaDir)* dirLightIntensity*float3(1-exDir.x,1-exDir.y,1-exDir.z);
				//specular
				float3 specularDirLight = saturate( pow(  dot(lightDirNorm,reflVect),g_specPower)) * dirLightIntensity * g_KsDir * exDir; 
				
				float4 outputColor = float4( dirAirLight.xyz + col.xyz*diffuseDirLight.xyz 
                          + splashDiffuse*specularDirLight ,1); 

//				return outputColor;
				return tex3D(_SplashBumpTex, float3(IN.worldPos.xz, g_timeCycle)) ;
			}
			ENDCG
		}
	}
}
