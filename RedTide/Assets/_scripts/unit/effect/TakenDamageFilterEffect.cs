using UnityEngine;
using System.Collections;

public class TakenDamageFilterEffect : Effect {
    public float changePercent;
    public float duration;

    public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, int skillEndureLevel, int comboIndex)
    {
        if (targetUnit)
        {
            targetUnit.AddEffect(new TakenDamageFilterEffectConfig(caster, changePercent, duration));
        }
    }
}

class TakenDamageFilterEffectConfig : EffectConfig, TakingDamageFilter
{
    private float changePercent;

    public TakenDamageFilterEffectConfig(GameUnit caster, float changePercent, float duration) : base(caster)
    {
        this.changePercent = changePercent;
        SetDuration(duration);
    }

    public override void OnStart(GameUnit target)
    {
        // Do nothing
    }

    public override bool OnUpdate(GameUnit target)
    {
        return true;
    }

    public override void OnEnd(GameUnit target)
    {
        // Do nothing
    }

    public EffectFilterType[] getFilterTypes()
    {
        return new EffectFilterType[]{EffectFilterType.TakingDamageFilter};
    }

    public float OnTakingDamage(float damage, GameUnit src, GameUnit victim, DamageType damageType, DamageRange damageRange,
        DamageAttribute damageAttribute, DamageSource damageSource)
    {
        return damage * changePercent;
    }
}

