using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Pathfinding;
using Random = System.Random;

[CreateAssetMenu (menuName = "GameConfig/Spawn/SpawnList")]
public class SpawnList : ScriptableObject {

    public bool AllMobsUnified;

    [HideInInspector]
    public List<MobSpawnConfig> mobSpawnConfigs = new List<MobSpawnConfig>();

    private List<KeyValuePair<MobSpawnConfig, KeyValuePair<float, float>>> _weights;
    private float _totalWeight;

    private float _randomValue;

    public void OnSpawn(Transform spawnTransform, int spawnIndex, bool hardMode)
    {
        if (_weights == null)
        {
            _weights = new List<KeyValuePair<MobSpawnConfig, KeyValuePair<float, float>>>();
            _totalWeight = 0f;

            foreach (MobSpawnConfig mobSpawnConfig in mobSpawnConfigs)
            {
                if (mobSpawnConfig.CanSpawn())
                {
                    float start = _totalWeight;
                    _totalWeight += mobSpawnConfig.Weight;
                    float end = _totalWeight;

                    KeyValuePair<float, float> weightPair = new KeyValuePair<float, float>(start, end);
                    KeyValuePair<MobSpawnConfig, KeyValuePair<float, float>> configPair =
                        new KeyValuePair<MobSpawnConfig, KeyValuePair<float, float>>(mobSpawnConfig, weightPair);
                    _weights.Add(configPair);
                }
            }
        }

        Random random = new Random(Guid.NewGuid().GetHashCode());
        float randomValue = (float) (random.NextDouble() * _totalWeight);

        if (AllMobsUnified)
        {
            if (spawnIndex == 0)
            {
                _randomValue = randomValue;
            }
            randomValue = _randomValue;
        }

        foreach (KeyValuePair<MobSpawnConfig, KeyValuePair<float, float>> configPair in _weights)
        {
            MobSpawnConfig mobSpawnConfig = configPair.Key;
            float start = configPair.Value.Key;
            float end = configPair.Value.Value;
            if (randomValue >= start && randomValue < end)
            {
                if (mobSpawnConfig.MobConfig)
                {
                    DoSpawn(mobSpawnConfig.MobConfig, spawnTransform, hardMode);
                }
                if (mobSpawnConfig.SecondMobConfig)
                {
                    DoSpawn(mobSpawnConfig.SecondMobConfig, spawnTransform, hardMode);
                }
                break;
            }
        }
    }

    public static GameObject DoSpawn(DestroyableUnitConfig unitConfig, Vector3 targetLoc)
    {
        var gameObject = unitConfig.Create();
        gameObject.transform.position = targetLoc;
        return gameObject;
    }

    public static GameObject DoSpawn(MobConfig mobConfig, Vector3 targetLoc)
    {
        return DoSpawn(mobConfig, targetLoc, Quaternion.identity);
    }
    
    public static GameObject DoSpawn(MobConfig mobConfig, Vector3 targetLoc, Quaternion rot, bool hardMode = false)
    {
        
        GameObject parent = new GameObject("UnitParent " + GameContext.unitId);
        var sphereCollider = parent.AddComponent<SphereCollider>();
        sphereCollider.radius = UnitManager.ACTIVE_RANGE;
        sphereCollider.isTrigger = true;
        parent.transform.position = targetLoc;
        parent.transform.rotation = rot;
        parent.transform.SetParent(UnitManager.Instance.transform, true);

        GameObject mob = mobConfig.Create();
        mob.transform.SetParent(parent.transform);
        mob.transform.localPosition = Vector3.zero;
        mob.transform.localRotation = Quaternion.identity;
        mob.name += GameContext.unitId;
        Interlocked.Increment(ref GameContext.unitId);
        MobUnit mobUnit = mob.AddComponent<MobUnit>();
        mobUnit.Init(mobConfig, GameContext.mobLevel);
                
//        GameObject mobHpPanel = Instantiate(GameContext.UnitManager.MobHpPanel);
//        mobHpPanel.transform.SetParent(mob.transform);
//        MobHpControl mobHpControl = mobHpPanel.GetComponent<MobHpControl>();
//        mobHpControl.gameUnit = mobUnit;
//        mobHpControl.target = mob;
//        GameObject mobNamePanel = Instantiate(GameContext.DropManager.MobNamePanel);
//        mobNamePanel.transform.SetParent(mob.transform);
//        var mobNameControl = mobNamePanel.GetComponent<MobNamePopupControl>();
//        mobNameControl.GameUnit = mobUnit;
//        mobNameControl.Target = mob;
//        mobNameControl.Name = mobUnit.name;
        // 获取头顶位置
//        var hpPanelBinder = mob.GetComponentInChildren<HpPanelBinder>();
//        if (hpPanelBinder != null)
//        {
//            mobHpControl.TargetTransform = hpPanelBinder.transform;
//            mobNameControl.TargetTransform = hpPanelBinder.transform;
//        }
        
        return parent;
    }

    private GameObject DoSpawn(MobConfig mobConfig, Transform spawnTransform, bool hardMode)
    {
        var parent = DoSpawn(mobConfig, spawnTransform.position, spawnTransform.rotation, hardMode);

        if (parent)
        {
            var routeSetter = spawnTransform.GetComponent<PatrolRouteSetter>();
            if (routeSetter)
            {
                var patrolRouteProvider = new PatrolRouteProvider();
                patrolRouteProvider.RandPatrol = routeSetter.RandPatrol;
                patrolRouteProvider.Radius = routeSetter.Radius;
                patrolRouteProvider.Center = routeSetter.transform.position;
                patrolRouteProvider.PatrolRoute = routeSetter.RoutePoints;
                patrolRouteProvider.PatrolDelay = routeSetter.PatrolDelay;

                var mobControl = parent.transform.GetChild(0).GetComponent<MobControl>();
                mobControl.PatrolProvider = patrolRouteProvider;
                var audioSource = mobControl.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = mobControl.gameObject.AddComponent<AudioSource>();
                }
                audioSource.loop = false;
            }
            
        }

        return parent;
    }
}

[Serializable]
public sealed class MobSpawnConfig
{
    public MobConfig MobConfig;
    public MobConfig SecondMobConfig;
    public float Weight;

    public bool CanSpawn()
    {
        return true;
    }
}
