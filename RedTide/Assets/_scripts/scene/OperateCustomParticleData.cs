using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OperateCustomParticleData : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystemRenderer psr;
    private List<Vector4> customData = new List<Vector4>();
    public float minDist = 30.0f;

    public bool CheckIndex = false;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        psr = GetComponent<ParticleSystemRenderer>();

        // emit in a sphere with no speed
        var main = ps.main;
        main.startSpeedMultiplier = 0.0f;
        main.simulationSpace =
            ParticleSystemSimulationSpace
                .World; // so our particle positions don't require any extra transformation, to compare with the mouse position
//        var emission = ps.emission;
//        emission.rateOverTimeMultiplier = 200.0f;
//        var shape = ps.shape;
//        shape.shapeType = ParticleSystemShapeType.Sphere;
//        shape.radius = 4.0f;
//        psr.sortMode = ParticleSystemSortMode.YoungestInFront;

        // send custom data to the shader
//        psr.EnableVertexStreams(ParticleSystemVertexStreams.Custom1);
    }

    void Update()
    {
        // 需要不断获取并设置, 不然新粒子无法设值
//        ps.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

        int particleCount = ps.particleCount;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        ps.GetParticles(particles);
//        Camera mainCam = Camera.main;
        if (CheckIndex)
        {
            CheckIndex = false;
            Vector3 lightVec = new Vector3(1, 1, 1);
            for (int i = 0; i < particleCount; i++)
            {
//                if (customData[i].x <= 0.001f)
//                {
//                    customData[i] = new Vector4(Random.value, 1, 0, 0);
//                }
                float opacity = GetOpacity(new Vector2(0.5f, 0.5f), new Vector3(1, 1, 1),
                    particles[i].position - Camera.main.transform.position, Random.Range(1, 8));
            }
        }

//        ps.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
    }

    private int MAX_VIDX = 4;
    private int MAX_HIDX = 8;
    private float PI = 3.14159265f;

    float GetOpacity(Vector2 uv, Vector3 lightVector, Vector3 eyeVector, int type)
    {
        float opacity = 0.0f;
        float fallOff = 1;

        Vector3 dropDir = new Vector3(0.0f, -1.0f, 0.0f);

        // Inputs: lightVector, eyeVector, dropDir
        Vector3 L = lightVector.normalized;
        Vector3 E = eyeVector.normalized;
        Vector3 N = dropDir.normalized;

        bool is_EpLp_angle_ccw = true;
        float hangle = 0;
        float vangle = Mathf.Abs((Mathf.Acos(Vector3.Dot(L, N))) * 180 / PI) - 90f; // 0 to 90

        {
            Vector3 Lp = L - Vector3.Dot(L, N) * N;
            Vector3 Ep = E - Vector3.Dot(E, N) * N;
            hangle = Mathf.Acos(Vector3.Dot(Ep, Lp)) * 180 / PI; // 0 to 180
            hangle = (hangle - 10) / 20.0f; // -0.5 to 8.5
            is_EpLp_angle_ccw = Vector3.Dot(N, Vector3.Cross(Ep, Lp)) > 0;
        }

        if (vangle >= 88.0)
        {
            hangle = 0;
            is_EpLp_angle_ccw = true;
        }

        vangle = (vangle - 10.0f) / 20.0f; // -0.5 to 4.5

        // Outputs:
        // verticalLightIndex[1|2] - two indices in the vertical direction
        // t - fraction at which the vangle is between these two indices (for lerp)
        int verticalLightIndex1 = (int) Math.Floor(vangle); // 0 to 5
        int verticalLightIndex2 = Math.Min(MAX_VIDX, (verticalLightIndex1 + 1));
        verticalLightIndex1 = Math.Max(0, verticalLightIndex1);
        float t = vangle - verticalLightIndex1;

        // textureCoordsH[1|2] used in case we need to flip the texture horizontally
        float textureCoordsH1 = uv.x;
        float textureCoordsH2 = uv.x;

        // horizontalLightIndex[1|2] - two indices in the horizontal direction
        // s - fraction at which the hangle is between these two indices (for lerp)
        int horizontalLightIndex1 = 0;
        int horizontalLightIndex2 = 0;
        float s = 0;

        horizontalLightIndex1 = (int) Math.Floor(hangle); // 0 to 8
        s = hangle - horizontalLightIndex1;
        horizontalLightIndex2 = horizontalLightIndex1 + 1;
        if (horizontalLightIndex1 < 0)
        {
            horizontalLightIndex1 = 0;
            horizontalLightIndex2 = 0;
        }

        if (is_EpLp_angle_ccw)
        {
            if (horizontalLightIndex2 > MAX_HIDX)
            {
                horizontalLightIndex2 = MAX_HIDX;
                textureCoordsH2 = 1.0f - textureCoordsH2;
            }
        }
        else
        {
            textureCoordsH1 = 1.0f - textureCoordsH1;
            if (horizontalLightIndex2 > MAX_HIDX)
            {
                horizontalLightIndex2 = MAX_HIDX;
            }
            else
            {
                textureCoordsH2 = 1.0f - textureCoordsH2;
            }
        }

        if (verticalLightIndex1 >= MAX_VIDX)
        {
            textureCoordsH2 = uv.x;
            horizontalLightIndex1 = 0;
            horizontalLightIndex2 = 0;
            s = 0;
        }

        // Generate the final texture coordinates for each sample
        Vector2 texIndicesV1 = new Vector2(verticalLightIndex1 * 90 + horizontalLightIndex1 * 10 + type,
            verticalLightIndex1 * 90 + horizontalLightIndex2 * 10 + type);
        Debug.Log("texIndexV1: " + texIndicesV1);
        Vector3 tex1 = new Vector3(textureCoordsH1, uv.y, texIndicesV1.x);
        Vector3 tex2 = new Vector3(textureCoordsH2, uv.y, texIndicesV1.y);
        if ((verticalLightIndex1 < 4) && (verticalLightIndex2 >= 4))
        {
            s = 0;
            horizontalLightIndex1 = 0;
            horizontalLightIndex2 = 0;
            textureCoordsH1 = uv.x;
            textureCoordsH2 = uv.x;
        }

        Vector2 texIndicesV2 = new Vector2(verticalLightIndex2 * 90 + horizontalLightIndex1 * 10 + type,
            verticalLightIndex2 * 90 + horizontalLightIndex2 * 10 + type);
        Debug.Log("texIndexV2: " + texIndicesV2);
        Vector3 tex3 = new Vector3(textureCoordsH1, uv.y, texIndicesV2.x);
        Vector3 tex4 = new Vector3(textureCoordsH2, uv.y, texIndicesV2.y);

        // Sample opacity from the textures
        // TODO samAniso为Repeat uv 
//                float col1 = UNITY_SAMPLE_TEX2DARRAY(_RainTexArray, tex1).r;        
//                float col1 = UNITY_SAMPLE_TEX2DARRAY( _RainTexArray, tex1).r * g_rainfactors[texIndicesV1.x];
//                float col2 = UNITY_SAMPLE_TEX2DARRAY( _RainTexArray, tex2).r * g_rainfactors[texIndicesV1.y];
//                float col3 = UNITY_SAMPLE_TEX2DARRAY( _RainTexArray, tex3).r * g_rainfactors[texIndicesV2.x];
//                float col4 = UNITY_SAMPLE_TEX2DARRAY( _RainTexArray, tex4).r * g_rainfactors[texIndicesV2.y];
//        
//                // Compute interpolated opacity using the s and t factors
//                float hOpacity1 = lerp(col1,col2,s);
//                float hOpacity2 = lerp(col3,col4,s);
//                opacity = lerp(hOpacity1,hOpacity2,t);
//                opacity = pow(opacity,0.7); // inverse gamma correction (expand dynamic range)
//                opacity = 4 * opacity * fallOff;

        return opacity;
    }

//    void OnGUI() {
//
//        minDist = GUI.HorizontalSlider(new Rect(25, 40, 100, 30), minDist, 0.0f, 100.0f);
//    }
}