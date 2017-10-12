using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectStackType {

    None,               // 不可叠加
    StackOnly,          // 增加层数
    RefreshTime,        // 刷新时间
    StackRefresh,       // 增加层数,刷新时间
    Replace             // 替换原有Buff

}
