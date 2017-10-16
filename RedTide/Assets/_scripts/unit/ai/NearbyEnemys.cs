using System.Collections.Generic;
using UnityEngine;

namespace _scripts.unit.ai
{
    // 用于收集周边敌人信息
    public class NearbyEnemys : MonoBehaviour
    {
        private HashSet<GameUnit> _enemys;
        private GameUnit _selfUnit;
        private DestroyableUnit _finalGoal;

        private void Start()
        {
            _enemys = new HashSet<GameUnit>();
            var parent = transform.parent;
            if (parent)
            {
                _selfUnit = parent.GetComponent<GameUnit>();
                if (_selfUnit)
                {
                    var bases = GameObject.FindGameObjectsWithTag("Base");
                    foreach (var baseObj in bases)
                    {
                        var destroyableUnit = baseObj.GetComponent<DestroyableUnit>();
                        if (_selfUnit.PlayerId != destroyableUnit.PlayerId)
                        {
                            _finalGoal = destroyableUnit;
                            break;
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_selfUnit == null) return;
            
            var gameUnit = other.GetComponent<GameUnit>();
            if (gameUnit && gameUnit.PlayerId != _selfUnit.PlayerId)
            {
                _enemys.Add(gameUnit);
                _selfUnit.ActiveAI(gameUnit);
                Debug.Log(name + " Add Enemy: " + gameUnit.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_selfUnit == null) return;

            var gameUnit = other.GetComponent<GameUnit>();
            if (gameUnit)
            {
                _enemys.Remove(gameUnit);
                Debug.Log(name + " Remove Enemy: " + gameUnit.name);
            }
        }

        public GameUnit GetNearestEnemy()
        {
            GameUnit target = _finalGoal;
            float minDistSqr = float.MaxValue;
            foreach (var enemy in _enemys)
            {
                if (enemy == null || enemy.Dead) 
                    continue;
                
                float distSqr = (transform.position - enemy.transform.position).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    target = enemy;
                    minDistSqr = distSqr;
                }
            }
            return target;
        }
    }
}