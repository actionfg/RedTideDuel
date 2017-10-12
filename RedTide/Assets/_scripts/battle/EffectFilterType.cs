using UnityEngine;
using System.Collections;

// ReSharper disable once CheckNamespace
public enum EffectFilterType {

    /**
     * After a damage is taken, used as a notification
     */
    AfterDamageTakenFilter,

    /**
     * After caused damage, used as a notification
     */
    AfterCausedDamageFilter,

    /**
     * Used to adjust base damage dealt
     */
    CauseDamageFilter,

    /**
     * Used to adjust base impulse dealt
     */
    CauseImpulseFilter,

    /**
     * Used to adjust damage taken, after CauseDamageFilter
     */
    TakingDamageFilter,

    /**
     * Used to adjust critical damage dealt
     */
    CauseCriticalDamageFilter,

    /**
     * Used for Attack Precondition
     */
    AttackPreConditionFilter,

    /**
     *  On Attack, no matter hit or not, Used as Attack Notification
     */
    AttackFilter,

    /**
     * Skill cast notification
     */
    SkillCastFilter,

    /**
     * Used to adjust skill cost
     */
    CostFilter,

    /**
     * Used to adjust skill cooldown
     */
    CooldownFilter,

    /**
     * Used for Change Armor
     */
    ArmorPercentChangedFilter,

    /**
     * Used for Input enable filter
     */
    InputEnableFilter,

    /**
     * Death Filter
     */
    DeathFilter,

    /**
     * Pick up soul notification
     */
    SoulPickUpFilter,

    /**
     * Used to adjust Pick up hp recover item healing
     */
    HpRecoverPickUpFilter,
 
    /**
     * Used to adjust Pick up mp recover item healing
     */
    MpRecoverPickUpFilter, 
 
    /**
     * Used to adjust Pick up Gold item healing
     */
    GoldPickUpFilter,

    /**
     * Kill notification
     */
    KillFilter,

    /**
     * Used for adjust Trap trigger condition
     */
    TrapTriggerFilter,

    /**
     * Used for modify equipment
     */
    EquipmentFilter,

    /**
     * Control effect filter
     */
    ControlledFilter,

    /**
     * Used for avoid all damage
     */
    AvoidInjuryFilter,

    /**
     * caster avoid cause damage
     */
    BlindFilter,

    /**
     * Used for avoid be attacked
     */
    FreeFromAttackedFilter,

    /**
     * Can not attack filter
     */
    DisarmFilter,

    // Change unit EndureLevel
    EndureFilter, 
 
    /**
     * notify on hit
     */
    OnHitFilter,
 
    // 通知进入下一关
    ChapterFilter,
 
    // 检测死亡后是否碎裂
    FratureFilter,
}
