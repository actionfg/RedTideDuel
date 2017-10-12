using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于选择下一个mob使用的技能
public class MobSkillSituation : BaseAISituation
{
    protected readonly MobSituation MobSituation;
    private static readonly float UPDATE_INTERVAL = 0.5f;

    public MobSkillSituation(GameUnit owner, MobSituation mobSituation) : base(owner)
    {
        MobSituation = mobSituation;
    }

    protected override bool shouldUpdate(float acc)
    {
        return acc >= UPDATE_INTERVAL;
    }

    protected override int updateSituation(float tpf)
    {
        var gameUnit = GetOwner();
        var mobUnit = gameUnit as MobUnit;
        if (mobUnit != null)
        {
            var skills = mobUnit.Config.attackSkills;
            if (skills != null)
            {
                var target = MobSituation.GetTarget();
                if (target)
                {
                    float distToTargetSqr =
                        Vector3.SqrMagnitude(target.transform.position - GetOwner().transform.position);
                    Dictionary<SkillConfig, int> availableSkills = mobUnit.GetAvailableSkills();
                    if (availableSkills.Count > 0)
                    {
                        List<int> priorityCandidates = new List<int>();
                        List<int> candidates = new List<int>();
                        // 考虑技能最小距离
                        foreach (KeyValuePair<SkillConfig, int> skillPair in availableSkills)
                        {
                            if (skillPair.Key is MobSkillConfig)
                            {
                                MobSkillConfig skillConfig = (MobSkillConfig) skillPair.Key;
                                if (skillConfig.MinimumAttackRange > 0.001f)
                                {
                                    if (distToTargetSqr >=
                                        (skillConfig.MinimumAttackRange + target.Radius + gameUnit.Radius) *
                                        (skillConfig.MinimumAttackRange + target.Radius + gameUnit.Radius))
                                    {
                                    priorityCandidates.Add(skillPair.Value);
                                    }
                                }
                                else
                                {
                                    candidates.Add(skillPair.Value);
                                }
                            }
                        }
                        if (priorityCandidates.Count > 0f)
                        {
                            // 提高有最小攻击距离限制的技能优先级
                            return priorityCandidates[Random.Range(0, priorityCandidates.Count)];
                        }
                        if (candidates.Count > 0)
                        {
                            return candidates[Random.Range(0, candidates.Count)];
                        }
                    }
                }
            }
        }
        return -1;
    }
}