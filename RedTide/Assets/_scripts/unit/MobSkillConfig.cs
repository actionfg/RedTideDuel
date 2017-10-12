using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "GameConfig/Skill/MobSkill")]
public class MobSkillConfig : SkillConfig
{
    public float MinimumAttackRange = 0f;   // 大于此距离才能使用该技能
    public float AttackRange = 3f;
    public float InitialCD = 0f;            // 初始CD, AI激活后开始计时
}
