using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpAI : MobAI
{
    [Tooltip("触发逃跑距离")]
    public float FleeDistance = 6f;
    protected override void Start()
    {
        InitAI();
        _aiPath = GetComponent<MobAIPath>();
        _mobUnit = GetComponent<MobUnit>();

        if (_mobUnit)
        {
            var mobSituation = new MobSituation(_mobUnit);
            AddAISituation(mobSituation);

            var skillSituation = new ImpSkillSituation(_mobUnit, mobSituation);
            AddAISituation(skillSituation);

            var eyeContactSituation = new EyeContactSituation(_mobUnit, mobSituation); // 视线检测
            AddAISituation(eyeContactSituation);

            var stayAwaySituation = new ImpStayAwaySituation(_mobUnit, mobSituation, FleeDistance); // 触发逃跑
            AddAISituation(stayAwaySituation);

            AddAIBehavior(new StayAwayAIBehavior(_mobUnit, mobSituation, FleeDistance * 1.5f), 6, 1, new AISituationConfig()
                .addSituationCondition(stayAwaySituation, (int) ImpStayAwaySituation.EvadeState.Flee));

            var attackAiBehavior = new SimpleAttackAIBehavior(_mobUnit, mobSituation, skillSituation, eyeContactSituation);
            AddAIBehavior(attackAiBehavior, 5, 1,new AISituationConfig()
                .addSituationCondition(mobSituation, (int) MobSituation.MobState.Chasing, (int) MobSituation.MobState.Attack));

            AddAIBehavior(new SimpleStandAIBehavior(_mobUnit), 0, 1,
                new AISituationConfig().addSituationCondition(mobSituation, (int) MobSituation.MobState.Stand));
        }
    }

}