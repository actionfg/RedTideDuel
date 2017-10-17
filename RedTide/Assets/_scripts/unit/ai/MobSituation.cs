using UnityEngine;
using System.Collections;
using _scripts;
using _scripts.unit.ai;

// 选择AI攻击目标
public class MobSituation : BaseAISituation {

    private static readonly float UPDATE_INTERVAL = 0.5f;
    public static readonly float RETREAT_THRESHOLD = 2.2f;

    protected GameUnit _target;
    private MobControl _mobControl;
    private NearbyEnemys _nearbyEnemys;

    public MobSituation(MobUnit owner) : base(owner)
    {
        _mobControl = owner.GetComponent<MobControl>();
        _nearbyEnemys = owner.GetComponentInChildren<NearbyEnemys>();
    }

    protected override bool shouldUpdate(float acc)
    {
        return acc >= UPDATE_INTERVAL;
    }

    protected override int updateSituation(float tpf)
    {
        if (GetOwner().CurrentHp <= 0 || GameContext.GameStage == GameStage.Prepare) 
            return (int) MobState.Stand;

        if (IsTargetValid(_target))
        {
            _target = SelectHigherPriorityTarget(_target);
        }
        else
        {
            _target = SelectTarget();
        }
        // 如果在空中, 则不再尝试靠近目标
        if (!IsOnGround())
        {
            return (int) MobState.Stand;
        }

        var checkTargetDistance = CheckTargetDistance();
        return checkTargetDistance;
    }

    private bool IsOnGround()
    {
        return GetOwner().CheckOnGround();
    }

    protected virtual GameUnit SelectTarget()
    {
        if (GetOwner() is MobUnit)
        {
            var mobOwner = (MobUnit)GetOwner();
            if (mobOwner.GetRouser() != null && mobOwner.GetRouser().PlayerId != mobOwner.PlayerId && mobOwner.GetRouser().CurrentHp > 0)
            {
                return mobOwner.GetRouser();
            }
        }

        if (_nearbyEnemys)
        {
            return _nearbyEnemys.GetNearestEnemy();    // 选择最近地方阵营单位 
        }

        return null;
    }

    private int CheckTargetDistance()
    {
        if (_target)
        {
            if (!_target.FilterOnActiveSkill())
            {
                return (int) MobState.ForceAttack;
            }

            // 如果被致盲, 无视攻击距离, 任意方向攻击
            if (GetOwner().GetEffect(EffectConfig.EffectContextID.BlindEffect) != null)
            {
                return (int) MobState.BlindAttack;
            }
            if (_target.GetEffect(EffectConfig.EffectContextID.StealthEffect) != null)
            {
                return (int) MobState.SearchAttack;
            }

            MobUnit owner = (MobUnit) GetOwner();
            var toTarget = _target.transform.position - owner.transform.position;
            toTarget.y = 0;
            var targetSqrMagnitude = toTarget.sqrMagnitude;
            if (targetSqrMagnitude <= owner.Config.ActiveRange * owner.Config.ActiveRange)
            {
                return (int) MobState.Attack;
            }
            else
            {
                return (int) MobState.Chasing;
            }
        }
        return (int) MobState.Stand;
    }

    protected virtual GameUnit SelectHigherPriorityTarget(GameUnit target)
    {
        // TODO 后期可根据相关AI策略修改, 如斩杀HP最少的敌人
        if (_nearbyEnemys)
        {
            return _nearbyEnemys.GetNearestEnemy();   
        }
        return target;
    }

    private bool IsTargetValid(GameUnit target)
    {
        if (target == null) return false;

        if (target.CurrentHp <= 0) return false;
        return true;
    }

    public GameUnit GetTarget()
    {
        return _target;
    }

    public enum MobState
    {
        Stand,            // 无目标时
        Chasing,          // 攻击距离不够
        Attack,
        ForceAttack,      // 强制攻击, 在目标受控时触发
        BlindAttack,      // 被致盲后的攻击行为
        SearchAttack,     // 目标隐形后的攻击行为
    }
}
