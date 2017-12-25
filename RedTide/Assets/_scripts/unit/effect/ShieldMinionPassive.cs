using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 受到远程伤害,有一定几率降低10点伤害
public class ShieldMinionPassive : Effect
{
	public float Ratio = 0.2f;
	public float ReduceDamage = 10f;
	public GameObject TriggerEffect;

	public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, int skillEndureLevel,
		int comboIndex)
	{
		if (targetUnit)
		{
			targetUnit.AddEffect(new ShieldMinionPassiveConfig(caster, Ratio, ReduceDamage, TriggerEffect));
		}
	}
	
	
}

public class ShieldMinionPassiveConfig : EffectConfig, TakingDamageFilter
{
	private readonly float _ratio;
	private readonly float _reduceDamage;
	private readonly GameObject _triggerEffect;

	public ShieldMinionPassiveConfig(GameUnit caster, float ratio, float reduceDamage, GameObject triggerEffect) : base(caster)
	{
		_ratio = ratio;
		_reduceDamage = reduceDamage;
		_triggerEffect = triggerEffect;
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
				if (_triggerEffect)
				{
					GameObject.Instantiate(_triggerEffect, GetCaster().transform.position, Quaternion.identity);
				}
				return _reduceDamage;
			}

		}
		return 0;
	}

	
}
