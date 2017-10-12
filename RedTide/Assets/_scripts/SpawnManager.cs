using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public float SpawnInterval = 10f;

	private Dictionary<MobConfig, int> _tideGroup;
	private float _acc = 0f;
	
	// Use this for initialization
	void Start ()
	{
		_acc = SpawnInterval;
	}
	
	// Update is called once per frame
	void Update () {
		if (_acc >= SpawnInterval)
		{
			_acc -= SpawnInterval;

			DoSpawn();
		}
	}

	private void DoSpawn()
	{
		
	}
}
