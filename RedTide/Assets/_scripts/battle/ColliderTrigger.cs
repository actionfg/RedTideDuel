using System;
using UnityEngine;
using System.Collections;

public class ColliderTrigger : MonoBehaviour {

    IColliderListener control;
    Collider attackTrigger;
    Vector3 contactVelocity;
    Vector3 lastPos;

    public GameObject unitHitFx = null;
    public GameObject nonUnitHitFx = null;

    [Header("Trigger Type has no effect on Weapons")]
    public HitTriggerType type;

    // Use this for initialization
    void Start()
    {
        attackTrigger = GetComponent<Collider>();
        lastPos = transform.position;
    }

    void Update()
    {
        contactVelocity = (transform.position - lastPos) / Time.deltaTime;
        lastPos = transform.position;
    }


    void OnTriggerEnter(Collider victim)
    {
        CollideWith(victim);
    }

    private void OnCollisionEnter(Collision other)
    {
        CollideWith(other.collider);
    }

    private void CollideWith(Collider victim)
    {
        if (victim == null || victim.isTrigger)
        {// 过滤trigger类型的碰撞体
            return;
        }

        if (control == null)
        {
            var mobControl = GetComponentInParent<MobControl>();
             if (mobControl)
            {
                control = mobControl;
            }
        }

        if (control != null)
        {
            var closestPointOnBounds = victim.ClosestPointOnBounds(transform.position);
            control.OnColliderTrigger(type, attackTrigger, victim, contactVelocity, closestPointOnBounds, unitHitFx, nonUnitHitFx);
        }
    }
}
