using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperateCustomParticleData : MonoBehaviour {

    private ParticleSystem ps;
    private ParticleSystemRenderer psr;
    private List<Vector4> customData = new List<Vector4>();
    public float minDist = 30.0f;

    void Start() {

        ps = GetComponent<ParticleSystem>();
        psr = GetComponent<ParticleSystemRenderer>();

        // emit in a sphere with no speed
        var main = ps.main;
        main.startSpeedMultiplier = 0.0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World; // so our particle positions don't require any extra transformation, to compare with the mouse position
//        var emission = ps.emission;
//        emission.rateOverTimeMultiplier = 200.0f;
//        var shape = ps.shape;
//        shape.shapeType = ParticleSystemShapeType.Sphere;
//        shape.radius = 4.0f;
//        psr.sortMode = ParticleSystemSortMode.YoungestInFront;

        // send custom data to the shader
        psr.EnableVertexStreams(ParticleSystemVertexStreams.Custom1);
    }

    void Update() {
        // 需要不断获取并设置, 不然新粒子无法设值
        ps.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

        int particleCount = ps.particleCount;
//        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
//        ps.GetParticles(particles);
//        Camera mainCam = Camera.main;

        for (int i = 0; i < particleCount; i++)
        {
            // 可获取particle世界位置
//            Vector3 sPos = mainCam.WorldToScreenPoint(particles[i].position);
//
//            // set custom data to 1, if close enough to the mouse
//            if (Vector2.Distance(sPos, Input.mousePosition) < minDist)
//            {
//                customData[i] = new Vector4(1, 0, 0, 0);
//            }
//            // otherwise, fade the custom data back to 0
//            else
//            {
//                float particleLife = particles[i].remainingLifetime / ps.main.startLifetimeMultiplier;
//
//                if (customData[i].x > 0)
//                {
//                    float x = customData[i].x;
//                    x = Mathf.Max(x - Time.deltaTime, 0.0f);
//                    customData[i] = new Vector4(x, 0, 0, 0);
//                }
//            }
            if (customData[i].x <= 0.001f)
            {
                customData[i] = new Vector4(Random.value, 1, 0, 0);
            }
        }

        ps.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
    }

    void OnGUI() {

        minDist = GUI.HorizontalSlider(new Rect(25, 40, 100, 30), minDist, 0.0f, 100.0f);
    }
}
