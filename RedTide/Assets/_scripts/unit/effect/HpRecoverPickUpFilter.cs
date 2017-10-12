using UnityEngine;

public interface HpRecoverPickUpFilter : UnitEffectFilter {
    float OnPickUpHpRecover(GameUnit unit, float healing);
}
