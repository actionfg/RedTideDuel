using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AttackFilter : UnitEffectFilter {
    // no matter hit or not
    void OnAttack(GameUnit src);
}
