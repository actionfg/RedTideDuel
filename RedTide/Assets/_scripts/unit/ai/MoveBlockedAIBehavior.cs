using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlockedAIBehavior : AIBehavior {
    private MobUnit _mobUnit;
    private MobSituation _mobSituation;
    private MobAIPath _aiPath;
    private MobControl _mobControl;

    public MoveBlockedAIBehavior(MobUnit mobUnit, MobSituation mobSituation)
    {
        _mobUnit = mobUnit;
        _mobSituation = mobSituation;
        _aiPath = mobUnit.GetComponent<MobAIPath>();
        _mobControl = mobUnit.GetComponent<MobControl>();
    }

    public void activate(AI ai)
    {
    }

    public bool doBehavior(float tpf)
    {
        // 往Collider的HitNormal之和方向前进
        if (_mobControl.CanMove())
        {
            _mobUnit.AddEffect(new UnRegularMoveEffect(_mobUnit));
            var targetLoc = _mobUnit.transform.position + _mobControl.CollideNormal.normalized * 4f;
            // TODO 测试看看会不会出现来回拖拉情况
            _aiPath.SetTargetPos(targetLoc);
            var target = _mobSituation.GetTarget();
            if (target)
            {
                var toTarget = target.transform.position - _mobUnit.transform.position;
                toTarget.y = 0f;
                _aiPath.LookDir = toTarget;
            }
            _aiPath.endReachedDistance = 0.5f;
            _aiPath.slowdownDistance = _aiPath.endReachedDistance + 1.5f;
            _aiPath.EnableTrace(true);
        }
        return false;
    }

    public void deactivate(AI ai)
    {
    }
}
