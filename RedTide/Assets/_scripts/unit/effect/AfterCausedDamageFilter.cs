using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AfterCausedDamageFilter : UnitEffectFilter
{
    void AfterCausedDamage(GameUnit src, GameUnit victim, float damage, DamageType damageType, DamageAttribute damageAttribute, DamageSource damageSource);
}
