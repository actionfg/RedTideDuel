using UnityEngine;
using System.Collections;

public interface TakingDamageFilter : UnitEffectFilter {

    // 返回值为正数, 则自己收到的伤害增加, 反之减少
    float OnTakingDamage(float damage, GameUnit src, GameUnit victim, DamageType damageType, DamageRange damageRange, DamageAttribute damageAttribute, DamageSource damageSource);

}
