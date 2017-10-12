using UnityEngine;

namespace _scripts.scene
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;

        public float Interval = 0f;
        public MobConfig mob1;
        public int Count = 1;

        private float _acc = 0;
        
        private void Start()
        {
            DoSingleton();

            _acc = Interval;
        }
        
        private void DoSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this.gameObject)
            {
                Destroy(this.gameObject);
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
                SpawnList.DoSpawn(mob1, transform.position);
            }
        }
    }
}