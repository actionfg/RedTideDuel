using UnityEngine;
using System.Collections;

public class ChainedEffectObject : Effect
{
    public ChainedTarget nextTarget = ChainedTarget.Target;
    public EffectObject chainedEffect;

    public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, int skillEndureLevel, int comboIndex)
    {
        if (chainedEffect)
        {
            if (nextTarget == ChainedTarget.Caster)
            {
                EffectUtil.createEffect(chainedEffect.gameObject, effectObject.transform.position,
                    effectObject.transform.rotation, caster, caster.gameObject, caster, skillEndureLevel);
            }
            else
            {
                EffectUtil.createEffect(chainedEffect.gameObject, effectObject.transform.position,
                    effectObject.transform.rotation, caster, target, targetUnit, skillEndureLevel);
            }
        }
    }
}

public enum ChainedTarget
{
    Target,
    Caster
}
