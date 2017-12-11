using UnityEngine;

namespace GameDuel
{
    // 受到近战伤害,有一定几率反击
    public class BroadSwordMinionPassive : Effect
    {
        public float Ratio = 0.2f;
	
        public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, int skillEndureLevel,
            int comboIndex)
        {
            if (targetUnit)
            {
                targetUnit.AddEffect(new BroadSwordMinionPassiveConfig(caster, Ratio));
            }
        }
	
	
    }
    
    public class BroadSwordMinionPassiveConfig : EffectConfig, TakingDamageFilter
    {
        private readonly float _ratio;

        public BroadSwordMinionPassiveConfig(GameUnit caster, float ratio) : base(caster)
        {
            _ratio = ratio;
        }
	
        public override void OnStart(GameUnit target)
        {
        }

        public override bool OnUpdate(GameUnit target)
        {
            return true;
        }

        public override void OnEnd(GameUnit target)
        {
        }

        public EffectFilterType[] getFilterTypes()
        {
            return new[]{EffectFilterType.TakingDamageFilter};
        }

        public float OnTakingDamage(float damage, GameUnit src, GameUnit victim, DamageType damageType, DamageRange damageRange,
            DamageAttribute damageAttribute, DamageSource damageSource)
        {
            if (damageRange == DamageRange.Ranged)
            {
                if (Random.value < _ratio)
                {
                    // TODO 立刻反击
                    
                }
            }
            return 0;
        }

	
    }

}