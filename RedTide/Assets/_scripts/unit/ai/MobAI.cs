using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class MobAI : AI
{
    protected MobUnit _mobUnit;
    private SimpleAttackAIBehavior _attackAiBehavior;

    protected override void Start()
    {
        base.Start();

        _mobUnit = GetComponent<MobUnit>();

        if (_mobUnit)
        {

            var mobSituation = new MobSituation(_mobUnit);
            AddAISituation(mobSituation);

            var skillSituation = new MobSkillSituation(_mobUnit, mobSituation);        // 攻击技能选择逻辑
            AddAISituation(skillSituation);

            var evadeCollideSituation = new EvadeCollideSituation(_mobUnit, mobSituation);    // 距离保持和逃跑
            AddAISituation(evadeCollideSituation);

            if (_mobUnit.Config.CanFlee)
            {// 逃跑
                AddAIBehavior(new FleeAIBehavior(_mobUnit, mobSituation), 8, 1, new AISituationConfig()
                    .addSituationCondition(evadeCollideSituation, (int)EvadeCollideSituation.EvadeState.Flee));
            }

            // 防止怪物挤到玩家头顶(完美解决方案, 还得注意查看怪物攻击距离是否过近)
            AddAIBehavior(new EvadeCollideBehaior(_mobUnit, mobSituation), 6, 1, new AISituationConfig()
                .addSituationCondition(evadeCollideSituation, (int)EvadeCollideSituation.EvadeState.EvadeCollide));

            // 攻击逻辑
            {// 杂鱼怪攻击
                _attackAiBehavior = new SimpleAttackAIBehavior(_mobUnit, mobSituation, skillSituation);
                AddAIBehavior(_attackAiBehavior, 5, 1, new AISituationConfig()
                    .addSituationCondition(mobSituation, (int)MobSituation.MobState.Chasing, (int)MobSituation.MobState.Attack));

            }

            // 目标隐形后的攻击行为
//            AddAIBehavior(new SearchAttackAIBehavior(_mobUnit, mobSituation, skillSituation), 5, 1,
//                new AISituationConfig().addSituationCondition(mobSituation, (int) MobSituation.MobState.SearchAttack));


            // 强制攻击
            AddAIBehavior(_attackAiBehavior, 9, 1, new AISituationConfig()
                .addSituationCondition(mobSituation, (int)MobSituation.MobState.ForceAttack));

            // 站立不动
            var standAiBehavior = new SimpleStandAIBehavior(_mobUnit);
            AddAIBehavior(standAiBehavior, 0, 1, new AISituationConfig()
                .addSituationCondition(mobSituation, (int)MobSituation.MobState.Stand));
        }
    }

    public AISituation GetSituation(Type situationType)
    {
        if (GetActiveSituations() != null)
        {
            foreach (var situation in GetActiveSituations())
            {
                if (situation.GetType() == situationType)
                {
                    return situation;
                }
            }
        }
        return null;
    }

    public GameUnit GetTarget()
    {
        var mobSituation = GetSituation(typeof(MobSituation));
        if (mobSituation != null)
        {
            return ((MobSituation) mobSituation).GetTarget();
        }
        return null;
    }
}
