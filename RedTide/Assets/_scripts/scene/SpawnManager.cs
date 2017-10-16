using UnityEngine;
using _scripts.unit;

namespace _scripts.scene
{
    public class SpawnManager : MonoBehaviour
    {
        public float Interval = 0f;
        public int PlayerId = -1;        // 对战场景, 阵营方可能会变; 为-1则中立

        public DestroyableUnitConfig BaseConfig;
        
        public MobConfig mob1;
        public int Count = 1;

        private float _acc = 0;
        
        private void Start()
        {
            _acc = Interval;

            if (BaseConfig)
            {
                SpawnList.DoSpawn(BaseConfig, transform.position, PlayerId);
            }
        }
        
        private void Update()
        {
            _acc += Time.deltaTime;
            if (_acc >= Interval)
            {
                _acc -= Interval;

                DoSpawn();
            }
        }

        private void DoSpawn()
        {
            for (int i = 0; i < Count; i++)
            {
                SpawnList.DoSpawn(mob1, transform.position, PlayerId);
            }
        }
    }
}