using UnityEngine;
using System.Collections;

public abstract class CharacterConfig : ScriptableObject
{
    public string aName = "New Character";
    public float Mass = 1f;
    public GameObject aModel;
    [Header("需要加入对象池的物体, 主要用于一些经常生成的弹道")]
    public GameObject ObjectNeedPool;
    [Tooltip("被击声音")]
    public AudioClip[] BeHitSounds;

    public virtual GameObject Create()
    {
        if (ObjectNeedPool)
        {
            ObjectPoolManager.Instance.RegisterObject(ObjectNeedPool);        
        }
        
        return Instantiate(aModel);
    }
}
