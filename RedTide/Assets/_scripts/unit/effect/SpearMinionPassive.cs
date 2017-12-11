using UnityEngine;

namespace GameDuel
{
    // 对骑兵造成15%额外伤害
    public class SpearMinionPassive : Effect
    {
        public float Ratio = 0.15f;

        public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target,
            GameUnit targetUnit, int skillEndureLevel,
            int comboIndex)
        {
            if (targetUnit)
            {
                targetUnit.AddEffect(new SpearMinionPassiveConfig(caster, Ratio));
            }
        }


    }

    public class SpearMinionPassiveConfig : EffectConfig, CauseDamageFilter
    {
        private readonly float _ratio;

        public SpearMinionPassiveConfig(GameUnit caster, float ratio) : base(caster)
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
            return new[] {EffectFilterType.CauseDamageFilter};
        }

        public float OnCauseDamage(float damage, GameUnit src, GameUnit victim, DamageType damageType,
            DamageRange damageRange,
            DamageAttribute damageAttribute, DamageSource damageSource)
        {
            MobUnit victimUnit = victim as MobUnit;

            if (victimUnit != null && victimUnit.Config.MobClassType == MobType.KnightMob)
            {
                return damage * _ratio;
            }
            return 0;
        }
    }
}