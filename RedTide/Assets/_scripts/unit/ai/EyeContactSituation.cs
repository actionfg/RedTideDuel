using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 暂时只检测一次, 否则需要判断途中每个碰撞物是否需要过滤
public class EyeContactSituation : BaseAISituation
{
    private readonly MobSituation _mobSituation;
    private static readonly float UPDATE_INTERVAL = 1f;

    private bool _hasContacted = false;
    public EyeContactSituation(GameUnit owner, MobSituation mobSituation) : base(owner)
    {
        _mobSituation = mobSituation;
    }

    protected override bool shouldUpdate(float acc)
    {
        return acc >= UPDATE_INTERVAL;
    }

    protected override int updateSituation(float tpf)
    {
        if (_hasContacted) return (int) EyeContactState.Yes;

        var target = _mobSituation.GetTarget();
        if (target && EyeContact(target))
        {
            _hasContacted = true;
            return (int) EyeContactState.Yes;;
        }
        return (int) EyeContactState.No;
    }

    private bool EyeContact(GameUnit target)
    {
        var toTarget = (target.transform.position - GetOwner().transform.position);
        float rayRange = toTarget.magnitude;
        toTarget.Normalize();
        var origin = GetOwner().transform.position + new Vector3(0, 2, 0);
        var ray = new Ray(origin, toTarget );
        int mask = ~(LayerMask.GetMask("TileRoot") | LayerMask.GetMask("Ignore Raycast"));
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, rayRange, mask))
        {
            if (rayHit.collider.gameObject == target.gameObject || rayHit.collider.gameObject == GetOwner().gameObject)
            {
                return true;
            }
            return false;
        }
        return true;
    }

    public enum EyeContactState
    {
        No,
        Yes,
    }
}
