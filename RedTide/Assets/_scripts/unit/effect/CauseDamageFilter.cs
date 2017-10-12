using UnityEngine;
using System.Collections;

public interface CauseDamageFilter : UnitEffectFilter {

    /**
     * @return Damage delta, positive to increase damage, negative to reduce
     */
    float OnCauseDamage(float damage, GameUnit src, GameUnit victim, DamageType damageType, DamageRange damageRange, DamageAttribute damageAttribute, DamageSource damageSource);
}
