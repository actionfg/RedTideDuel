// 绘制雨粒子: 参考Nvidia SDK10.5中Rain例子
Shader "Custom/RainParticle" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RainTexArray("Tex", 2DArray) = "" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Alpha("Alpha",Range(0,1)) = 1
		
	}
	SubShader {
		Tags {"Queue" ="Transparent" "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

        struct appdata_particles {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            fixed4 color : COLOR;
            float4 texcoords : TEXCOORD0;
            float4 customData : TEXCOORD1;
        };
            
		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			fixed4 color;
            float4 customData : TEXCOORD1;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Alpha;

        float PI = 3.1415926;
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

        UNITY_DECLARE_TEX2DARRAY(_RainTexArray);

        void vert(inout appdata_particles v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.uv_MainTex = v.texcoords.xy;
            o.color = v.color;
            o.customData = v.customData;
          }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
//			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_RainTexArray, float3(IN.uv_MainTex, 0));          
            
//            o.Alpha = opacity;
//            o.Albedo = float3(1.0, 1.0, 1.0);
            
			o.Albedo = float3( IN.customData.r / 8.0, 0.0, 0.0);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

//            if (c.r > 0.5f) {
//			    o.Alpha = 1.0; 
//            }
//            else {
//                discard;
//            }
		}
		
        float GetOpacity(float2 uv, float3 lightVector, float3 eyeVector) 
        {
            float opacity = 0.0;
            float fallOff = 1;
            
            float3 dropDir = float3(0.0, -1.0, 0.0);

            #define MAX_VIDX 4
            #define MAX_HIDX 8
            // Inputs: lightVector, eyeVector, dropDir
            float3 L = normalize(lightVector);
            float3 E = normalize(eyeVector);
            float3 N = normalize(dropDir);
            
            bool is_EpLp_angle_ccw = true;
            float hangle = 0;
            float vangle = abs( (acos(dot(L,N)) * 180/PI) - 90 ); // 0 to 90
            
            {
                float3 Lp = normalize( L - dot(L,N)*N );
                float3 Ep = normalize( E - dot(E,N)*N );
                hangle = acos( dot(Ep,Lp) ) * 180/PI;  // 0 to 180
                hangle = (hangle-10)/20.0;           // -0.5 to 8.5
                is_EpLp_angle_ccw = dot( N, cross(Ep,Lp)) > 0;
            }
            
            if(vangle>=88.0)
            {
                hangle = 0;
                is_EpLp_angle_ccw = true;
            }
                    
            vangle = (vangle-10.0)/20.0; // -0.5 to 4.5
            
            // Outputs:
            // verticalLightIndex[1|2] - two indices in the vertical direction
            // t - fraction at which the vangle is between these two indices (for lerp)
            int verticalLightIndex1 = floor(vangle); // 0 to 5
            int verticalLightIndex2 = min(MAX_VIDX, (verticalLightIndex1 + 1) );
            verticalLightIndex1 = max(0, verticalLightIndex1);
            float t = frac(vangle);
    
            // textureCoordsH[1|2] used in case we need to flip the texture horizontally
            float textureCoordsH1 = uv.x;
            float textureCoordsH2 = uv.x;
            
            // horizontalLightIndex[1|2] - two indices in the horizontal direction
            // s - fraction at which the hangle is between these two indices (for lerp)
            int horizontalLightIndex1 = 0;
            int horizontalLightIndex2 = 0;
            float s = 0;
            
            s = frac(hangle);
            horizontalLightIndex1 = floor(hangle); // 0 to 8
            horizontalLightIndex2 = horizontalLightIndex1+1;
            if( horizontalLightIndex1 < 0 )
            {
                horizontalLightIndex1 = 0;
                horizontalLightIndex2 = 0;
            }
                       
            if( is_EpLp_angle_ccw )
            {
                if( horizontalLightIndex2 > MAX_HIDX ) 
                {
                    horizontalLightIndex2 = MAX_HIDX;
                    textureCoordsH2 = 1.0 - textureCoordsH2;
                }
            }
            else
            {
                textureCoordsH1 = 1.0 - textureCoordsH1;
                if( horizontalLightIndex2 > MAX_HIDX ) 
                {
                    horizontalLightIndex2 = MAX_HIDX;
                } else 
                {
                    textureCoordsH2 = 1.0 - textureCoordsH2;
                }
            }
                    
            if( verticalLightIndex1 >= MAX_VIDX )
            {
                textureCoordsH2 = uv.x;
                horizontalLightIndex1 = 0;
                horizontalLightIndex2 = 0;
                s = 0;
            }
            
            // Generate the final texture coordinates for each sample
            // TODO 暂定为1, 应该是[1, 8]之间的随机数
            uint type = 1;
            uint2 texIndicesV1 = uint2(verticalLightIndex1*90 + horizontalLightIndex1*10 + type,
                                         verticalLightIndex1*90 + horizontalLightIndex2*10 + type);
            float3 tex1 = float3(textureCoordsH1, uv.y, texIndicesV1.x);
            float3 tex2 = float3(textureCoordsH2, uv.y, texIndicesV1.y);
            if( (verticalLightIndex1<4) && (verticalLightIndex2>=4) ) 
            {
                s = 0;
                horizontalLightIndex1 = 0;
                horizontalLightIndex2 = 0;
                textureCoordsH1 = uv.x;
                textureCoordsH2 = uv.x;
            }
            
            uint2 texIndicesV2 = uint2(verticalLightIndex2*90 + horizontalLightIndex1*10 + type,
                                         verticalLightIndex2*90 + horizontalLightIndex2*10 + type);
            float3 tex3 = float3(textureCoordsH1, uv.y, texIndicesV2.x);        
            float3 tex4 = float3(textureCoordsH2, uv.y, texIndicesV2.y);
    
            // Sample opacity from the textures
            // TODO samAniso为Repeat uv 
            float col1 = UNITY_SAMPLE_TEX2DARRAY(_RainTexArray, tex1).r;        
//            float col1 = _RainTexArray.Sample( samAniso, tex1) * g_rainfactors[texIndicesV1.x];
//            float col2 = _RainTexArray.Sample( samAniso, tex2) * g_rainfactors[texIndicesV1.y];
//            float col3 = _RainTexArray.Sample( samAniso, tex3) * g_rainfactors[texIndicesV2.x];
//            float col4 = _RainTexArray.Sample( samAniso, tex4) * g_rainfactors[texIndicesV2.y];
//    
//            // Compute interpolated opacity using the s and t factors
//            float hOpacity1 = lerp(col1,col2,s);
//            float hOpacity2 = lerp(col3,col4,s);
//            opacity = lerp(hOpacity1,hOpacity2,t);
//            opacity = pow(opacity,0.7); // inverse gamma correction (expand dynamic range)
//            opacity = 4*lightIntensity * opacity * fallOff;
            
            return opacity;
        }
		ENDCG
	}
	FallBack "Diffuse"
}
