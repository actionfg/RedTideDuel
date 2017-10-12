using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpStayAwaySituation : BaseAISituation
{
    private static readonly float UPDATE_INTERVAL = 0.5f;
    private float _fleeDistance; // 距离小于3m, 则触发逃跑
    private float FleeThreshold { get; set; }

    private MobSituation _targetSituation;

    public ImpStayAwaySituation(GameUnit owner, MobSituation mobSituation, float fleeDistance) : base(owner)
    {
        _targetSituation = mobSituation;
        _fleeDistance = fleeDistance;
        if (GetOwner() is MobUnit)
        {
            var mobUnit = (MobUnit) GetOwner();
            FleeThreshold = mobUnit.Config.FleeThreshold;
        }

    }

    protected override bool shouldUpdate(float acc)
    {
        return acc >= UPDATE_INTERVAL;
    }

    protected override int updateSituation(float tpf)
    {
        var target = _targetSituation.GetTarget();
        if (target && GetOwner().CurrentHp < FleeThreshold * GetOwner().MaxHp)
        {
            MobUnit owner = (MobUnit) GetOwner();
            var toTarget = target.transform.position - owner.transform.position;
            toTarget.y = 0;
            var targetSqrMagnitude = toTarget.sqrMagnitude;
            if (targetSqrMagnitude <= _fleeDistance * _fleeDistance)
            {
                return (int) EvadeState.Flee;
            }
            else if (targetSqrMagnitude > 2f * _fleeDistance * _fleeDistance)
            {
                return (int) EvadeState.None;
            }
        }
        return (int) EvadeState.None;
    }

    public enum EvadeState
    {
        None,
        Flee, // 逃跑
    }
}