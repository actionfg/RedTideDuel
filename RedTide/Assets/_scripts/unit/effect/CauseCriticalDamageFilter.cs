using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CauseCriticalDamageFilter : UnitEffectFilter
{
    float OnCauseCriticalDamage(GameUnit src, GameUnit victim, float baseCriticalDamage, DamageType damageType, DamageAttribute damageAttribute, DamageSource damageSource);
}
