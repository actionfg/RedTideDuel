using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "GameConfig/Character/BasicAttributeConfig")]
public class BasicAttributeConfig : ScriptableObject {
    public float hp;
    public float mp;
    public float moveSpeed;
    public float AnimMoveSpeed;
    public float armor;
    public float resist;

    public float hpPerVit = 1f;
    public float mpPerSpirit = 1f;
    public float armorPerVit = 1f;
    public float resistPerSpirit = 1f;
    public float apPerStr = 1f;
    public float spPerInt = 1f;
}
