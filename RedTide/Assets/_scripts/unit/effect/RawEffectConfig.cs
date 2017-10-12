using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class RawEffectConfig
{

    public string name;
	public HitTriggerType triggerType = HitTriggerType.All;
    public int triggerStage;
    public EffectObject effectObject;
}