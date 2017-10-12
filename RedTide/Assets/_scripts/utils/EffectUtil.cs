using System;
using UnityEngine;
using Random = System.Random;

public class EffectUtil
{
    public static EffectObject createEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation,
        GameUnit caster,
        GameObject target, GameUnit targetUnit, int skillEndureLevel, int comboIndex = 0)
    {
        return createEffect(effectPrefab, position, rotation, caster, target, targetUnit, 0, skillEndureLevel,
            comboIndex);
    }

    public static EffectObject createEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation,
        GameUnit caster, GameObject target, GameUnit targetUnit, int index, int skillEndureLevel, int comboIndex = 0)
    {
        GameObject instantiate;
        if (ObjectPoolManager.Instance && ObjectPoolManager.Instance.HasPooled(effectPrefab))
        {
            instantiate = ObjectPoolManager.Instance.Spawn(effectPrefab, position, rotation);
        }
        else
        {
            instantiate = GameObject.Instantiate(effectPrefab, position, rotation) as GameObject;
        }

        if (instantiate)
        {
            EffectObject effect = instantiate.GetComponent<EffectObject>();
            effect.Index = index;
            effect.caster = caster;
            effect.SetSkillEndureLevel(skillEndureLevel);
            effect.SetComboIndex(comboIndex);
            effect.OnTrigger(target, targetUnit);

            return effect;
        }

        return null;
    }

    public static bool IsEffect(float rate)
    {
        Random random = new Random(Guid.NewGuid().GetHashCode());
        return random.NextDouble() * 100 <= rate;
    }

    public static GameUnit SearchNearestUnit(Vector3 castLocation, GameUnit caster, float radius, bool includeFriendly,
        bool includeEnemy, bool includeSelf)
    {
        GameUnit closestUnit = null;
        float closestDis = float.MaxValue;
        Vector3 origin = castLocation;
        Collider[] colliders = Physics.OverlapSphere(origin, radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            GameObject target = collider.gameObject;
            GameUnit targetUnit = target.GetComponent<GameUnit>();
            if (targetUnit && !targetUnit.Dead)
            {
                if (!includeEnemy && (caster.isEnemy(targetUnit)))
                {
                    continue;
                }
                if (!includeFriendly && (!caster.isEnemy(targetUnit)) && caster != targetUnit)
                {
                    continue;
                }
                if (!includeSelf && (caster == targetUnit))
                {
                    continue;
                }

                float distance = Vector3.Distance(origin, targetUnit.transform.position);
                if (distance <= closestDis)
                {
                    closestDis = distance;
                    closestUnit = targetUnit;
                }
            }
        }
        return closestUnit;
    }

    public static Vector3 GetFloorHitPosition()
    {
        int floorHitPlaneMask = LayerMask.GetMask("FloorHitPlane") | LayerMask.GetMask("Floor");
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;
        if (Physics.Raycast(camRay, out floorHit, 200f, floorHitPlaneMask))
        {
            return floorHit.point;
        }
        return Vector3.zero;
    }
}