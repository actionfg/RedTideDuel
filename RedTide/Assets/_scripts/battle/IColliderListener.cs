using UnityEngine;
using System.Collections;

public interface IColliderListener
{

    void OnColliderTrigger(HitTriggerType attackTriggerType, Collider attackTrigger, Collider victim, Vector3 contactVelocity, Vector3 contactPoint, GameObject hitFx, GameObject nonUnitHitFx);
}
