using UnityEngine;

namespace GameDuel
{
    // TODO 可能需要根据类型, 释放不同特效(如燃烧效果)
    public class DamageOverTime : EffectConfig
    {
        private readonly float _dps;

        private float _acc = 0f;
        
        public DamageOverTime(GameUnit caster, float duration, float dps) : base(caster, EffectContextID.DamageOverTime)
        {
            _dps = dps;
            SetDuration(duration);
        }

        public override void OnStart(GameUnit target)
        {
        }

        public override bool OnUpdate(GameUnit target)
        {
            _acc += Time.deltaTime;
            if (_acc >= 1f)
            {
                target.ProcessDamage(GetCaster(), _dps, DamageType.MagicalDamage, DamageRange.Ranged, DamageAttribute.Fire, DamageSource.Trigger, 0);

            }
            return true;
        }

        public override void OnEnd(GameUnit target)
        {
        }

    }
}