using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeCollideBehaior : AIBehavior {
    private readonly MobUnit _mobUnit;
    private readonly MobSituation _mobSituation;
    private MobAIPath _aiPath;
    private MobControl _mobControl;

    public EvadeCollideBehaior(MobUnit mobUnit, MobSituation mobSituation)
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
        var target = _mobSituation.GetTarget();
        if (target)
        {
            var toTarget = target.transform.position - _mobUnit.transform.position;
            toTarget.y = 0f;
            var radiusTogether = _mobUnit.Radius + target.Radius;
            float radiusSqr = radiusTogether * radiusTogether * 1.2f;
            if (toTarget.sqrMagnitude < radiusSqr)
            {
                if (_mobControl.CanMove())
                {
                    //实现面朝玩家后退
                    toTarget.Normalize();

                    // 一直往后移直到RetreatState改变
                    _mobUnit.AddEffect(new UnRegularMoveEffect(_mobUnit));
                    var targetLoc = _mobUnit.transform.position + toTarget * -radiusTogether;
                    _aiPath.SetTargetPos(targetLoc);
                    _aiPath.LookDir = toTarget;
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
