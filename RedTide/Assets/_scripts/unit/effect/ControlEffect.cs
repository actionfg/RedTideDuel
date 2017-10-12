using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ControlEffect
{
    ControlEffectType GetControlType();
}

public enum ControlEffectType
{
    None,                      // 非控制类
    Snare,                     // 定身
    Slow,                      // 减速
    Silence,                   // 沉默
    Uncontrollable             // 不可控制，包含stun,fear,knockback等
}
