using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeAIBehavior : AIBehavior {
    private readonly MobUnit _mobUnit;
    private readonly MobSituation _mobSituation;
    private MobAIPath _aiPath;
    private MobControl _mobControl;

    private GameObject _targetObject = null;

    public FleeAIBehavior(MobUnit mobUnit, MobSituation mobSituation)
    {
        _mobUnit = mobUnit;
        _mobSituation = mobSituation;
        _aiPath = mobUnit.GetComponent<MobAIPath>();
        _mobControl = mobUnit.GetComponent<MobControl>();
    }


    public void activate(AI ai)
    {
    }

    // 与玩家角色当前朝向相同的方向逃跑。
    // 尽可能往最近怪物位置逃跑
    // 与玩家距离超过5米时停止逃跑
    public bool doBehavior(float tpf)
    {
        if (_mobUnit.Dead) return false;

        var target = _mobSituation.GetTarget();
        if (target)
        {
            var awayTarget = _mobUnit.transform.position - target.transform.position;
            awayTarget.y = 0f;
            if (awayTarget.sqrMagnitude < EvadeCollideSituation.FLEE_DISTANCE_SQR)
            {
                if (_mobControl.CanMove())
                {
                    // 搜索位于awayTarget指向上, 且在5米外的怪物, 朝其移动,
                    // 若没有, 则沿awayTarget移动
                    // 已选定的怪物若进入5m范围, 则另选目标
                    awayTarget.Normalize();
                    // TODO 逃跑目标点设远一点, 防止过早退出, 又进入攻击节奏
                    var candidateTarget = _mobUnit.transform.position + awayTarget * EvadeCollideSituation.FLEE_DISTANCE * 2f;
                    if (_targetObject == null || Vector3.SqrMagnitude(_targetObject.transform.position - target.transform.position)
                        <= EvadeCollideSituation.FLEE_DISTANCE_SQR)
                    {
                        // TODO 是否需要区分善意, 中立和仇视?
                        var enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
                        float nearestDistanceSqr = float.MaxValue;
                        GameObject nearestEnemy = null;
                        foreach (var enemyObject in enemyObjects)
                        {
                            var toSearchedObject = enemyObject.transform.position - target.transform.position;
                            var distSqr = Vector3.SqrMagnitude(toSearchedObject);
                            if (!enemyObject.Equals(_mobUnit.gameObject) && distSqr <= EvadeCollideSituation.FLEE_DISTANCE_SQR && distSqr < nearestDistanceSqr && Vector3.Dot(toSearchedObject, awayTarget) > 0)
                            {
                                nearestDistanceSqr = distSqr;
                                nearestEnemy = enemyObject;
                            }
                        }
                        _targetObject = nearestEnemy;
                    }
                    if (_targetObject != null)
                    {
                        candidateTarget = _targetObject.transform.position;
                    }

                    _aiPath.SetTargetPos(candidateTarget);
                    _aiPath.LookDir = Vector3.zero;
                    _aiPath.endReachedDistance = 0.5f;
                    _aiPath.slowdownDistance = _aiPath.endReachedDistance + 1.5f;
                    _aiPath.EnableTrace(true);
					return true;
                }
            }
        }
        _aiPath.EnableTrace(false);
        return false;
    }

    public void deactivate(AI ai)
    {
    }
}
