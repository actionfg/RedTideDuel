using System;
using UnityEngine;
using System.Collections;

public class MobTrigger : MonoBehaviour
{

    protected GameUnit SelfUnit;

    protected virtual void Start()
    {
        var transformParent = transform.parent;
        if (transformParent)
        {
            SelfUnit = transformParent.GetComponent<GameUnit>();
        }
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (SelfUnit == null) return;
        var gameUnit = other.GetComponent<GameUnit>();
        if (gameUnit==null) return;
        if (!gameUnit.isEnemy(SelfUnit)) return;

        // 激活AI
        SelfUnit.ActiveAI(gameUnit);
    }
}
