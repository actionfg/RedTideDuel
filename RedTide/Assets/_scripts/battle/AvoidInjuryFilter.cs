using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AvoidInjuryFilter : UnitEffectFilter
{
    AvoidInjuryType OnAvoidInjury(GameUnit src, GameUnit victim, float damage, DamageType damageType, DamageRange damageRange,
        DamageAttribute damageAttribute, DamageSource damageSource);
}

public enum AvoidInjuryType
{
    None,
    Miss,
    Invincible
}