// 场景单位, 用于释放场景特性, 无法摧毁

using System;
using UnityEngine;

public class SceneUnit : DestroyableUnit
{
    public float AttackPower { set; get; }
    
    protected override bool OnDeath()
    {
        return false;
    }

    protected override void OnFalling()
    {
    }

    protected override void Start()
    {
        base.Start();
        var basicAttributeConfig = ScriptableObject.CreateInstance<BasicAttributeConfig>();
        basicAttributeConfig.hp = 1;
        if (AttackPower < 0.001f)
        {
            AttackPower = 1f;
        }

        Init(basicAttributeConfig);
    }

    public override float GetAttackPower()
    {
        return AttackPower;
    }

    public override float GetSpellPower()
    {
        return AttackPower;
    }
}