using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropPlacement
{
    private static List<Vector3> _fixOffsets = new List<Vector3>();
    private static int _searchCount = 0;

    // 要求所有DropItem的collider大小一样, 最大1m的正方体范围
    // 先在center周围找1*3合法点, 若都填充满, 则在外围扩充一圈: @ # @
    public static void SetDropCenter(Vector3 center)
    {
        if (_fixOffsets.Count == 0)
        {
            InitFixOffsets();
        }

        _searchCount = 1;
    }

    private static bool IsValid(Vector3 loc)
    {
        var nnInfo = AstarPath.active.GetNearest(loc);
        if (nnInfo.node != null)
        {
            if (nnInfo.node.Walkable)
            {
                var offset = nnInfo.clampedPosition - loc;
                if (offset.sqrMagnitude < 0.25f)
                {
                    return true;
                }
            }

        }
        return false;
    }

    private static void InitFixOffsets()
    {
        _fixOffsets.Add(new Vector3(0, 0, 0));
        _fixOffsets.Add(new Vector3(-2, 0, 0));
        _fixOffsets.Add(new Vector3(2, 0, 0));

        _fixOffsets.Add(new Vector3(-2, 0, 2));
        _fixOffsets.Add(new Vector3(0, 0, 2));
        _fixOffsets.Add(new Vector3(2, 0, 2));

        _fixOffsets.Add(new Vector3(-2, 0, -2));
        _fixOffsets.Add(new Vector3(0, 0, -2));
        _fixOffsets.Add(new Vector3(2, 0, -2));

        _fixOffsets.Add(new Vector3(-4, 0, 0));
        _fixOffsets.Add(new Vector3(4, 0, 0));
        _fixOffsets.Add(new Vector3(-4, 0, 4));
        _fixOffsets.Add(new Vector3(4, 0, 4));
        _fixOffsets.Add(new Vector3(-4, 0, -4));
        _fixOffsets.Add(new Vector3(4, 0, -4));

        _fixOffsets.Add(new Vector3(0, 0, 6));
        _fixOffsets.Add(new Vector3(-2, 0, 6));
        _fixOffsets.Add(new Vector3(2, 0, 6));
        _fixOffsets.Add(new Vector3(-4, 0, 6));
        _fixOffsets.Add(new Vector3(4, 0, 6));

        _fixOffsets.Add(new Vector3(0, 0, -6));
        _fixOffsets.Add(new Vector3(-2, 0, -6));
        _fixOffsets.Add(new Vector3(2, 0, -6));
        _fixOffsets.Add(new Vector3(-4, 0, -6));
        _fixOffsets.Add(new Vector3(4, 0, -6));
    }



    public static Vector3 GetDropLoc(Vector3 center)
    {
        while (_searchCount < _fixOffsets.Count)
        {
            var candidateLoc = center + _fixOffsets[_searchCount++];
            if (IsValid(candidateLoc))
            {
                return candidateLoc;
            }
        }

        return center;
    }

    public static Vector3 GetFloorLoc(Vector3 location)
    {
        Ray ray = new Ray((location + Vector3.up * 3), Vector3.down);
        RaycastHit rayHit;
        int mask = LayerMask.GetMask("Floor");
        if (Physics.Raycast(ray, out rayHit, 100, mask))
        {
            return new Vector3(location.x, rayHit.point.y, location.z);
        }
        return location;
    }
}