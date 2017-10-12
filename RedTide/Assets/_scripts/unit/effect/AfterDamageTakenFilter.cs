using UnityEngine;
using System.Collections;

public interface AfterDamageTakenFilter : UnitEffectFilter {

    void AfterDamageTaken(GameUnit src, GameUnit victim, float damage);
}
