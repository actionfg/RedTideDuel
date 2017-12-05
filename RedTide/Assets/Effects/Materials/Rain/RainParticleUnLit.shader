Shader "Unlit/RainParticleUnLit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _RainTexArray("Tex", 2DArray) = "" {}
		_Alpha("Alpha",Range(0,1)) = 1

	}
	SubShader
	{
		Tags {"Queue" ="Transparent" "RenderType"="Transparent" }
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                float4 customData : TEXCOORD1;
            };

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 customData : TEXCOORD1;
                float3 viewDir : COLOR;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float PI = 3.1415926;
            UNITY_DECLARE_TEX2DARRAY(_RainTexArray);
	
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
                o.customData = v.customData;
                o.viewDir = float3(v.vertex.xyz - _WorldSpaceCameraPos);
	            return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
//				fixed4 col = tex2D(_MainTex, i.uv);
//                fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_RainTexArray, float3(i.uv, 0));
                fixed4 col = fixed4(i.customData.rgb, 1);
                    
                // apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
