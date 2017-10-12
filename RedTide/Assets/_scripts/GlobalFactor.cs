using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFactor : MonoBehaviour
{

    public float GoldDrop = 1f;
    public float MerchantCost = 1f;
    public float TalentCost = 1f;
    public float SoulDrop = 1f;
    public float PlayerDamage = 1f;
    public float NpcDamage = 1f;
    public float ManaCost = 1f;
    public float PlayerSpeed = 1f;
    public float NpcSpeed = 1f;
    public float ComboFactor = 0.2f;        // 连招伤害修正
    public float DropFactor4Minion = 0.5f;    // 修改Minion 掉落概率
    public float DropFactor4Leader = 2f;  // 随机boss和宝箱 rare级别装备掉落概率提高一倍
    public float DropFactor4Boss = 2f;    // boss epic掉落概率提高一倍
    public float MerchantPriceFactor = 10f;    // 商品价格系数

    private static GlobalFactor s_instance;

    public static GlobalFactor Instance
    {
        get
        {
            if (s_instance == null)
            {
                var o = new GameObject("GlobalFactor");
                o.AddComponent<RetainOnLoad>();
                return o.AddComponent<GlobalFactor>();
            }
            else
            {
                return s_instance;
            }
        }
    }

    private void Awake() {
        // Only one instance of GlobalFactor at a time!
        if (s_instance != null) {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
    }

}
