using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于设定作弊码
public class CheatCode : MonoBehaviour
{
    public bool Refresh = false;

    [Tooltip("正数, 则增加受到的伤害; 反之, 减少")] public float reduceDamage = 0.5f;

    // 增加伤害
    public float AttackPower = 0f;

    public float timeScale = 1f;

    [Tooltip("在角色附近刷装备")]
    public int ItemLevel = 10;
    public int DropRate = 10;
    
    // 用于调试AI
    public GameUnit aiTarget;

    public String aiBehaviorName;
    public SkillConfig currentSkill;

    private List<EffectConfig> _cheatEffects = new List<EffectConfig>();

    // Update is called once per frame
    void Update()
    {
        if (Refresh)
        {
            ClearAllCheatCode();

            StartCoroutine(ActiveCheatCode());
            Refresh = false;
        }

        var mobUnit = GetComponent<MobUnit>();
        if (mobUnit)
        {
            aiTarget = mobUnit.GetTarget();

            var mobAi = GetComponent<MobAI>();
            if (mobAi && mobAi.GetCurrentBehavior() != null)
            {
                aiBehaviorName = mobAi.GetCurrentBehavior().GetType().Name;
            }
            var mobControl = GetComponent<MobControl>();
            if (mobControl)
            {
                currentSkill = mobControl.GetCurrentSkill();
            }
        }
    }

    private IEnumerator ActiveCheatCode()
    {
        yield return new WaitForSeconds(0.1f);

        Time.timeScale = Mathf.Min(1f, timeScale);
        var currentPlayer = GetComponent<GameUnit>();
        if (currentPlayer != null)
        {
            if (Math.Abs(reduceDamage) > 0f)
            {
                var invincibleEffect = new TakenDamageFilterEffectConfig(currentPlayer, reduceDamage, 0);
                currentPlayer.AddEffect(invincibleEffect);
                _cheatEffects.Add(invincibleEffect);
            }
            if (Math.Abs(AttackPower) > 0.0001f)
            {
                currentPlayer.AddAttribute(AttributeType.AttackPower, AttackPower);
            }
        }

    }

    private void ClearAllCheatCode()
    {
        Time.timeScale = 1f;
        foreach (var effect in _cheatEffects)
        {
            effect.SetRemoved();
        }
        _cheatEffects.Clear();
    }

    private void OnDestroy()
    {
        ClearAllCheatCode();
    }
}