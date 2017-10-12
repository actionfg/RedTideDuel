using UnityEngine;
using System.Collections;

// 选择合适时机Destroy gameObject
public interface RemovalControl
{
    // 由EffectObject碰撞到其他物品后通知
    void notifyCollideWith(GameUnit caster, GameObject victim);
    void SetCaster(GameUnit caster);
}