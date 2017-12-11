using UnityEngine;

namespace GameDuel
{
    public class BowMinionPassive : Effect
    {
        public float Ratio = 0.15f;
        public float Duration = 5f;
        public float DamagePerSecond = 2f;

        public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target,
            GameUnit targetUnit, int skillEndureLevel,
            int comboIndex)
        {
            if (targetUnit)
            {
                targetUnit.AddEffect(new BowMinionPassiveConfig(caster, Ratio, Duration, DamagePerSecond));
            }
        }


    }
    
    public class BowMinionPassiveConfig : EffectConfig, CauseDamageFilter
    {
        private readonly float _ratio;
        private readonly float _duration;
        private readonly float _dps;

        public BowMinionPassiveConfig(GameUnit caster, float ratio, float duration, float dps) : base(caster)
        {
            _ratio = ratio;
            _duration = duration;
            _dps = dps;
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
            return new[] {EffectFilterType.CauseDamageFilter};
        }

        public float OnCauseDamage(float damage, GameUnit src, GameUnit victim, DamageType damageType,
            DamageRange damageRange,
            DamageAttribute damageAttribute, DamageSource damageSource)
        {
            if (Random.value < _ratio)
            {
                victim.AddEffect(new DamageOverTime(src, _duration, _dps));
            }
            return 0;
        }
    }

}