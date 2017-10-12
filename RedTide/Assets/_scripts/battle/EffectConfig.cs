using System;
using UnityEngine;

[Serializable]
public abstract class EffectConfig
{
    private GameUnit _caster;
    private EffectContextID _contextId;
    private bool _removed = false;

    public int Stacks { get; private set; }
    public int MaxStacks { get; private set; }
    public EffectStackType StackType { get; private set; }
    public float Duration { get; private set; }
    public float Remaining { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Sprite Icon { get; set; }

    public enum EffectContextID
    {
        None,
        DamageOverTime,
        FrankensteinSkillDLink,
        WolfBlood,
        WolfSkillDDebuff,
        UncontrollableEffect,
        UnRegularMoveEffect,
        MoveSpeedChange,
        Snare,
        AttackSpeedUp,
        IceAndFireDot,
        HolyLightControl,
        HolyLight,
        TimeReversesPassive,
        BlindEffect,
        ResistsReduction,
        StealthEffect,        // 隐形, 实际EffectConfig待实现
        AntiStealthEffect,        // 反隐形, Mob对目标隐形后的对策
        ForceMove,
        // mob effect,
        DeathSkillA,
        DanteSkillA,

        //weapon effect
        RigidPlastic,
        FuriousFlame,
        NOTE788,
        ThunderCaller,
        SwordOfBlessing,
        SwordInTheStone,
        SwordOfTheKing,
        TwoHandSkill1Passive,
        TwoHandSkill3,
        //Helm effect
        LightningRod,
        FireCrown,
        MinerHelmet,
        GoldBand,
        BambooCopter,
        BaseballCap,
        ChristmasCap,
        //Cloack effect
        Tortoiseshell,
        VampiricTouch,
        HealingCloack,
        DrinkFromSource,
        DrinkFromSourceEffect,
        MeichangsuStyle,
        NarrowEscape,
        SnowAnger,
        SnowAngerEffect,
        //Equipment Suit
        AssassinSuitEffect,
        BatmanSuitEffect,
        RogueSuitEffect,
        //consumable effect
        Cathartic,
        DragonBlood,
        MushroomPotion,
        Poison,
        AtkSpeedPotion,
        WildPotion,
        HolyWater,
        WeakenPotion,
        PrincePotion,
        ChickenTransform,
        ElectricBass,
        Hourglass,
        DeterioratedFood,
        KhazraLeg,
        DiamondSkin,
        GlassCannon,
        DeathProtection,
        DemongargonState,
        BugbearEndure,
        AccumulateHeal,
        IgnoreCollision,
        DeathCall,            // 特性, 硬派猴子
        ShieldState,          // 护盾生命值  
    }

    public EffectConfig(GameUnit caster) : this(caster, EffectContextID.None)
    {

    }

    public EffectConfig(GameUnit caster, EffectContextID contextId)
    {
        _caster = caster;
        _contextId = contextId;

        Stacks = 1;
        MaxStacks = 1;
        StackType = EffectStackType.RefreshTime;
        Duration = 0;
        Remaining = 0;
    }

    public abstract void OnStart(GameUnit target);
    public abstract bool OnUpdate(GameUnit target);
    public abstract void OnEnd(GameUnit target);

    public void DoEndProcess(GameUnit target)
    {
        OnEnd(target);
        SetRemoved();
    }

    public virtual bool onEffectUpdate(GameUnit target)
    {
        if (IsRemoved())
        {
            return false;
        }

        bool keep = OnUpdate(target);

        if (keep)
        {
            if (Duration > 0)
            {
                Remaining -= Time.deltaTime;
                return Remaining > 0 && Stacks > 0;
            }
            else
            {
                // use 0 for infinite duration
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public int GetContextId()
    {
        return (int) _contextId;
    }

    public void SetRemoved()
    {
        _removed = true;
    }

    public bool IsRemoved()
    {
        return _removed;
    }

    // @return true to replace existing effect, false to NOT replace
    public virtual bool DoContextReplace(GameUnit owner, EffectConfig existingEffect)
    {
        switch (StackType)
        {
            case EffectStackType.None:
                return false;
            case EffectStackType.StackOnly:
                existingEffect.AddStacks(Stacks);
                break;
            case EffectStackType.RefreshTime:
                existingEffect.refreshTime(Duration);
                break;
            case EffectStackType.StackRefresh:
                existingEffect.AddStacks(Stacks);
                existingEffect.refreshTime(Duration);
                break;
            case EffectStackType.Replace:
                return true; // 替换原有Buff
                break;
        }
        return false;   // 保留原有BuffEffect
    }

    // 是否死亡时移除
    public virtual bool IsClearOnDeath()
    {
        return true;
    }

    // 是否能被其他同类技能替换
    public virtual bool IsReplaceable()
    {
        return false;
    }

    // 是否是武器被动特效
    public virtual bool IsWeaponEffect()
    {
        return false;
    }

    public virtual bool IsHelmEffect()
    {
        return false;
    }

    public virtual bool IsCloackEffect()
    {
        return false;
    }

    public virtual bool IsTransformEffect()
    {
        return false;
    }

    // 是否是套装特效
    public virtual bool IsEquipmentSuitEffect()
    {
        return false;
    }

    public GameUnit GetCaster()
    {
        return _caster;
    }

    public virtual void ApplyToTarget(GameObject target, GameUnit targetUnit)
    {
        if (targetUnit)
        {
            targetUnit.AddEffect(this);
        }
    }

    protected virtual void AddStacks(int stacks) {
        if (Stacks + stacks > MaxStacks) {
            Stacks = MaxStacks;
        }
        else {
            Stacks = Math.Max(0, Stacks + stacks);
        }
    }

    protected void refreshTime(float duration) {
        Remaining = duration;
    }

    public void SetDuration(float duration)
    {
        Duration = duration;
        Remaining = duration;
    }

    public void SetStackType(EffectStackType stackType, int maxStack = 1)
    {
        this.StackType = stackType;
        this.MaxStacks = maxStack;
    }

    public virtual int GetState()
    {
        return 0;
    }
}
