using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 检测AI的移动是否被其他怪物堵住 ( 静态障碍物由AstarPath自己管理 )
public class MoveBlockedSituation : BaseAISituation
{
    private static readonly float UPDATE_INTERVAL = 1f;
    private MobSituation _targetSituation;
    private MobControl _mobControl;

    public MoveBlockedSituation(GameUnit owner, MobSituation mobSituation) : base(owner)
    {
        _targetSituation = mobSituation;
        if (GetOwner() is MobUnit)
        {
            var mobUnit = (MobUnit) GetOwner();
            _mobControl = mobUnit.GetComponent<MobControl>();
        }
    }

    protected override bool shouldUpdate(float acc)
    {
        return acc >= UPDATE_INTERVAL;
    }

    protected override int updateSituation(float tpf)
    {
        if (_mobControl)
        {
            var mobUnit = (MobUnit) GetOwner();
            var availableSkills = mobUnit.GetAvailableSkills();
            var target = _targetSituation.GetTarget();
            if (target != null)
            {
                float maxAttackRange = GetMaxAttackRange(mobUnit, availableSkills);
                float toTargetSqr = Vector3.SqrMagnitude(target.transform.position - _mobControl.transform.position);
                if (toTargetSqr > maxAttackRange * maxAttackRange)
                {// 在攻击距离之外
                    return CheckColliderUnitCount();
                }
            }
            else
            {// 无目标

                return CheckColliderUnitCount();
            }
        }
        return (int) BlockState.None;
    }

    private float GetMaxAttackRange(MobUnit mobUnit, Dictionary<SkillConfig, int> availableSkills)
    {
        float maxRange = 0;
        if (availableSkills.Count > 0)
        {
            var skills = mobUnit.Config.attackSkills;
            foreach (var skillPair in availableSkills)
            {
                var mobSkillConfig = (MobSkillConfig) skillPair.Key;
                if (mobSkillConfig.AttackRange > maxRange)
                {
                    maxRange = mobSkillConfig.AttackRange;
                }
            }
        }
        return maxRange;
    }

    private int CheckColliderUnitCount()
    {
        if (_mobControl.ColliderUnits >= 3)
        {
            return (int) BlockState.TotalBlock;
        }
        else if (_mobControl.ColliderUnits > 1)
        {
            return (int) BlockState.PartialBlock;
        }
        return (int) BlockState.None;
    }

    public enum BlockState
    {
        None,
        PartialBlock,
        TotalBlock,
    }
}