using UnityEngine;

namespace Game04.Util
{
    // 用于碎片爆炸 
    public class FractureExplode : MonoBehaviour
    {
        private void Start()
        {
//            var fracturedObject = GetComponent<FracturedObject>();
//            fracturedObject.Explode(transform.position, Force, Radius, true, false, false, true);
        }

        public float Force = 20;

        public float Radius = 2;
    }
}