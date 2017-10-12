using UnityEngine;
using System.Collections;

public interface InputEnableFilter : UnitEffectFilter
{
    // true为可接受移动，反之不能
    bool OnMoveEnabled();

    // true为可启动技能, 反之不能
    bool OnActiveSkill();
}
