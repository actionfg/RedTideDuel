using UnityEngine;
using System.Collections;

public abstract class MoveControl
{

    protected GameObject BingdingObject;
    protected float FlySpeed;
    protected float TurnSpeed;             // 每秒旋转度数(degree)
    protected float MaxRange;
    protected float FlyingRange = 0;


    protected MoveControl(GameObject bindingObject, float flySpeed, float maxRange)
    {
        BingdingObject = bindingObject;
        FlySpeed = flySpeed;
        MaxRange = maxRange;
        TurnSpeed = 0;
    }

    protected MoveControl(GameObject bindingObject, float flySpeed, float turnSpeed, float maxRange)
    {
        BingdingObject = bindingObject;
        FlySpeed = flySpeed;
        MaxRange = maxRange;
        TurnSpeed = turnSpeed;
    }


    public abstract bool Update();

    protected bool MoveForward()
    {
        return MoveToward(Direction.Forward);
    }

    protected bool MoveToward(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            // 直线飞行
            var delta = FlySpeed * Time.deltaTime;
            BingdingObject.transform.Translate(dir * delta, Space.World);
            FlyingRange += delta;
            if (FlyingRange >= MaxRange)
            {
                return false;
            }
        }
        return true;
    }

    protected bool MoveToward(Direction direction)
    {
        if (BingdingObject == null) return false;

        Vector3 dir = Vector3.zero;
        switch (direction)
        {
            case Direction.Left:
                dir = -BingdingObject.transform.right;
                break;
            case Direction.Right:
                dir = BingdingObject.transform.right;
                break;
            case Direction.Up:
                dir = BingdingObject.transform.up;
                break;
            case Direction.Down:
                dir = -BingdingObject.transform.up;
                break;
            case Direction.Forward:
                dir = BingdingObject.transform.forward;
                break;
            case Direction.Backward:
                dir = -BingdingObject.transform.forward;
                break;
        }

        if (dir != Vector3.zero)
        {
            // 直线飞行
            var delta = FlySpeed * Time.deltaTime;
            BingdingObject.transform.Translate(dir * delta, Space.World);
            FlyingRange += delta;
            if (FlyingRange >= MaxRange)
            {
                return false;
            }
        }
        return true;
    }

    protected void RotateTowardsGradually(Vector3 dir)
    {
        if (dir == Vector3.zero)
        {
            return;
        }
        Quaternion rot = BingdingObject.transform.rotation;
        Quaternion toTarget = Quaternion.LookRotation (dir);

        float factor = TurnSpeed * Time.deltaTime / Quaternion.Angle(rot, toTarget);
        rot = Quaternion.Slerp (rot, toTarget, factor);
//        Vector3 euler = rot.eulerAngles;        // 限定只绕Y轴旋转
//        euler.z = 0;
//        euler.x = 0;
//        rot = Quaternion.Euler (euler);

        BingdingObject.transform.rotation = rot;
    }

    protected void RotateTowards(Vector3 dir)
    {
        if (dir == Vector3.zero)
        {
            return;
        }

        BingdingObject.transform.rotation = Quaternion.LookRotation (dir);
    }

    public void SetFlySpeed(float flySpeed)
    {
        FlySpeed = flySpeed;
    }

}
