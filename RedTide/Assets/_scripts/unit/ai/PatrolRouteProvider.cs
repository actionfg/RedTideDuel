using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRouteProvider : IMovingTargetProvider
{
    // 在刷新点固定半径圆形范围随意走动
    public bool RandPatrol;
    public float Radius;
    public Vector3 Center { get; set; }

    public float PatrolDelay;

    public List<Transform> PatrolRoute;
    
    private int _patrolIndex = 0;
    private Vector3 _latestTarget = Vector3.zero;

    public Vector3 GetNextTargetLoc(bool reachTarget)
    {
        if (reachTarget || _latestTarget.Equals(Vector3.zero))
        {
            if (RandPatrol)
            {
                float length = Random.value * Radius;
                var angleAxis = Quaternion.AngleAxis(Random.value * 360f, Vector3.up);
                var dir = angleAxis * Vector3.forward;
                var candidateLoc = Center + dir * length;
                _latestTarget = AstarPathUtil.GetValidPos(candidateLoc);
            }
            else if (PatrolRoute.Count > 1)
            {
                _patrolIndex += 1;
                _patrolIndex = _patrolIndex % PatrolRoute.Count;
                _latestTarget = PatrolRoute[_patrolIndex].position;
            }
        }

        return _latestTarget;
    }
}
