using UnityEngine;

// 用于清除尸体
public class BodyExplodeEffect : MonoBehaviour
{
	private int _maxHitTimes;
	private int _hitTimes;

	private bool _haveTriggered = false;
	// Use this for initialization
	void Start ()
	{
		var ragdollCollider = gameObject.AddComponent<SphereCollider>();
		ragdollCollider.center = Vector3.zero;
		ragdollCollider.radius = 1;
		ragdollCollider.isTrigger = true;

		_maxHitTimes = 2;
	}

	void OnTriggerEnter(Collider caster)
	{
		if (_haveTriggered)
		{
			return;
		}

//		var playerUnit = caster.gameObject.GetComponent<PlayerUnit>();
//		if (!playerUnit)
//		{
//			var colliderTrigger = caster.gameObject.GetComponentInChildren<ColliderTrigger>();
//			if (colliderTrigger)
//			{
//				_hitTimes++;
//				if (_hitTimes >= _maxHitTimes)
//				{
//					if (ObjectPoolManager.Instance && ObjectPoolManager.Instance.HasPooled(GameContext.DropManager.BodyExplodeEffect))
//					{
//						ObjectPoolManager.Instance.Spawn(GameContext.DropManager.BodyExplodeEffect, transform.position, transform.rotation);
//						_haveTriggered = true;
//					}
//					Destroy(gameObject);
//				}
//			}
//		}
	}
}
