using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "GameConfig/Character/DestroyableUnitConfig")]
public class DestroyableUnitConfig : CharacterConfig
{
    public float hp;

    public BasicAttributeConfig CreateBasicAttributeConfig()
    {
        var basicAttributeConfig = CreateInstance<BasicAttributeConfig>();
        basicAttributeConfig.hp = hp;

        return basicAttributeConfig;
    }

    public override GameObject Create()
    {
        var gameObject = base.Create();
        var destroyableUnit = gameObject.AddComponent<DestroyableUnit>();
        destroyableUnit.Init(this);
        return gameObject;
    }
}
