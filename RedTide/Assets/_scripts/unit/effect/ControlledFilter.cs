using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ControlledFilter : UnitEffectFilter {
    // 返回值为true，则忽略此控制效果
    bool OnControlled(ControlEffect controlEffect);
}
