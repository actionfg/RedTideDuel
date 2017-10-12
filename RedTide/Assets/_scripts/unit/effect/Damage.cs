using System;
using UnityEngine;
using System.Collections;

public class Damage : Effect
{
    public float damageFactor;
    public DamageType damageType;
    public DamageRange damageRange;
    public DamageAttribute damageAttribute;
    public DamageSource damageSource;

    public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, int skillEndureLevel, int comboIndex)
	{
		if (targetUnit)
		{
		    targetUnit.AddEffect(new DamageConfig(caster, skillEndureLevel, damageFactor, damageType, damageRange, damageAttribute, damageSource, comboIndex));
		}
	}
}

public class DamageConfig : EffectConfig
{
    private readonly int _skillEndureLevel;
    private readonly DamageType _damageType;
    private readonly float _damageFactor;
    private readonly DamageRange _damageRange;
    private readonly DamageAttribute _damageAttribute;
    private readonly DamageSource _damageSource;
    private readonly int _comboIndex;

    public DamageConfig(GameUnit caster, int skillEndureLevel, float damageFactor, DamageType damageType = DamageType.PhysicalDamage, DamageRange damageRange = DamageRange.Melee, DamageAttribute damageAttribute = DamageAttribute.Normal, 
        DamageSource damageSource = DamageSource.Attack, int comboIndex = 0) : base(caster)
    {
        _skillEndureLevel = skillEndureLevel;
        _damageType = damageType;
        _damageFactor = damageFactor;
        _damageRange = damageRange;
        _damageAttribute = damageAttribute;
        _damageSource = damageSource;
        _comboIndex = comboIndex;
    }

    public override void OnStart(GameUnit target)
    {

    }

    public override bool OnUpdate(GameUnit target)
    {
        if (target)
        {
            float casterDamage;
            switch (_damageType)
            {
                case DamageType.PhysicalDamage:
                    casterDamage = GetCaster().GetAttackPower ();
                    break;
                case DamageType.MagicalDamage:
                    casterDamage = GetCaster().GetSpellPower();
                    break;
                case DamageType.BlendDamage:
                    casterDamage = GetCaster().GetAttackPower() + GetCaster().GetSpellPower();
                    break;
                default:
                    casterDamage = GetCaster().GetAttackPower ();
                    break;
            }

            float rawDamage = casterDamage * _damageFactor * ( 1f + GlobalFactor.Instance.ComboFactor * _comboIndex);
            target.ProcessDamage(GetCaster(), rawDamage, _damageType, _damageRange, _damageAttribute, _damageSource, _skillEndureLevel);
        }
        return false;
    }

    public override void OnEnd(GameUnit target)
    {
    }
}
