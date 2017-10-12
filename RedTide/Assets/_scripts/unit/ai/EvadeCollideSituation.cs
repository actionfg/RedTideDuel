using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeCollideSituation : BaseAISituation
{
    private static readonly float UPDATE_INTERVAL = 0.2f;
    private static readonly float EVADE_CD = 4f; // 暂定的距离保持CD时间
    public static readonly float FLEE_DISTANCE = 5f; // 逃跑距离为5m
    public static readonly float FLEE_DISTANCE_SQR = FLEE_DISTANCE * FLEE_DISTANCE;

    private MobSituation _targetSituation;
    private int _latestState = 0;
    private float _evadeStamp; // 最后一次触发距离保持的时间


    private float _fleeStamp; // 最后一次触发逃跑的时间
    private float FleeCd { set; get; }
    private float FleeThreshold { set; get; }
    private bool _canFlee;

    public EvadeCollideSituation(GameUnit owner, MobSituation mobSituation) : base(owner)
    {
        _targetSituation = mobSituation;
        if (GetOwner() is MobUnit)
        {
            var mobUnit = (MobUnit) GetOwner();
            _canFlee = mobUnit.Config.CanFlee;
            FleeCd = mobUnit.Config.FleeCd;
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
        if (target)
        {
            MobUnit owner = (MobUnit) GetOwner();
            var toTarget = target.transform.position - owner.transform.position;
            toTarget.y = 0;
            var targetSqrMagnitude = toTarget.sqrMagnitude;
            // 1.生命值低于30%时无视逃跑CD强制触发一次; 2.逃跑有1~10S的内置CD，逃跑时间不多于5S，视怪物的不同而不同，技能好了就用
            if (owner.Config.CanFlee)
            {
                if (owner.CurrentHp <= owner.MaxHp * FleeThreshold && targetSqrMagnitude <= FLEE_DISTANCE_SQR)
                {
                    if (_latestState != (int) EvadeState.Flee)
                    {
                        if (Time.time - _fleeStamp >= FleeCd)
                        {
                            _latestState = (int) EvadeState.Flee;
                            _fleeStamp = Time.time;

                            return (int) EvadeState.Flee;
                        }
                    }
                    else
                    {
                        if (Time.time - _fleeStamp < FleeCd)
                        {
                            return (int) EvadeState.Flee;
                        }
                    }
                }
            }
            // 面朝玩家后退, 作为防止小怪跑到玩家头顶的机制
            float radiusSqr = (owner.Radius + target.Radius) * (owner.Radius + target.Radius);
            if (targetSqrMagnitude <= radiusSqr * 0.8f)
            {
                var evadeCollide = (int) EvadeState.EvadeCollide;
                _latestState = evadeCollide;
                return evadeCollide;
            }
        }
        if (_latestState == (int) EvadeState.EvadeCollide)
        {
            _evadeStamp = Time.time;
        }
        var normalState = (int) EvadeState.None;
        _latestState = normalState;
        return normalState;
    }

    public enum EvadeState
    {
        None,
        EvadeCollide, // 过于接近, 则后退
        Flee, // 逃跑
    }
}