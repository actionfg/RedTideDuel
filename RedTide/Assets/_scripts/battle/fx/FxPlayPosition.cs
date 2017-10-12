using UnityEngine;
using System.Collections;

public enum FxPlayPosition {
    LogicalPosition,
    Head,
    Center,
    LeftHand,
    RightHand,
    Other,            // 注意同类型只有第一个Other的FxBinder有效
}
