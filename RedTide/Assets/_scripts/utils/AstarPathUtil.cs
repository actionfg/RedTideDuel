using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarPathUtil {

    public static Vector3 GetValidPos(Vector3 candidatePos)
    {
        var nnInfo = AstarPath.active.GetNearest(candidatePos);
        if (nnInfo.node != null)
        {
            if (nnInfo.node.Walkable)
            {
                return nnInfo.clampedPosition;
            }
        }
        Debug.Log("Can't find legal path: " + candidatePos);
        return candidatePos;
    }


}
