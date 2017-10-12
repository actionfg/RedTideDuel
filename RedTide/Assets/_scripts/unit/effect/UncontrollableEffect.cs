using System;
using UnityEngine;
using System.Collections;

public class UncontrollableEffect : Effect
{

    public UncontrollableType uncontrollableType;
    public float duration;

    public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, int skillEndureLevel, int comboIndex)
    {
        if (targetUnit)
        {
            targetUnit.AddEffect(new UncontrollableEffectConfig(caster, targetUnit, uncontrollableType, duration));
        }
    }
}

class UncontrollableEffectConfig : EffectConfig, InputEnableFilter, ControlEffect, FractureAfterDeathFilter
{
    public UncontrollableType uncontrollableType { get; private set; }
    private float duration;
    private Animator anim;
    private FxPlayPosition fxPlayPosition;
    private String animName;
    private float acc;

    public UncontrollableEffectConfig(GameUnit caster, GameUnit target, UncontrollableType uncontrollableType, float duration) : base(caster, EffectContextID.UncontrollableEffect)
    {
        this.uncontrollableType = uncontrollableType;
        this.duration = duration;
    }

    public override void OnStart(GameUnit target)
    {
        acc = 0;
        anim = target.GetComponent<Animator>();
        animName = GetAnimName();
        if (!String.IsNullOrEmpty(animName))
        {
            anim.SetBool(animName, true);
        }
        var mobAiPath = target.GetComponent<MobAIPath>();
        if (mobAiPath)
        {
            mobAiPath.EnableTrace(false);
        }
        // TODO 打断target的当前技能行为, 重置状态
    }

    public override bool OnUpdate(GameUnit target)
    {
        acc += Time.deltaTime;
        return acc < duration;
    }

    public override void OnEnd(GameUnit target)
    {
        if (!String.IsNullOrEmpty(animName))
        {
            anim.SetBool(animName, false);
        }
    }

    public EffectFilterType[] getFilterTypes()
    {
        return new[]{EffectFilterType.InputEnableFilter, EffectFilterType.FratureFilter};
    }

    public bool OnMoveEnabled()
    {
        return false;
    }

    public bool OnActiveSkill()
    {
        return false;
    }

    private String GetAnimName()
    {
        switch (uncontrollableType)
        {// TODO 实际被控动画
            case UncontrollableType.Stun:
                return "isStun";
            case UncontrollableType.Numb:
                return "isWalking";
            case UncontrollableType.Fear:
                return "isWalking";
            case UncontrollableType.KnockBack:
                return "isWalking";
            case UncontrollableType.OnHit:
                return "OnHit";
        }
        return null;
    }

    public ControlEffectType GetControlType()
    {
        return ControlEffectType.Uncontrollable;
    }

    public bool EnableBroken(GameUnit victim)
    {
        return uncontrollableType == UncontrollableType.Frozen;
    }
}
