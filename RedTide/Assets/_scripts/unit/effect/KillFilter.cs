using UnityEngine;

public interface KillFilter : UnitEffectFilter
{
    void OnKill(GameUnit target, DamageSource damageSource);
}
