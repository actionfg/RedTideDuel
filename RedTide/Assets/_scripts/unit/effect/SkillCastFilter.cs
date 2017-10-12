using UnityEngine;

public interface SkillCastFilter : UnitEffectFilter {
    void OnSkillCast(GameUnit src, ActiveSkillType skillType);
}
