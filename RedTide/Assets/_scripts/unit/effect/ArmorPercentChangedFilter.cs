using UnityEngine;
using System.Collections;

public interface ArmorPercentChangedFilter : UnitEffectFilter {

    // 返回值为正数, 则增加护甲，反之减少
    float OnArmorPercentChanged(float armor, GameUnit src, GameUnit victim);
}
