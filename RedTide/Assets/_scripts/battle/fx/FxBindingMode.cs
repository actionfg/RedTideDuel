using UnityEngine;
using System.Collections;

public enum FxBindingMode {
    None,                 // 不进行绑定, 默认出现在EffectObject位置
    BindPosition,         // 绑定位置
    BindRotation,         // 绑定旋转
    BindPosAndRot,
    InitUnitPosition,     // 不绑定, 但初始化在Unit的位置, 而不是EffectObject位置

}
