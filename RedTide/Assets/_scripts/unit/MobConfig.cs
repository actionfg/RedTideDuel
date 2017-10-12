using UnityEngine;
using System.Collections;
using _scripts.unit;

[CreateAssetMenu (menuName = "GameConfig/Character/MobConfig")]
public class MobConfig : CharacterConfig
{

    public float attackFactor = 1f;
    public float spellFactor = 1f;
    public float hpFactor = 1f;
    public float armorFactor = 1f;
    public float resistFactor = 1f;

    [Tooltip("怪物环绕半径")]
    public float ActiveRange = 5f;
    public float MoveSpeedDefault = 5.0f;
    [Tooltip("跑步动画的标准速度")]
    public float AnimMoveSpeed = 4.2f;
    [Tooltip("跑步声音")]
    public AudioClip LeftFootSound;
    public AudioClip RightFootSound;
    public FlyType MobType;
    public MobGroup MobGroup = MobGroup.Human;
    public MobAttackType AttackType = MobAttackType.Mix;
    public GameObject RagModel;
    [Tooltip("碎尸效果")]
    public GameObject FractureObject;
    public GameObject DeathFx;    // 死亡时播放的视觉特效
    [Tooltip("死亡时是否触发装备掉落")]
    public bool DropOnDeath = true;

    public bool CanFlee;
    public float FleeCd;
    public float FleeThreshold = 0.6f;

    public MobSkillConfig[] attackSkills;
    [Tooltip("死亡时释放点技能")]
    public MobSkillConfig deathSkill;
    // 由Ragdoll触发, 意味着可被玩家退至特定地点释放
    [Tooltip("死亡技能由Ragdoll触发")]
    public bool DeathSkillBindRd = false;
    [Tooltip("靠近角色后的发呆时间")]
    public float DullDuration = 3f;

    public BasicAttributeConfig CreateBasicAttributeConfig(int level)
    {
        BasicAttributeConfig config = CreateInstance<BasicAttributeConfig>();


        config.hp = level * 10f * hpFactor;
        config.armor = level * 0.4f * armorFactor;
        config.resist = level * 0.4f * resistFactor;
        config.moveSpeed = MoveSpeedDefault;
        // TODO: Attack
        return config;
    }
}
