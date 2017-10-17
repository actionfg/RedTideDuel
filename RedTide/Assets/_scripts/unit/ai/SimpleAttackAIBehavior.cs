using UnityEngine;
using System.Collections;

public class SimpleAttackAIBehavior : AIBehavior
{
    public float MaxDullDuration = 3f;

    protected readonly MobControl _mobControl;
    protected readonly MobSituation _mobSituation;
    protected readonly MobSkillSituation _skillSituation;
    protected MobAIPath _aiPath;
    protected MobUnit _mobUnit;
    private float _dullTime;

//    public delegate void ActiveSkillHandler(SkillConfig skillConfig);
//
//    public event ActiveSkillHandler activeSkillEvent;

    public SimpleAttackAIBehavior(MobUnit mobUit, MobSituation mobSituation, MobSkillSituation skillSituation)
    {
        _mobUnit = mobUit;
        _mobControl = mobUit.GetComponent<MobControl>();
        _mobSituation = mobSituation;
        _skillSituation = skillSituation;
//        activeSkillEvent += skillSituation.ActiveSkill;
        _aiPath = mobUit.GetComponent<MobAIPath>();

        MaxDullDuration = mobUit.DullDuration;
    }


    public void activate(AI ai)
    {
    }

    public virtual bool doBehavior(float tpf)
    {
        // 进入攻击范围则攻击, 否则追击
        var target = _mobSituation.GetTarget();
        if (target)
        {
            var targetTransform = target.transform;

            var toTarget = targetTransform.position - _mobControl.transform.position;
            toTarget.y = 0;
            MobSkillConfig skillConfig = _mobControl.GetSkillConfig(_skillSituation.getCurrentStatus());
            float attackRange = 0f;
            if (skillConfig)
            {
                attackRange = skillConfig.AttackRange + target.Radius + _mobUnit.Radius;
            }
            else
            {
                // 所有技能都处于CD中时, 设定观战距离5m
                attackRange = _mobUnit.Config.ActiveRange;
            }

            var targetSqrMagnitude = toTarget.sqrMagnitude;
            if (targetSqrMagnitude <= attackRange * attackRange )
            {
                _aiPath.MovingTargetProvider = null;
                toTarget.Normalize();
                if (Time.time >= _dullTime && _mobControl.CanActiveSkill(skillConfig))
                {
                    _mobControl.DoSimpleAttackWithoutCheck(toTarget, skillConfig);
                    _aiPath.EnableTrace(false);
                }
                else if (targetSqrMagnitude <= (attackRange - 2) * (attackRange - 2))
                {// 防止在转向目标和转向前进方向来回切换 
                    // 不攻击时, 根据转向速度转向目标
                    MovingInAttackInterval(toTarget, targetTransform, attackRange);
                }
            }
            else if (_mobControl.CanMove() && targetSqrMagnitude > attackRange * attackRange)
            {
                var moveEffect = _mobUnit.GetEffect(EffectConfig.EffectContextID.UnRegularMoveEffect);
                if (moveEffect != null)
                {
                    ((UnRegularMoveEffect) moveEffect).Activated = false;
                }

                _aiPath.SetTargetTransform(targetTransform);
                if (_aiPath.MovingTargetProvider == null)
                {
                    _aiPath.endReachedDistance = Mathf.Max(attackRange - 0.5f, 0.5f);
                    _aiPath.slowdownDistance = _aiPath.endReachedDistance + 1f;
                }
                _aiPath.EnableTrace(true);

                // 设置靠近后迟钝时间
                _dullTime = Time.time + Random.value * MaxDullDuration;
            }
            else
            {
                _aiPath.EnableTrace(false);
            }
           
            if (!_mobControl.CanMove() && _mobControl.CanRotate())
            {// 能移动时由移动脚步决定朝向
                _aiPath.RotateTowards(toTarget);   
            }
            return true;
        }

        return false;
    }

    protected virtual void MovingInAttackInterval(Vector3 toTarget, Transform targetTransform, float attackRange)
    {
        _aiPath.RotateTowards(toTarget);
        _aiPath.EnableTrace(false);
    }

    public void deactivate(AI ai)
    {
    }
}