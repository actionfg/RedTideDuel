using System;
using UnityEngine;
using System.Collections;

public class LockedTargetMoveControl : MoveControl {
    private Transform _target;
    private Vector3 _targetPos;

    public LockedTargetMoveControl(GameObject bingdingObjcet, float flySpeed, float turnSpeed, float maxRange, Transform target, Vector3 targetPos) :
        base(bingdingObjcet, flySpeed, turnSpeed, maxRange)
    {
        if (target)
        {
            _target = target;
            _targetPos = Vector3.zero;
        }
        else
        {
            _target = null;
            _targetPos = targetPos;
        }
    }


    public override bool Update()
    {
        if (_target == null && _targetPos.Equals(Vector3.zero)) return false;

        Vector3 toTarget;
        if (_target != null)
        {
            toTarget = _target.position + Vector3.up * 1.5f - BingdingObject.transform.position;
        }
        else
        {
            toTarget = _targetPos - BingdingObject.transform.position;
        }

        if (toTarget.sqrMagnitude < 0.5f * 0.5f)
        {// 已到达目标
            return true;
        }

        RotateTowardsGradually(toTarget);
        return MoveForward();

    }



}
