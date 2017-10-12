using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpSkillSituation : MobSkillSituation
{
    private static readonly float UPDATE_INTERVAL = 0.5f;

    public ImpSkillSituation(GameUnit owner, MobSituation mobSituation) : base(owner, mobSituation)
    {
    }

    protected override bool shouldUpdate(float acc)
    {
        return acc >= UPDATE_INTERVAL;
    }

    protected override int updateSituation(float tpf)
    {
        var owner = GetOwner() as MobUnit;
        if (owner == null) return 0;

        var target = MobSituation.GetTarget();

        if (target)
        {
            if (owner.CurrentHp < owner.MaxHp * 0.6f)
            {//血量低于60%, 则远程扔火球
                return 1;
            }
            else
            {// 不尝试走进攻击
                return 0;
            }
        }
        return -1;
    }

}