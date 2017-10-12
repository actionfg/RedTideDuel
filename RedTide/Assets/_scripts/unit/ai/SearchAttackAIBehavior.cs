using UnityEngine;
using System.Collections;

// 目标隐形后的攻击行为: 选择随机地点作为目标点展开攻击, 如果打中, 则维持方向; 否则继续选择随机地点
public class SearchAttackAIBehavior : AIBehavior
{

    public float MaxSearchDistance = 10f;
    public float AntiStealthDuration = 60f;        // 默认反隐形时间

    protected readonly MobControl _mobControl;
    protected readonly MobSituation _mobSituation;
    protected MobAIPath _aiPath;
    protected MobUnit _mobUnit;
    private float _dullTime;
    private MobSkillSituation _skillSituation;
    private Vector3 _lastDir = Vector3.zero;
    private Vector3 _lastTargetLoc = Vector3.zero;
    private bool _attackedCandidateLoc = false;

    public SearchAttackAIBehavior(MobUnit mobUit, MobSituation mobSituation, MobSkillSituation skillSituation)
    {
        _mobUnit = mobUit;
        _mobControl = mobUit.GetComponent<MobControl>();
        _mobSituation = mobSituation;
        _skillSituation = skillSituation;
//        doneSkillEvent += skillSituation.DoneSkill;
        _aiPath = mobUit.GetComponent<MobAIPath>();

    }


    public void activate(AI ai)
    {
    }

    public virtual bool doBehavior(float tpf)
    {
        var target = _mobSituation.GetTarget();
//        if (target)
//        {
//            var antiStealthEffect = _mobUnit.GetEffect(EffectConfig.EffectContextID.AntiStealthEffect) as AntiStealthEffectConfig;
//            if (antiStealthEffect == null)
//            {
//                antiStealthEffect = new AntiStealthEffectConfig(_mobUnit, AntiStealthDuration);
//                _mobUnit.AddEffect(antiStealthEffect);
//            }
//            Vector3 toTarget;
//            Vector3 candidateLoc;
//            if (_lastDir.Equals(Vector3.zero) || (_attackedCandidateLoc && !antiStealthEffect.HasHit(target) && _mobControl.GetCurrentSkill() == null))
//            {
//                // 随机选择地点, 攻击结束而且没有命中目标
//                var rot = Quaternion.AngleAxis(Random.value * 360f, Vector3.up);
//                _lastDir = rot * Vector3.right;
//                toTarget = _lastDir;
//                _lastTargetLoc = AstarPathUtil.GetValidPos(_mobUnit.transform.position +
//                                                           _lastDir * Random.value * MaxSearchDistance);
//                candidateLoc = _lastTargetLoc;
//                _attackedCandidateLoc = false;
//            }
//            else
//            {
//                toTarget = _lastDir;
//                candidateLoc = _lastTargetLoc;
//            }
//
//            MobSkillConfig skillConfig = _mobControl.GetSkillConfig(_skillSituation.getCurrentStatus());
//            float attackRange = 0f;
//            if (skillConfig)
//            {
//                attackRange = skillConfig.AttackRange + target.Radius + _mobUnit.Radius;
//            }
//            else
//            {
//                // 所有技能都处于CD中时, 设定观战距离5m
//                attackRange = 5f;
//            }
//            float toTargetDistanceSqr = Vector3.SqrMagnitude(candidateLoc - _mobUnit.transform.position);
//            if (target.FilterOnFreeFromAttacked() || _mobUnit.FilterOnDisarm())
//            {
//                // do nothing
//            }
//            else if (_mobControl.CanActiveSkill(skillConfig) && toTargetDistanceSqr <= attackRange * attackRange)
//            {
//                if (_mobControl.DoSimpleAttackWithoutCheck(toTarget, skillConfig))
//                {
//                    antiStealthEffect.ClearVictim();
//                    _attackedCandidateLoc = true;
//                }
//            }
//            else if (_mobControl.CanMove() && toTargetDistanceSqr > attackRange * attackRange)
//            {
//                // 靠近candidateLoc
//                _aiPath.MovingTargetProvider = null;
//                _aiPath.SetTargetPos(candidateLoc);
//                _aiPath.endReachedDistance = Mathf.Max(attackRange - 0.5f, 0.5f);
//                _aiPath.slowdownDistance = _aiPath.endReachedDistance + 1f;
//                _aiPath.EnableTrace(true);
//            }
//            else
//            {
//                _aiPath.EnableTrace(false);
//            }
//
//            return true;
//        }

        return false;
    }

    public void deactivate(AI ai)
    {
    }
}