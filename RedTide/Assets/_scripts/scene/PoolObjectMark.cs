// 用于标记GameObjectPool生成的clone体

using UnityEngine;

public class PoolObjectMark : MonoBehaviour
{
    public GameObjectPool Pool;

    public bool Unspawn()
    {
        if (Pool != null)
        {
            return Pool.Unspawn(gameObject);
        }
        return false;
    }
}