using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAwayAIBehavior : AIBehavior
{
    private readonly MobUnit _mobUnit;
    private readonly MobSituation _mobSituation;
    private readonly float _safeDistance;
    private MobAIPath _aiPath;
    private MobControl _mobControl;

    public StayAwayAIBehavior(MobUnit mobUnit, MobSituation mobSituation, float safeDistance)
    {
        _mobUnit = mobUnit;
        _mobSituation = mobSituation;
        _safeDistance = safeDistance;
        _aiPath = mobUnit.GetComponent<MobAIPath>();
        _mobControl = mobUnit.GetComponent<MobControl>();
    }


    public void activate(AI ai)
    {
    }

    // 与玩家距离超过8米时停止逃跑
    public bool doBehavior(float tpf)
    {
        if (_mobUnit.Dead) return false;

        var target = _mobSituation.GetTarget();
        if (target)
        {
            var awayTarget = _mobUnit.transform.position - target.transform.position;
            awayTarget.y = 0f;
            if (awayTarget.sqrMagnitude < _safeDistance * _safeDistance)
            {
                if (_mobControl.CanMove())
                {
                    awayTarget.Normalize();
                    // 逃跑目标点设远一点, 防止过早退出, 又进入攻击节奏
                    var candidateTarget = _mobUnit.transform.position + awayTarget * _safeDistance * 2f;
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
