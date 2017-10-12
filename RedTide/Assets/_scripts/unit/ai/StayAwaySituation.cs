using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAwaySituation : BaseAISituation
{
    private static readonly float UPDATE_INTERVAL = 0.2f;
    private static readonly float EVADE_THRESHOLD = 5f;
    public static readonly float SAFE_DISTANCE = 8f; // 距离大于8m, 则不再逃跑
    public static readonly float SAFE_DISTANCE_SQR = SAFE_DISTANCE * SAFE_DISTANCE;
    public static readonly float FLEE_DISTANCE = 4f; // 距离小于6m的时长超过5s, 则触发逃跑
    public static readonly float FLEE_DISTANCE_SQR = FLEE_DISTANCE * FLEE_DISTANCE;

    private MobSituation _targetSituation;
    private int _latestState = 0;

    private float _accDanger;

    private float _fleeStamp; // 最后一次触发逃跑的时间

    public StayAwaySituation(GameUnit owner, MobSituation mobSituation) : base(owner)
    {
        _targetSituation = mobSituation;
    }

    protected override bool shouldUpdate(float acc)
    {
        return acc >= UPDATE_INTERVAL;
    }

    protected override int updateSituation(float tpf)
    {
        var target = _targetSituation.GetTarget();
        if (target)
        {
            MobUnit owner = (MobUnit) GetOwner();
            var toTarget = target.transform.position - owner.transform.position;
            toTarget.y = 0;
            var targetSqrMagnitude = toTarget.sqrMagnitude;
            if (targetSqrMagnitude <= FLEE_DISTANCE_SQR)
            {
                _accDanger += tpf;
                if (_accDanger > EVADE_THRESHOLD)
                {
                    _latestState = (int) EvadeState.Flee;
                    return _latestState;
                }
            }
            else if (targetSqrMagnitude > SAFE_DISTANCE * SAFE_DISTANCE)
            {
                _accDanger = 0f;
                _latestState =(int) EvadeState.None;
                return _latestState;
            }
        }
        return _latestState;
    }

    public enum EvadeState
    {
        None,
        Flee, // 逃跑
    }
}