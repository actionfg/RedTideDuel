// 用于stage管理各个chapter的ObjectPool   
// 切换chapter时清空

using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    private Dictionary<String, GameObjectPool> _poolMap;
    
    private void Awake()
    {
        DoSingleton();
        
        _poolMap = new Dictionary<String, GameObjectPool>();
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

    // 注册即刻生成
    public void RegisterObject(GameObject prefab)
    {
        RegisterObject(prefab, 30);
    }

    public void RegisterObject(GameObject prefab, int count)
    {
        var objectPool = new GameObjectPool(prefab, count);
        _poolMap.Add(prefab.name, objectPool);
    }

    public bool HasPooled(GameObject obj)
    {
        var poolName = GetPoolName(obj);
        return _poolMap.ContainsKey(poolName);
    }
    
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rot)
    {
        if (_poolMap.ContainsKey(prefab.name))
        {
            return _poolMap[prefab.name].Spawn(position, rot);
        }
        Debug.LogWarning(prefab.name + " has not registered in PoolManager");
        return null;
    }

    public bool Unspawn(GameObject obj)
    {
        var substring = GetPoolName(obj);
        if (_poolMap.ContainsKey(substring))
        {
            _poolMap[substring].Unspawn(obj);
        }
        return true;
    }

    private static string GetPoolName(GameObject obj)
    {
        var substring = obj.name;
        var indexOf = obj.name.IndexOf("(");
        if (indexOf > 0)
        {
           substring = obj.name.Substring(0, indexOf);
        }
        return substring;
    }

    public void Clear()
    {
        foreach (var pool in _poolMap.Values)
        {
            pool.Clear();
        }
    }
}