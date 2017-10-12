using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IMovingTargetProvider
{

    // reachTarget为true, 则新生成目标点, 否则返回上次生成的目标点
    Vector3 GetNextTargetLoc(bool reachTarget);
}
