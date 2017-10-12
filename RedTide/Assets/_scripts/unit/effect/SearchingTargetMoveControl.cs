using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class SearchingTargetMoveControl : MoveControl {
    private Transform _target;
    private SelectTargetType _selectType;


    public SearchingTargetMoveControl(GameObject bindingObject, float flySpeed, float turnSpeed, float maxRange, SelectTargetType selectTargetType) :
        base(bindingObject, flySpeed, turnSpeed, maxRange)
    {
        _selectType = selectTargetType;
    }

    public override bool Update()
    {
        if (_target)
        {
            MoveToTarget();
        }
        else
        {
            SearchTarget();
            return MoveForward();
        }
        return true;
    }

    private bool MoveToTarget()
    {
        if (_target == null) return false;

        var toTarget = _target.position - BingdingObject.transform.position;
        if (toTarget.sqrMagnitude < 0.5f * 0.5f)
        {// 已到达目标
            return true;
        }

        RotateTowardsGradually(toTarget);
        return MoveForward();
    }

    private void SearchTarget()
    {
        var enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        switch (_selectType)
        {
            case SelectTargetType.Random:
                var randomIndex = (enemyObjects.Length - 1) * Random.value;
                _target = enemyObjects[(int) randomIndex].transform;
                Debug.DrawLine(BingdingObject.transform.position, _target.position);
                break;
            case SelectTargetType.Closest:
                selectTheNearest(enemyObjects);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private void selectTheNearest(GameObject[] enemyObjects)
    {
        GameObject closest = null;
        float distanceSqr = Mathf.Infinity;

        foreach (GameObject enemy in enemyObjects)
        {
            var dir = enemy.transform.position - BingdingObject.transform.position;
            float curDist = dir.sqrMagnitude;
            if (curDist < distanceSqr)
            {
                distanceSqr = curDist;
                closest = enemy;
            }
        }
        if (closest)
        {
            _target = closest.transform;
        }
    }
}
