using UnityEngine;

public interface OnHitFilter : UnitEffectFilter
{
    void OnHit(GameUnit caster);
}
