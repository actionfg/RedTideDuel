using System.IO;
using Imaging.DDSReader;
using UnityEditor;
using UnityEngine;

namespace RedTide
{
    public class ShaderVariableUpdater : MonoBehaviour
    {
        public float Step = 0.085f;
        
        private Material _material;
        private float _timeCycle;
        
        private void Start()
        {
            // 注意, 会永久(停止运行后)修改Material的属性
            _material = GetComponent<Renderer>().material;
            _timeCycle = 0f;
        }

        private void Update()
        {
            _timeCycle += Time.deltaTime;
            var hasProperty = _material.HasProperty("TimeCycle");
            _material.SetFloat("g_timeCycle", _timeCycle);
            if (_timeCycle >= 1f)
            {
                _timeCycle = 0;

                _material.SetFloat("XDisplace", Random.value * 2f);
                _material.SetFloat("YDisplace", Random.value * 2f);
                
            }
        }
    }
}