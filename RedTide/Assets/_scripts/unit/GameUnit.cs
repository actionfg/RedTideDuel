using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Random = System.Random;

/**
 * Base class for all GameUnits
 * 
 */

public abstract class GameUnit : MonoBehaviour
{
    // Note: This is Additional Attributes (Strength, Intelligence, etc)
    protected BasicAttribute _attributes = new BasicAttribute();
    protected BasicAttributeConfig _basicAttribute;
    protected EffectProcessor _effectProcessor;
    protected float _currentHp;
    protected float _currentMp;

    protected Vector3 LastCheckPos;    // 用于记录掉落前的安全位置
    protected CharacterController _characterController;
    public float Mass;
    public float Radius { get; private set; }

    public bool Dead { get; protected set; }

    public float CurrentHp {
        get { return _currentHp; }
        set
        {
            _currentHp = value;
            if (_currentHp > MaxHp)
            {
                _currentHp = MaxHp;
            }
            else if (_currentHp < 0)
            {
                _currentHp = 0;
            }
        }
    }

    public float CurrentMp {
        get { return _currentMp; }
        set
        {
            _currentMp = value;
            if (_currentMp > MaxMp)
            {
                _currentMp = MaxMp;
            }
            else if (_currentMp < 0)
            {
                _currentMp = 0;
            }
        }
    }

    public float GetRawAttribute(AttributeType type)
    {
        return _attributes.GetValue(type);
    }

    protected virtual void Init(BasicAttributeConfig attributeConfig)
    {
        Dead = false;

        _basicAttribute = attributeConfig;
        _attributes.Init(attributeConfig);

        _currentHp = MaxHp;
        _currentMp = MaxMp;

        _effectProcessor = new EffectProcessor(this);
    }

    private float BaseHp
    {
        get { return _basicAttribute.hp + (_attributes.GetValue(AttributeType.Vitality) * _basicAttribute.hpPerVit); }
    }

    public float MaxHp
    {
        get
        {
            return (BaseHp + _attributes.GetValue(AttributeType.HpExtra)) * _attributes.GetValue(AttributeType.HpFactor);
        }
    }

    private float BaseMp
    {
        get { return _basicAttribute.mp + (_attributes.GetValue(AttributeType.Spirit) * _basicAttribute.mpPerSpirit); }
    }

    public float MaxMp
    {
        get
        {
            return (BaseMp + _attributes.GetValue(AttributeType.MpExtra)) * _attributes.GetValue(AttributeType.MpFactor);
        }
    }

    public float Armor
    {
        get
        {
            return (
                       _basicAttribute.armor
                       + _attributes.GetValue(AttributeType.ArmorExtra)
                       + _attributes.GetValue(AttributeType.Vitality) * _basicAttribute.armorPerVit
                   )
                   * _attributes.GetValue(AttributeType.ArmorFactor);
        }
    }

    public float Resist
    {
        get
        {
            return (
                       _basicAttribute.resist
                       + _attributes.GetValue(AttributeType.ResistExtra)
                       + _attributes.GetValue(AttributeType.Spirit) * _basicAttribute.resistPerSpirit
                   ) * _attributes.GetValue(AttributeType.ResistFactor);
        }
    }

    public float MoveSpeed
    {
        get
        {
            return (_basicAttribute.moveSpeed + _attributes.GetValue(AttributeType.MoveSpeedExtra)) *
                   _attributes.GetValue(AttributeType.MoveSpeedFactor);
        }
    }

    public float AnimMoveSpeed
    {
        get
        {
            return _basicAttribute.AnimMoveSpeed;
        }
    }

//    public abstract float AttackSpeed();
    public float attackSpeed
    {
        get { return _attributes.GetValue(AttributeType.AttackSpeedFactor); }
    }

    public int AddAttribute(AttributeType type, float factor)
    {
        return AddAttribute(type, factor, false);
    }

    public int AddAttribute(AttributeType type, float factor, bool basic)
    {
        return _attributes.AddAttributeFactor(type, factor, basic);
    }

    public void RemoveAttributeFactor(AttributeType type, int handle)
    {
        _attributes.RemoveAttributeFactorByHandle(type, handle);
    }

    public float GetAttributeFactor(AttributeType type, int handle)
    {
        return _attributes.GetAttributeFactorByHandle(type, handle);
    }

    public abstract float GetAttackPower();
    public abstract float GetSpellPower();

    protected virtual void Start()
    {
        _characterController = GetComponent<CharacterController>();
        if (_characterController)
        {
            Radius = _characterController.radius;
        }
        else
        {
            var colliderComponent = GetComponent<Collider>();
            if (colliderComponent)
            {
                var boundsExtents = colliderComponent.bounds.extents;
                Radius = Mathf.Max(boundsExtents.x, boundsExtents.z);
            }
        }
    }

    public virtual void Update()
    {
        // 可能第一次Update时hp等于0
        if (CurrentHp <= 0)
        {
            //Todo
            DoDeath();
        }
        else
        {
            // 检测是否掉出屏幕
            if (_characterController && _characterController.isGrounded)
            {
                // 获取掉落后的重置位置
                LastCheckPos = transform.position;
            }
            if (!CheckOnGround())
            {
                Debug.Log(gameObject.name + " is in air, yVelocity: " + _characterController.velocity.y);
                OnFalling();
            }
            _effectProcessor.Update(this);
        }
    }

    public virtual bool CheckOnGround()
    {
        if (_characterController != null)
        {// 自由下落3s, 判定死亡
            return _characterController.velocity.y > -30f;
        }
        return true;
    }

    protected abstract void OnFalling();

    private void DoDeath()
    {
        if (_effectProcessor.FilterDeath(this))
        {
            if (OnDeath())
            {
                _effectProcessor.DoClearOnDeath();
            }
        }
    }

    public void Reborn()
    {
        this.Dead = false;
    }

    protected abstract bool OnDeath();

    public void AddEffect(EffectConfig effect)
    {
        _effectProcessor.AddEffect(effect);
    }

    public void RemoveEffect(EffectConfig.EffectContextID effectContextId)
    {
        _effectProcessor.RemoveEffect(effectContextId);
    }

    public void ProcessDamage(GameUnit srcUnit, float baseDamage, DamageType damageType, DamageRange damageRange, DamageAttribute damageAttribute, DamageSource damageSource, int hitEndureLevel)
    {
        if (CurrentHp <= 0)
        {
            return;
        }

        var filterOnAvoidInjury = _effectProcessor.FilterOnAvoidInjury(srcUnit, this, baseDamage, damageType, damageRange, damageAttribute, damageSource);
        if (filterOnAvoidInjury != AvoidInjuryType.None)
        {
            ProcessFloatingCombatPopup(GetFloatingCombatPopupType(filterOnAvoidInjury), 0, this);
            return;
        }

        float damage = srcUnit._effectProcessor.FilterCauseDamage(srcUnit, this, baseDamage, damageType, damageRange, damageAttribute, damageSource);
        damage = _effectProcessor.FilterTakingDamage(srcUnit, this, damage, damageType, damageRange, damageAttribute, damageSource);
        bool critical = isCritical(srcUnit);
        if (critical)
        {
            float criticalDamage = srcUnit._effectProcessor.FilterCauseCriticalDamage(srcUnit, this, damageType, damageAttribute,
                    damageSource);
            damage *= criticalDamage;
        }

        float targetDefence = 0;
        if (damageType == DamageType.PhysicalDamage)
        {
//            targetDefence = this.Armor;
            float penetrationFactor = srcUnit.GetRawAttribute(AttributeType.PenetrationFactor);
            targetDefence = Armor * (1 - penetrationFactor);
            targetDefence = srcUnit._effectProcessor.FilterArmorPercentChanged(srcUnit, this, targetDefence);
        }
        else if (damageType == DamageType.MagicalDamage)
        {
            targetDefence = this.Resist;
        }
        else if (damageType == DamageType.BlendDamage)
        {
            float penetrationFactor = srcUnit.GetRawAttribute(AttributeType.PenetrationFactor);
            targetDefence = Armor * (1 - penetrationFactor);
            targetDefence = srcUnit._effectProcessor.FilterArmorPercentChanged(srcUnit, this, targetDefence);
            targetDefence += this.Resist;
        }
        else if (damageType == DamageType.TrueDamage)
        {
            targetDefence = 0;
        }

        float actualDamage = Math.Max(damage - targetDefence, 0);


        actualDamage *= GlobalFactor.Instance.NpcDamage;
        this.CurrentHp -= actualDamage;
//#if UNITY_EDITOR
        StatisticManager.GetInstance().RegisterDamage(srcUnit.name, this, actualDamage);
//#endif



        ProcessFloatingCombatPopup(critical ? FloatingCombatPopupType.CriticalDamage : FloatingCombatPopupType.Damage, actualDamage, this);
        ProcessLeech(srcUnit, damageType, actualDamage);

        srcUnit._effectProcessor.FilterAfterCausedDamage(srcUnit, this, actualDamage, damageType, damageAttribute, damageSource);
        _effectProcessor.FilterAfterDamageTaken(srcUnit, this, actualDamage);

        OnHit(srcUnit, actualDamage, hitEndureLevel);

        if (CurrentHp <= 0)
        {
            srcUnit._effectProcessor.FilterOnKill(this, damageSource);
        }
    }

    public void ProcessHealing(GameUnit caster, float healing)
    {
        if (healing > 0)
        {
            bool critical = isCritical(caster);
            if (critical)
            {
                // critical healing factor is not set, default is 2
                healing *= 2f;
            }
            this.CurrentHp += healing;

            if (healing > 0)
            {
                ProcessFloatingCombatPopup(critical ? FloatingCombatPopupType.CriticalHealing : FloatingCombatPopupType.Healing, healing, this);

                //#if UNITY_EDITOR
                            StatisticManager.GetInstance().RegisterPlayHeal(healing);
                //#endif
            }
        }
    }

    private bool isCritical(GameUnit srcUnit)
    {
        if (srcUnit == null)
        {
            return false;
        }
        Random random = new Random(Guid.NewGuid().GetHashCode());
        return random.NextDouble() <= srcUnit.GetRawAttribute(AttributeType.CriticalChance);
    }

    // 处理被击动画等
    protected abstract void OnHit(GameUnit srcUnit, float actualDamage, int hitEndureLevel);

    public abstract bool isEnemy(GameUnit otherUnit);

    public bool FilterOnMove()
    {
        return _effectProcessor.FilterMoveEnable();
    }

    public bool FilterOnBroken()
    {
        return _effectProcessor.FilterBrokenEnable(this);
    }

    public bool FilterOnActiveSkill()
    {
        return _effectProcessor.FilterOnActiveSkill();
    }

    public void FilterOnBreak(GameUnit caster)
    {
        _effectProcessor.FilterOnHit(caster);
    }

    public void DoClearOnTransform()
    {
        _effectProcessor.DoClearOnTransform();
    }

    public Dictionary<AttributeType, float> GetBasicAttributes()
    {
        Dictionary<AttributeType, float> attributes = new Dictionary<AttributeType, float>();
        Array attributeTypes = Enum.GetValues(typeof(AttributeType));
        foreach (AttributeType attributeType in attributeTypes)
        {
            attributes.Add(attributeType, _attributes.GetValue(attributeType));
        }
        return attributes;
    }

    private void ProcessLeech(GameUnit srcUnit, DamageType damageType, float actualDamage)
    {
        float leechRate = 0;
        switch (damageType)
        {
            case DamageType.PhysicalDamage:
                leechRate = srcUnit._attributes.GetValue(AttributeType.PhysicalLeech);
                break;
            case DamageType.MagicalDamage:
                leechRate = srcUnit._attributes.GetValue(AttributeType.MagicLeech);
                break;
        }
        srcUnit.ProcessHealing(srcUnit, leechRate * actualDamage);
    }

//    public void HpAutoRecover()
//    {
//        CurrentHp += _attributes.GetValue(AttributeType.HpAutoRecover);
//    }

    public void CorrectHp(float vitality, float extraHp, bool add)
    {
        if (add)
        {
            CurrentHp += vitality * _basicAttribute.hpPerVit;
            CurrentHp += extraHp;
        }
        else
        {
            CurrentHp -= vitality * _basicAttribute.hpPerVit;
            CurrentHp -= extraHp;
        }
    }

    public EffectConfig GetEffect(EffectConfig.EffectContextID contextId)
    {
        return _effectProcessor.GetEffect(contextId);
    }

    private void ProcessFloatingCombatPopup(FloatingCombatPopupType popupType, float value, GameUnit target)
    {
        if (FloatingCombatPopupManager.Instance)
        {
            FloatingCombatPopupManager.Instance.ProcessFloatingCombatPopup(popupType, value, target);
        }
    }

    private FloatingCombatPopupType GetFloatingCombatPopupType(AvoidInjuryType avoidInjuryType)
    {
        switch (avoidInjuryType)
        {
            case AvoidInjuryType.Miss:
                return FloatingCombatPopupType.Miss;
            case AvoidInjuryType.Invincible:
                return FloatingCombatPopupType.Invincible;
        }
        return FloatingCombatPopupType.Miss;
    }

    public virtual void ActiveAI(GameUnit rouser)
    {
    }

    public virtual Transform GetUnitTransform()
    {
        return transform;
    }
}