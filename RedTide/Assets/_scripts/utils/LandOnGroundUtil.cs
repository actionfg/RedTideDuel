using UnityEngine;

namespace Game04.Util
{
    public class LandOnGroundUtil : MonoBehaviour
    {
        private void OnEnable()
        {
            // rayTest到地面, 注意该GameObject需要模型原点在最底下位置
            var ray = new Ray(transform.position + Vector3.up * 10f, Vector3.down);
            RaycastHit rayHit;
            int mask = LayerMask.GetMask("Floor");

            if (Physics.Raycast(ray, out rayHit, 20, mask))
            {
                transform.position = rayHit.point;
            }
        }
    }
}