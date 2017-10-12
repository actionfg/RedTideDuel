using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Effect : MonoBehaviour
{
    public bool includeEnemy = true;
    public bool includeSelf;
    public bool includeFriendly;
    public bool includeScene;

    public bool OnTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, 
        int hitEndureLevel, int comboIndex = 0)
    {
        if (!includeScene && targetUnit == null)
        {
            return false;
        }

        if (targetUnit)
        {
            if (!includeEnemy && (caster.isEnemy(targetUnit)))
            {
                return false;
            }


            if (!includeFriendly && (!caster.isEnemy(targetUnit)) && caster != targetUnit)
            {
                return false;
            }

            if (!includeSelf && (caster == targetUnit))
            {
                return false;
            }
        }
        DoTrigger(effectObject, caster, target, targetUnit, hitEndureLevel, comboIndex);
        return true;
    }

    public abstract void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, int skillEndureLevel, int comboIndex);

    public virtual void DoTriggerOnCollideCompleted(EffectObject effectObject, GameUnit caster, HashSet<GameObject> hitSet)
    {

    }
}