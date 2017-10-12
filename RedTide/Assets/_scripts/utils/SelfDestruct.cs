using UnityEngine;
using System.Collections;


public class SelfDestruct : MonoBehaviour, RemovalControl {
	public float selfdestruct_in = 4;

	void Start () {
		var poolObjectMark = GetComponent<PoolObjectMark>();
		if (poolObjectMark)
		{
			poolObjectMark.Unspawn();
		}
		else
		{
			Destroy (gameObject, selfdestruct_in);
		}
	}

    public void notifyCollideWith(GameUnit caster, GameObject victim)
    {

    }

    public void SetCaster(GameUnit caster)
    {

    }
}
