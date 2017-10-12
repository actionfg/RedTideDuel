using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class DirectionMoveControl : MoveControl
{
    private Vector3 _direction;

    public DirectionMoveControl(GameObject bindObject, Vector3 direction, float speed, float maxRange) : base(bindObject, speed, maxRange)
    {
        _direction = direction;
        RotateTowards(direction);
    }

    public override bool Update()
    {
        return MoveToward(_direction);
    }
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
    Forward,
    Backward,
    ToTarget,
    Bearing,                // 初始方向为与Forwar的Up方向固定夹角
    ForwardUp2Target,      // 考虑角色高度的Forward类型    
}
