using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于实现Boss或场景投出的一些可破坏石块, 或滚石等召唤物
public class DestroyableUnit : GameUnit, RemovalControl {

    public void Init(DestroyableUnitConfig unitConfig)
    {
        Init(unitConfig.CreateBasicAttributeConfig());
    }

    public override float GetAttackPower()
    {
        // 实际伤害由后续的EffectObject决定
        return 1;
    }

    public override float GetSpellPower()
    {
        return 1;
    }

    protected override bool OnDeath()
    {
        // 2秒后删除
        Destroy(gameObject, 0.5f);
        return true;
    }

    public void notifyCollideWith(GameUnit caster, GameObject victim)
    {
        // do nothing
    }

    public void SetCaster(GameUnit caster)
    {

    }

    public override bool isEnemy(GameUnit otherUnit)
    {
        return otherUnit != this;
    }

    protected override void OnHit(GameUnit srcUnit, float actualDamage, int hitEndureLevel)
    {
        // do nothing
    }

    protected override void OnFalling()
    {
        this.CurrentHp = 0;
    }
}
