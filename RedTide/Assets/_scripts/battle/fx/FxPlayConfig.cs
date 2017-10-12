using UnityEngine;
using System.Collections;

public class FxPlayConfig : Effect {

    public GameObject fxPrefab;
    public FxPlayPosition fxPlayPosition;
    public FxBindingMode fxBindingMode;
    [Header("Use 0 for infinite duration")]
    public float duration;
    public bool RetainOnLoad = false;

    [Tooltip("与buff绑定, buff结束则特效关闭")]
    public EffectConfig.EffectContextID BindingEffectId = EffectConfig.EffectContextID.None;
    [Tooltip("检测buff状态, 不匹配则关闭")]
    public int BindingEffectState = 0;
    public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, int skillEndureLevel, int comboIndex)
    {
        InitPlayControl(fxPrefab, fxPlayPosition, fxBindingMode, duration, effectObject, targetUnit, RetainOnLoad, BindingEffectId, BindingEffectState);
    }

    public static void InitPlayControl(GameObject fxPrefab, FxPlayPosition fxPlayPosition, FxBindingMode fxBindingMode, float duration, 
        EffectObject effectObject, GameUnit targetUnit, bool retainOnLoad, EffectConfig.EffectContextID bindingEffectID = EffectConfig.EffectContextID.None,
        int bindingEffectState = 0)
    {
        GameObject fx = Instantiate(fxPrefab, targetUnit.transform.position, Quaternion.identity);
        FxPlayControl playControl = fx.AddComponent<FxPlayControl>();
        playControl.TargetUnit = targetUnit;
        playControl.FxPlayPosition = fxPlayPosition;
        playControl.FxBindingMode = fxBindingMode;
        playControl.Duration = duration;
        playControl.SetBindingEffectInfo(bindingEffectID, bindingEffectState);
        if (fxBindingMode == FxBindingMode.InitUnitPosition)
        {
            fx.transform.position = targetUnit.transform.position;
            fx.transform.rotation = targetUnit.transform.rotation;
        }
        else if (fxBindingMode != FxBindingMode.None)
        {
//            fx.transform.SetParent(targetUnit.transform);
            fx.transform.position = effectObject.transform.position;
            fx.transform.rotation = effectObject.transform.rotation;
        }
        else
        {
            fx.transform.position = effectObject.transform.position;
            fx.transform.rotation = effectObject.transform.rotation;
        }

        if (retainOnLoad)
        {
            fx.AddComponent<RetainOnLoad>();
        }
    }
}
