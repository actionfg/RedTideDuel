using UnityEngine;

public class FxPlayControl : MonoBehaviour {

    public GameUnit TargetUnit { get; set; }
    public FxPlayPosition FxPlayPosition { get; set; }
    public FxBindingMode FxBindingMode { get; set; }
    public float Duration { get; set; }

    private float acc;
    private bool active;
    private FxBinder binder;
	
	// 当TargetUnit身上的BindingEffect被移除或者state不等于_bindingEffectState; 
	// 则删除该视觉特效
	private EffectConfig.EffectContextID _bindingEffectId = EffectConfig.EffectContextID.None;
	private int _bindingEffectState = 0;
	private EffectConfig _bindingEffect;

	// Use this for initialization
	void Start ()
	{
	    acc = 0;
	    active = true;

	    binder = GetBinder();
	    if (FxBindingMode != FxBindingMode.None)
	    {
	        gameObject.transform.position = GetPlayPosition();
	    }
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (TargetUnit)
	    {
	        switch (FxBindingMode)
	        {
	            case FxBindingMode.None:
	                break;
	            case FxBindingMode.BindPosition:
	                gameObject.transform.position = GetPlayPosition();
	                DestroyOnDead();
	                break;
                case FxBindingMode.BindRotation:
	                gameObject.transform.rotation = TargetUnit.transform.rotation;
	                DestroyOnDead();
	                break;
                case FxBindingMode.BindPosAndRot:
	                gameObject.transform.position = GetPlayPosition();
	                gameObject.transform.rotation = TargetUnit.transform.rotation;
	                DestroyOnDead();
	                break;
	        }
		    if (_bindingEffectId != EffectConfig.EffectContextID.None && _bindingEffect == null)
		    {
			    _bindingEffect = TargetUnit.GetEffect(_bindingEffectId);
		    }

		    if (_bindingEffect != null)
		    {
			    if (_bindingEffect.IsRemoved() || _bindingEffect.GetState() != _bindingEffectState)
			    {
					Destroy(gameObject);    
			    }
		    }
	    }
	    else if(FxBindingMode != FxBindingMode.None && FxBindingMode != FxBindingMode.InitUnitPosition)
	    {
	        Destroy(gameObject);
	    }

	    if (Duration > 0)
	    {
	        acc += Time.deltaTime;
	    }
	    else
	    {
	        acc = -1;
	    }

	    if (active && acc >= Duration)
	    {
	        active = false;
	    }
		if (!active)
		{
	        Destroy(gameObject);
		}
	}

    private FxBinder GetBinder()
    {
        if (FxBindingMode != FxBindingMode.None && TargetUnit != null)
        {
            FxBinder[] binders = TargetUnit.gameObject.GetComponentsInChildren<FxBinder>();
            switch (FxPlayPosition)
            {
                case FxPlayPosition.LogicalPosition:
                    return null;
                case FxPlayPosition.Center:
                case FxPlayPosition.Head:
                case FxPlayPosition.LeftHand:
                case FxPlayPosition.RightHand:
                case FxPlayPosition.Other:
                    foreach (FxBinder binder in binders)
                    {
                        if (binder.bindingPosition == FxPlayPosition)
                        {
                            return binder;
                        }
                    }
                    break;
            }
        }
        return null;
    }

    private Vector3 GetPlayPosition()
    {
        Vector3 position = TargetUnit.transform.position;
        if (binder != null)
        {
            switch (binder.bindingPosition)
            {
                case FxPlayPosition.Head:
                    position = binder.transform.position + Vector3.up;
                    break;
                case FxPlayPosition.Center:
                case FxPlayPosition.LeftHand:
                case FxPlayPosition.RightHand:
                    position = binder.transform.position;
                    break;
            }
        }
        return position;
    }

    public bool checkDeaded()
    {
        return TargetUnit.Dead;
    }

    private void DestroyOnDead()
    {
        if (TargetUnit.Dead)
        {
            acc = Duration;
        }
    }

	public void SetDeactive()
	{
		active = false;
	}

	public void SetBindingEffectInfo(EffectConfig.EffectContextID bindingEffect, int bindingEffectState)
	{
		_bindingEffectId = bindingEffect;
		_bindingEffectState = bindingEffectState;
	}
}
