using UnityEngine;

namespace GameDuel
{
    // 自身第一次伤害附带昏迷和额外伤害
    public class KnightMinionPassive : Effect
    {
        public float StunDuration = 5f;
        public float Damage = 2f;

        public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target,
            GameUnit targetUnit, int skillEndureLevel,
            int comboIndex)
        {
            if (targetUnit)
            {
                targetUnit.AddEffect(new KnightMinionPassiveConfig(caster, StunDuration, Damage));
            }
        }


    }
    
    public class KnightMinionPassiveConfig : EffectConfig, CauseDamageFilter
    {
        private readonly float _stunDuration;
        private readonly float _damage;

        private bool _activated = false;
        
        public KnightMinionPassiveConfig(GameUnit caster, float stunDuration, float damage) : base(caster)
        {
            _stunDuration = stunDuration;
            _damage = damage;
        }

        public override void OnStart(GameUnit target)
        {
        }

        public override bool OnUpdate(GameUnit target)
        {
            return !_activated;
        }

        public override void OnEnd(GameUnit target)
        {
        }

        public EffectFilterType[] getFilterTypes()
        {
            return new[] {EffectFilterType.CauseDamageFilter};
        }

        public float OnCauseDamage(float damage, GameUnit src, GameUnit victim, DamageType damageType,
            DamageRange damageRange,
            DamageAttribute damageAttribute, DamageSource damageSource)
        {
            
            victim.AddEffect(new UncontrollableEffectConfig(src, victim, UncontrollableType.Stun, _stunDuration));
            victim.ProcessDamage(GetCaster(), _damage, DamageType.PhysicalDamage, DamageRange.Melee, DamageAttribute.Normal, DamageSource.Trigger, 0);
            _activated = true;
            return 0;
        }
    }

}