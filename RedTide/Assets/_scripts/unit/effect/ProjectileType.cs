using UnityEngine;
using System.Collections;

public enum ProjectileType {
    None,                // 原地不动
    Direction,           // 直线飞行
    Lock,                // 跟踪导弹
    Searching,             // 寻找周边敌人
}
