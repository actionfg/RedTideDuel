using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class EffectProcessor {
    private GameUnit _owner;
    private LinkedList<EffectConfig> _effectList = new LinkedList<EffectConfig>();
    private Dictionary<int, EffectConfig> _contextEffectDictionary = new Dictionary<int, EffectConfig>();

    private Dictionary<EffectFilterType, LinkedList<EffectConfig>> _filters = new Dictionary<EffectFilterType, LinkedList<EffectConfig>>();

    private readonly Object _effectLock = new Object();

    public EffectProcessor(GameUnit owner)
    {
        this._owner = owner;
    }

    public void AddEffect(EffectConfig effectConfig)
    {
        lock (_effectLock)
        {
            DoAdd(effectConfig);
        }
    }

    public void RemoveEffect(EffectConfig.EffectContextID effectContextId)
    {
        lock (_effectLock)
        {
            if ((int) effectContextId > 0)
            {
                EffectConfig effectConfig = GetEffect(effectContextId);
                if (effectConfig != null)
                {
                    effectConfig.DoEndProcess(_owner);
                    _contextEffectDictionary.Remove((int) effectContextId);
                }
            }
        }
    }

    private void DoAdd(EffectConfig effectConfig)
    {
        if (FilterEffect(effectConfig))
        {
            return;
        }

        int contextId = effectConfig.GetContextId();
        if (contextId <= 0)
        {
            effectConfig.OnStart(_owner);
            if (effectConfig.onEffectUpdate(_owner))
            {
                _effectList.AddLast(effectConfig);
                AddToFilter(effectConfig);
            }
            else
            {
                effectConfig.DoEndProcess(_owner);
            }
        }
        else
        {

            bool replace = true;
            if (_contextEffectDictionary.ContainsKey(contextId))
            {
                EffectConfig existingEffect = _contextEffectDictionary[contextId];
                if (existingEffect != null && !existingEffect.IsRemoved()) {
                    replace = effectConfig.DoContextReplace(_owner, existingEffect);
                    if (replace) {
                        effectConfig.DoEndProcess(_owner);
                    }
                }
            }
            if (replace) {
                effectConfig.OnStart(_owner);
                if (effectConfig.onEffectUpdate(_owner)) {
                    _contextEffectDictionary.Remove(contextId);

                    _contextEffectDictionary.Add(contextId, effectConfig);

                    AddToFilter(effectConfig);
                }
                else {
                    effectConfig.DoEndProcess(_owner);
                    _contextEffectDictionary.Remove(contextId);
                }
            }
        }
    }

    private void AddToFilter(EffectConfig effectConfig)
    {
        UnitEffectFilter effectFilter = effectConfig as UnitEffectFilter;
        if (effectFilter != null)
        {
            EffectFilterType[] filterTypes = effectFilter.getFilterTypes();
            foreach (EffectFilterType filterType in filterTypes)
            {
                lock (_filters)
                {
                    LinkedList<EffectConfig> effectConfigs;
                    if (!_filters.TryGetValue(filterType, out effectConfigs))
                    {
                        effectConfigs = new LinkedList<EffectConfig>();
                        _filters.Add(filterType, effectConfigs);
                    }
                    effectConfigs.AddLast(effectConfig);

                }
            }
        }
    }

    private bool FilterEffect(EffectConfig effectConfig)
    {
        if (FilterControlEffect(effectConfig))
        {
            return true;
        }
        return false;
    }

    private bool FilterControlEffect(EffectConfig effectConfig)
    {
        bool result = false;
        ControlEffect controlEffect = effectConfig as ControlEffect;
        if (controlEffect != null)
        {
            bool isControlEffect = controlEffect.GetControlType() != ControlEffectType.None;
            if (isControlEffect)
            {
                LinkedList<EffectConfig> effectConfigs;
                if (_filters.TryGetValue(EffectFilterType.ControlledFilter, out effectConfigs))
                {
                    foreach (EffectConfig effect in effectConfigs)
                    {
                        if (effect.IsRemoved())
                        {
                            return true;
                        }
                        else
                        {
                            if (((ControlledFilter) effect).OnControlled(controlEffect))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
        }
        return result;
    }

    public void Update(GameUnit gameUnit)
    {
        lock (_effectLock)
        {
            List<EffectConfig> toRemoveList = new List<EffectConfig>();
            foreach (EffectConfig effect in _contextEffectDictionary.Values)
            {
                if (!effect.onEffectUpdate(gameUnit)) {
                    effect.DoEndProcess(gameUnit);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effectConfig in toRemoveList)
            {
                _contextEffectDictionary.Remove(effectConfig.GetContextId());
            }
            toRemoveList.Clear();

            foreach (var effect in _effectList)
            {
                if (!effect.onEffectUpdate(gameUnit)) {
                    effect.DoEndProcess(gameUnit);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effectConfig in toRemoveList)
            {
                _effectList.Remove(effectConfig);
            }
        }
    }

    public void DoClearOnDeath()
    {
        lock (_effectLock)
        {
            List<EffectConfig> toRemoveList = new List<EffectConfig>();
            foreach (var effect in _contextEffectDictionary.Values)
            {
                if (effect.IsClearOnDeath())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }

            }
            foreach (EffectConfig effectConfig in toRemoveList)
            {
                _contextEffectDictionary.Remove(effectConfig.GetContextId());
            }

            toRemoveList.Clear();
            foreach (EffectConfig effect in _effectList)
            {
                if (effect.IsClearOnDeath())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (EffectConfig effect in toRemoveList)
            {
                _effectList.Remove(effect);
            }

        }
    }

    public void DoClearControlEffects()
    {
        lock (_effectLock)
        {
            List<EffectConfig> toRemoveList = new List<EffectConfig>();
            foreach (var effect in _contextEffectDictionary.Values)
            {
                if (effect is ControlEffect)
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }

            }
            foreach (EffectConfig effectConfig in toRemoveList)
            {
                _contextEffectDictionary.Remove(effectConfig.GetContextId());
            }

            toRemoveList.Clear();
            foreach (EffectConfig effect in _effectList)
            {
                if (effect is ControlEffect)
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (EffectConfig effect in toRemoveList)
            {
                _effectList.Remove(effect);
            }
        }
    }

    public void DoClearReplaceableEffect()
    {
        lock (_effectLock)
        {
            List<EffectConfig> toRemoveList = new List<EffectConfig>();
            foreach (var effect in _contextEffectDictionary.Values)
            {
                if (effect.IsReplaceable())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (EffectConfig effectConfig in toRemoveList)
            {
                _contextEffectDictionary.Remove(effectConfig.GetContextId());
            }

            toRemoveList.Clear();
            foreach (EffectConfig effect in _effectList)
            {
                if (effect.IsReplaceable())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (EffectConfig effect in toRemoveList)
            {
                _effectList.Remove(effect);
            }
        }
    }

    public void DoClearOnChangeWeapon()
    {
        lock (_effectLock)
        {
            List<EffectConfig> toRemoveList = new List<EffectConfig>();
            foreach (var effect in _contextEffectDictionary.Values)
            {
                if (effect.IsWeaponEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (EffectConfig effectConfig in toRemoveList)
            {
                _contextEffectDictionary.Remove(effectConfig.GetContextId());
            }

            toRemoveList.Clear();
            foreach (EffectConfig effect in _effectList)
            {
                if (effect.IsWeaponEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (EffectConfig effect in toRemoveList)
            {
                _effectList.Remove(effect);
            }
        }
    }

    public void DoClearOnChangeHelm()
    {
        lock (_effectLock)
        {
            List<EffectConfig> toRemoveList = new List<EffectConfig>();
            foreach (var effect in _contextEffectDictionary.Values)
            {
                if (effect.IsHelmEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effectConfig in toRemoveList)
            {
                _contextEffectDictionary.Remove(effectConfig.GetContextId());
            }

            toRemoveList.Clear();
            foreach (var effect in _effectList)
            {
                if (effect.IsHelmEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effect in toRemoveList)
            {
                _effectList.Remove(effect);
            }
        }
    }

    public void DoClearOnChangeCloack()
    {
        lock (_effectLock)
        {
            List<EffectConfig> toRemoveList = new List<EffectConfig>();
            foreach (var effect in _contextEffectDictionary.Values)
            {
                if (effect.IsCloackEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effectConfig in toRemoveList)
            {
                _contextEffectDictionary.Remove(effectConfig.GetContextId());
            }

            toRemoveList.Clear();
            foreach (var effect in _effectList)
            {
                if (effect.IsCloackEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effect in toRemoveList)
            {
                _effectList.Remove(effect);
            }
        }
    }

    public void DoClearOnChangeEquipmentSuit()
    {
        lock (_effectLock)
        {
            List<EffectConfig> toRemoveList = new List<EffectConfig>();
            foreach (var effect in _contextEffectDictionary.Values)
            {
                if (effect.IsEquipmentSuitEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effectConfig in toRemoveList)
            {
                _contextEffectDictionary.Remove(effectConfig.GetContextId());
            }

            toRemoveList.Clear();
            foreach (var effect in _effectList)
            {
                if (effect.IsEquipmentSuitEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effect in toRemoveList)
            {
                _effectList.Remove(effect);
            }
        }
    }

    public void DoClearOnTransform()
    {
        lock (_effectLock)
        {
            List<EffectConfig> toRemoveList = new List<EffectConfig>();
            foreach (var effect in _contextEffectDictionary.Values)
            {
                if (effect.IsTransformEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effectConfig in toRemoveList)
            {
                _contextEffectDictionary.Remove(effectConfig.GetContextId());
            }

            toRemoveList.Clear();
            foreach (var effect in _effectList)
            {
                if (effect.IsTransformEffect())
                {
                    effect.DoEndProcess(_owner);
                    toRemoveList.Add(effect);
                }
            }
            foreach (var effect in toRemoveList)
            {
                _effectList.Remove(effect);
            }
        }
    }

    public float FilterCauseDamage(GameUnit src, GameUnit victim, float baseDamage, DamageType damageType, DamageRange damageRange,
        DamageAttribute damageAttribute, DamageSource damageSource) {
        float actualDmg = baseDamage;

        LinkedList<EffectConfig> list;
        lock (_filters)
        {
            var tryGetValue = _filters.TryGetValue(EffectFilterType.CauseDamageFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        actualDmg += ((CauseDamageFilter) effect).OnCauseDamage(actualDmg, src, victim, damageType, damageRange, damageAttribute, damageSource);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {// 删除已停止的
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }

            }
        }

        return Math.Max(0f, actualDmg);
    }

    public float FilterTakingDamage(GameUnit src, GameUnit victim, float damage, DamageType damageType, DamageRange damageRange,
        DamageAttribute damageAttribute, DamageSource damageSource) {
        float actualDmg = damage;
        LinkedList<EffectConfig> list;
        lock (_filters)
        {
            var tryGetValue = _filters.TryGetValue(EffectFilterType.TakingDamageFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        actualDmg += ((TakingDamageFilter) effect).OnTakingDamage(actualDmg, src, victim, damageType, damageRange, damageAttribute, damageSource);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
        return Math.Max(0f, actualDmg);
    }

    public void FilterAfterDamageTaken(GameUnit src, GameUnit victim, float actualDamage) {
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.AfterDamageTakenFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        ((AfterDamageTakenFilter) effect).AfterDamageTaken(src, victim, actualDamage);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
    }

    public void FilterAfterCausedDamage(GameUnit src, GameUnit victim, float damage, DamageType damageType, DamageAttribute damageAttribute, DamageSource damageSource)
    {
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.AfterCausedDamageFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        ((AfterCausedDamageFilter) effect).AfterCausedDamage(src, victim, damage, damageType, damageAttribute, damageSource);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
    }

    public float FilterCauseCriticalDamage(GameUnit src, GameUnit victim, DamageType damageType, DamageAttribute damageAttribute, DamageSource damageSource)
    {
        float baseCriticalDamage = src.GetRawAttribute(AttributeType.CriticalDamage);
        float actualCriticalDamage = baseCriticalDamage;

        LinkedList<EffectConfig> list;
        lock (_filters)
        {
            var tryGetValue = _filters.TryGetValue(EffectFilterType.CauseCriticalDamageFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        actualCriticalDamage += ((CauseCriticalDamageFilter) effect).OnCauseCriticalDamage(src, victim, baseCriticalDamage, damageType, damageAttribute, damageSource);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {// 删除已停止的
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }

            }
        }

        return Math.Max(0f, actualCriticalDamage);
    }

    public void FilterOnAttack(GameUnit src)
    {
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.AttackFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        ((AttackFilter) effect).OnAttack(src);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
    }

    public void FilterOnSkillCast(GameUnit src, ActiveSkillType skillType)
    {
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.SkillCastFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        ((SkillCastFilter) effect).OnSkillCast(src, skillType);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
    }      

    public float FilterArmorPercentChanged(GameUnit src, GameUnit victim, float armor)
    {
        float actualArmor = armor;
        LinkedList<EffectConfig> list;
        lock (_filters)
        {
            var tryGetValue = _filters.TryGetValue(EffectFilterType.ArmorPercentChangedFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        float armorChangePercent = ((ArmorPercentChangedFilter) effect).OnArmorPercentChanged(actualArmor, src, victim);
                        actualArmor *= (1f + armorChangePercent);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
        return Math.Max(0f, actualArmor);
    }
    
    public AvoidInjuryType FilterOnAvoidInjury(GameUnit src, GameUnit victim, float damage,
        DamageType damageType, DamageRange damageRange, DamageAttribute damageAttribute, DamageSource damageSource)
    {
        AvoidInjuryType avoidInjuryType = AvoidInjuryType.None;
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.AvoidInjuryFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        avoidInjuryType = ((AvoidInjuryFilter) effect).OnAvoidInjury(src, victim, damage, damageType, damageRange, damageAttribute, damageSource);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
        return avoidInjuryType;
    }

    public float FilterPickUpHpRecover(GameUnit unit, float healing)
    {
        float actualHealing = healing;
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.HpRecoverPickUpFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        actualHealing += ((HpRecoverPickUpFilter) effect).OnPickUpHpRecover(unit, healing);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
        return Math.Max(actualHealing, 0);
    }

    public float FilterPickUpGold(GameUnit unit, float gold)
    {
        float actualGold = gold;
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.GoldPickUpFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        actualGold += ((GoldPickUpFilter) effect).OnPickUpGold(unit, gold);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
        return actualGold;
    }

    public bool FilterDeath(GameUnit target)
    {
        bool death = true;
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.DeathFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        death = ((DeathFilter) effect).OnDeath(target);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
        return death;
    }

    public void FilterOnKill(GameUnit target, DamageSource damageSource)
    {
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.KillFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        ((KillFilter) effect).OnKill(target, damageSource);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
    }
    
    public void FilterOnHit(GameUnit caster)
    {
        lock (_filters)
        {
            LinkedList<EffectConfig> list;
            var tryGetValue = _filters.TryGetValue(EffectFilterType.OnHitFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        ((OnHitFilter) effect).OnHit(caster);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
    }

    public bool FilterMoveEnable()
    {
        bool moveEnabled = true;
        LinkedList<EffectConfig> list;
        lock (_filters)
        {
            var tryGetValue = _filters.TryGetValue(EffectFilterType.InputEnableFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        moveEnabled = ((InputEnableFilter) effect).OnMoveEnabled();
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
        return moveEnabled;
    }

    public bool FilterBrokenEnable(GameUnit victim)
    {
        bool brokenEnabled = false;
        LinkedList<EffectConfig> list;
        lock (_filters)
        {
            var tryGetValue = _filters.TryGetValue(EffectFilterType.FratureFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        brokenEnabled |= ((FractureAfterDeathFilter) effect).EnableBroken(victim);
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
        return brokenEnabled;
    }

    public bool FilterOnActiveSkill()
    {
        bool activeSkillEnabled = true;
        LinkedList<EffectConfig> list;
        lock (_filters)
        {
            var tryGetValue = _filters.TryGetValue(EffectFilterType.InputEnableFilter, out list);
            if (tryGetValue)
            {
                LinkedList<EffectConfig> toRemoveList = new LinkedList<EffectConfig>();
                foreach (EffectConfig effect in list)
                {
                    if (!effect.IsRemoved())
                    {
                        activeSkillEnabled = ((InputEnableFilter) effect).OnActiveSkill();
                    }
                    else
                    {
                        toRemoveList.AddLast(effect);
                    }
                }
                if (toRemoveList.Count > 0)
                {
                    foreach (EffectConfig effect in toRemoveList)
                    {
                        list.Remove(effect);
                    }
                }
            }
        }
        return activeSkillEnabled;
    }

    public EffectConfig GetEffect(EffectConfig.EffectContextID contextId)
    {
        if (_contextEffectDictionary.ContainsKey((int) contextId))
        {
            return _contextEffectDictionary[(int) contextId];
        }
        return null;
    }

    public int EffectCount()
    {
        return _contextEffectDictionary.Count;
    }
}

