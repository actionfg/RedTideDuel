using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 所有ragdoll都挂于此gameObject下
public class RagdollRoot : MonoBehaviour {
    public static RagdollRoot Root;

    void Awake()
    {
        if (Root == null)
        {
            DontDestroyOnLoad(gameObject);
            Root = this;
        }
        else if (Root != this)
        {
            Destroy(gameObject);
        }
    }

}
