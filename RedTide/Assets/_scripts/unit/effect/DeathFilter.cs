using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DeathFilter : UnitEffectFilter {
    // True, 则死亡; 反之, 继续存活
    bool OnDeath(GameUnit target);
}
