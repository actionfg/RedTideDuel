using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public DestroyableUnitConfig UnitConfig;
    public float Interval;

    private float _acc = 0;
    public bool _enable = false;

    private void Awake()
    {

    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (_enable)
	    {
            _acc += Time.deltaTime;
            if (_acc >= Interval)
            {
                var newObj = UnitConfig.Create();
                newObj.transform.position = transform.position;
                newObj.transform.rotation = transform.rotation;
                var gameUnit = newObj.AddComponent<DestroyableUnit>();
                gameUnit.Init(UnitConfig);
                var effectObject = newObj.AddComponent<EffectObject>();
                effectObject.OnTrigger(null, null, gameUnit);

                UnitManager.Instance.AddDestroyableUnit(gameUnit);
                var rigid = newObj.GetComponent<Rigidbody>();
                if (rigid)
                {
                    rigid.velocity = transform.forward * Random.value * 20f;
                }
                _acc -= Interval;
            }
	    }
	}

}
