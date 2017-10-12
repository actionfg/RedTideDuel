using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileControl : MonoBehaviour, RemovalControl
{

    public ProjectileType ProjectileType = ProjectileType.Direction;
    public Direction Direction = Direction.Forward;
    public float FlySpeed;
    public float BearingAngle;
    public SelectTargetType SelectTargetType;
    public Transform Target;            // 锁定目标, 跟踪移动
    public Vector3 TargetPos;            // 锁定目标点
    public float MaxRange = float.MaxValue;
    public int TriggerUnitCount = Int32.MaxValue;
    public int TriggerSceneCount = Int32.MaxValue;
    public float TurningSpeed = 30f;
    public float LifeTime = float.MaxValue;
    public bool ActiveSkillOnDestroy = false;       // 是否在结束时触发一次技能

    protected bool Active = true;
    private MoveControl _moveControl;
    private GameUnit _caster;

    public void SetCaster(GameUnit caster)
    {
        _caster = caster;
    }

    private void Start()
    {
        switch (ProjectileType)
        {
            case ProjectileType.None:
                break;
            case ProjectileType.Direction:
                Vector3 dir = GetFlyingDir(Direction, _caster);
                _moveControl = new DirectionMoveControl(gameObject, dir, FlySpeed, MaxRange);
                break;
            case ProjectileType.Lock:
                float speed = FlySpeed;
                if (LifeTime > 0 && LifeTime < 1000f)
                {
                    speed = Vector3.Distance(TargetPos, gameObject.transform.position) / LifeTime;
                }
                if (TargetPos.Equals(Vector3.zero) && Target == null && Direction == Direction.ToTarget)
                {
                    if (_caster is MobUnit)
                    {// 将caster目标锁定
                        Target = ((MobUnit) _caster).GetTarget().transform;
                    }
                }
                _moveControl = new LockedTargetMoveControl(gameObject, speed, TurningSpeed, MaxRange, Target, TargetPos);
                break;
            case ProjectileType.Searching:
                _moveControl =
                    new SearchingTargetMoveControl(gameObject, FlySpeed, TurningSpeed, MaxRange, SelectTargetType);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Vector3 GetFlyingDir(Direction direction, GameUnit caster)
    {

        Vector3 dir = Vector3.zero;
        switch (direction)
        {
            case Direction.Left:
                dir = -gameObject.transform.right;
                break;
            case Direction.Right:
                dir = gameObject.transform.right;
                break;
            case Direction.Up:
                dir = gameObject.transform.up;
                break;
            case Direction.Down:
                dir = -gameObject.transform.up;
                break;
            case Direction.Forward:
                dir = gameObject.transform.forward;
                break;
            case Direction.Backward:
                dir = -gameObject.transform.forward;
                break;
            case Direction.ForwardUp2Target:
            {
                var mobUnit = caster as MobUnit;
                if (mobUnit != null)
                {
                    GameUnit targetUnit = mobUnit.GetTarget();
                    if (targetUnit != null)
                    {
                        var toTarget = targetUnit.GetUnitTransform().position + Vector3.up * 1.5f - transform.position;
                        var angle = Mathf.Rad2Deg * Mathf.Asin(toTarget.y / toTarget.magnitude);
                        dir = transform.forward;
                        var upRot = Quaternion.AngleAxis(-angle, transform.right);
                        dir = upRot * dir;
                    }
                }
                else
                {
                    dir = gameObject.transform.forward;
                }
                break;
            }
            case Direction.Bearing:
                dir = gameObject.transform.forward;
                var angleAxis = Quaternion.AngleAxis(BearingAngle, Vector3.up);
                dir = angleAxis * dir;
                break;
            case Direction.ToTarget:
            {
                if (Target == null)
                {
                    dir = gameObject.transform.forward;
                    var mobUnit = caster as MobUnit;
                    if (mobUnit != null)
                    {
                        GameUnit targetUnit = mobUnit.GetTarget();
                        if (targetUnit != null)
                        {
                            dir = (targetUnit.GetUnitTransform().position + Vector3.up * 1.5f - gameObject.transform.position).normalized;
                        }
                    }
                }
                else
                {
                    dir = (Target.position - gameObject.transform.position).normalized;
                }
            }
            break;
        }
        return dir;
    }

    public virtual void Update()
    {
        if (Active)
        {
            if (_moveControl != null)
            {
                Active = _moveControl.Update();
            }
            CheckLifeTime();
            if (!Active && ActiveSkillOnDestroy)
            {
                var effectObject = GetComponent<EffectObject>();
                if (effectObject)
                {// 结束时触发一次技能
                    effectObject.OnTrigger(null, null, _caster);
                }
            }
        }
        else
        {
            var poolObjectMark = GetComponent<PoolObjectMark>();
            if (poolObjectMark)
            {
                poolObjectMark.Unspawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // 由EffectObject碰到其他物体后, 通知ProjectileControl
    public void notifyCollideWith(GameUnit caster, GameObject victim)
    {
        var targetUnit = victim.GetComponent<GameUnit>();
        if (targetUnit)
        {
            if (targetUnit.isEnemy(caster))
            {
                TriggerUnitCount -= 1;
            }
            if (TriggerUnitCount <= 0)
            {
                Active = false;
            }
        }
        else
        {
            TriggerSceneCount -= 1;
            if (TriggerSceneCount <= 0)
            {
                Active = false;
            }
        }
    }

    private void CheckLifeTime()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime < 0)
        {
            Active = false;
        }
    }

    public void SetFlySpeed(float flySpeed)
    {
        if (_moveControl != null)
        {
            _moveControl.SetFlySpeed(flySpeed);
        }
    }

}
