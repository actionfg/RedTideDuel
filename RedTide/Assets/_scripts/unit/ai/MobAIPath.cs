using System;
using UnityEngine;
using System.Collections;
using Pathfinding;

// 负责角色移动
public class MobAIPath : AIPath
{
    private CharacterController _characterController;
    public IMovingTargetProvider MovingTargetProvider { get; set; }
    public IMovingListener MovingListener { get; set; }
    private Vector3 _impact = Vector3.zero;
    private Vector3 _speedY = Vector3.zero;

    private IEnumerator _retryCoroutine;

    protected override void Start()
    {
        base.Start();
        _characterController = GetComponent<CharacterController>();
    }

    public override void RotateTowards(Vector3 dir)
    {
        if (dir == Vector3.zero)
        {
            return;
        }

        if (Math.Abs(turningSpeed) < 0.001f)
        {
            tr.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            base.RotateTowards(dir);
        }
    }

    public override void SetTargetTransform(Transform targetTransform)
    {
        if (MovingTargetProvider != null)
        {
            target = targetTransform;
        }
        else
        {
            base.SetTargetTransform(targetTransform);
        }
    }

    public override Vector3 GetActualTargetLoc()
    {
        if (MovingTargetProvider != null)
        {// 优先Provider
            return MovingTargetProvider.GetNextTargetLoc(false);
        }
        return base.GetActualTargetLoc();
    }

    public override void OnPathComplete(Path _p)
    {
        base.OnPathComplete(_p);

        if (path == null)
        {
            return;
        }
        // 检查是否终点在Target附近, 若无法到达, 则进入随机巡逻
        var vPath = path.vectorPath;
        if (vPath.Count > 0 && target != null)
        {
            float distanceSqr = Vector3.SqrMagnitude(target.position - vPath[vPath.Count - 1]);
            if (distanceSqr > 2f * 2f)
            {
                if (MovingTargetProvider == null)
                {
                    var patrolRouteProvider = new PatrolRouteProvider();
                    patrolRouteProvider.RandPatrol = true;
                    patrolRouteProvider.Radius = 5f;
                    patrolRouteProvider.Center = transform.position;

                    MovingTargetProvider = patrolRouteProvider;
                    slowdownDistance = 0.8f;
                    endReachedDistance = 0.5f;
                    SetTargetPos(patrolRouteProvider.GetNextTargetLoc(true));
//                    Debug.Log(gameObject.name + " can't reach target, Rand Patrol!!!");
                    
                    // 同时间隔一段时间, 重新搜索Target路径
                    _retryCoroutine = RetrySearchPathToTarget();
                    StartCoroutine(_retryCoroutine);
                }
            }
            else
            {// 已经可以寻路径至目标, 则停止随机行走
                MovingTargetProvider = null;
                if (_retryCoroutine != null)
                {
                    StopCoroutine(_retryCoroutine);
                }
            }
        }
    }

    private IEnumerator RetrySearchPathToTarget()
    {// 再次尝试寻找至Target的路径
        while (true)
        {
            yield return new WaitForSeconds(2f * repathRate);
            if (target != null)
            {
                seeker.StartPath (GetFeetPosition(), target.position);
            }
        }
        yield return null;
    }

    public override void Update ()
    {
        _speedY += Physics.gravity * Time.deltaTime;
        if (MovingListener != null)
        {
            MovingListener.NotifyMoving(false);
        }
        // 计算impact部分
        Vector3 impactSpeed = Vector3.zero;
        if (_impact.sqrMagnitude > 0.04f)
        {
            impactSpeed = _impact;
        }
        _impact = Vector3.Lerp(_impact, Vector3.zero, 10 * Time.deltaTime);

        // 计算寻径网格移动部分
        if (MovingTargetProvider != null)
        {
            SetTargetPos(MovingTargetProvider.GetNextTargetLoc(false), false);
        }
        else
        {
            if (target == null && GetTargetPos() == Vector3.zero)
            {
                if (_characterController)
                {
                    ResetSpeedY(_characterController.Move((impactSpeed + _speedY) * Time.deltaTime));
                }
                return;
            }
        }

        var targetPosition = GetActualTargetLoc();
        var toTarget = transform.position - targetPosition;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude <= endReachedDistance * endReachedDistance)
        {
            OnTargetReached();
        }

        if (!canMove)
        {
            if (_characterController)
            {
                ResetSpeedY(_characterController.Move((impactSpeed + _speedY) * Time.deltaTime));
            }
            return;
        }


        Vector3 dir = CalculateVelocity (GetFeetPosition()) * GlobalFactor.Instance.NpcSpeed;
        //Rotate towards targetDirection (filled in by CalculateVelocity)
        if (LookDir.Equals(Vector3.zero))
        {
            RotateTowards(targetDirection);
        }
        else
        {
            RotateTowards(LookDir);
        }

        if (rigid)
        {
            // 使用Rigid.MovePosition方式上坡, 会因为重力原因上坡困难, 而且下坡加速
            dir.y = rigid.velocity.y;
            rigid.velocity = dir;
        }
        else if (_characterController)
        {
            if (_characterController.enabled)
            {
                var collisionFlags = _characterController.Move((impactSpeed + dir + _speedY) * Time.deltaTime);
                ResetSpeedY(collisionFlags);
            }
        }
        else
        {
            transform.Translate(dir * Time.deltaTime, Space.World);
        }
        if (MovingListener != null && dir.sqrMagnitude > 0.01f)
        {
            MovingListener.NotifyMoving(true);
        }
    }

    private void ResetSpeedY(CollisionFlags collisionFlags)
    {
        if ((collisionFlags & CollisionFlags.Below) != 0)
        {
            _speedY = Vector3.zero;
        }
    }

//    private void OnDrawGizmos()
//    {
//        Gizmos.DrawWireCube(_latestHit, new Vector3(0.1f, 0.1f, 0.1f));
//    }

    public new void OnTargetReached()
    {
        if (MovingTargetProvider != null)  //  有额外路点提供
        {
            var nextTargetLoc = MovingTargetProvider.GetNextTargetLoc(true);
            if (nextTargetLoc != null && !nextTargetLoc.Equals(Vector3.zero))
            {
                SetTargetPos(nextTargetLoc, false);
            }
            else
            {
                canSearchAgain = false;
                canMove = false;
            }
        }
        else
        {
            canSearchAgain = false;
            canMove = false;
        }

        if (rigid)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }


    }

    public void EnableTrace(bool enable)
    {
        canSearchAgain = enable;
        canMove = enable;
        if (!enable)
        {
            SetTargetPos(Vector3.zero);
            target = null;
            LookDir = Vector3.zero;
        }
    }

   // 不用严格按着路线走, 当目标跳到下一路点时, 直接朝目标移动, 而不用移至路径直线上
    protected override Vector3 CalculateTargetPoint(Vector3 p, Vector3 a, Vector3 b)
    {
        return b;
    }

    public void AddImpact(Vector3 impact)
    {
        _impact += impact;
    }
}
