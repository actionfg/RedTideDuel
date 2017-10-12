using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于巡逻, 后退, 走位等非正常移动, 移动速度减半
public class UnRegularMoveEffect : EffectConfig
{
    private readonly float Slow_Percent = -0.5f;
    private int _slowHandle = -1;
    public bool Activated { get; set; }

    public UnRegularMoveEffect(GameUnit caster) : base(caster, EffectContextID.UnRegularMoveEffect)
    {
        SetStackType(EffectStackType.None);

    }

    public override void OnStart(GameUnit target)
    {
        _slowHandle = target.AddAttribute(AttributeType.MoveSpeedFactor, Slow_Percent);
        Activated = true;
    }

    public override bool OnUpdate(GameUnit target)
    {
        if (Activated)
        {
            if (_slowHandle == -1)
            {
                OnStart(target);
            }
        }
        else
        {
            if (_slowHandle != -1)
            {
                OnEnd(target);
            }
        }
        return true;
    }

    public override void OnEnd(GameUnit target)
    {
        target.RemoveAttributeFactor(AttributeType.MoveSpeedFactor, _slowHandle);
        _slowHandle = -1;
    }
}
