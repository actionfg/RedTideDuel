using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public abstract class SkillConfig : ScriptableObject
{
    public string animTrigger;
    [Tooltip("释放技能过程中能否移动")]
    public bool CanMove = false;    
    [Tooltip("前摇过程中是否能改变朝向")]
    public bool CanRotate = false; 
    
    [Tooltip("不为0, 且无动作,则表明技能持续时长")]
    public float animTime;
    public float MinCooldown;
    public float MaxCooldown;
    [Tooltip("技能硬直等级")]
    public int endureLevel = 0;
    public GameObject AdditionCollider;

    public EffectObject passive;
    public GameObject CastSound;
    public GameObject CastEffect;
    public FxPlayPosition PlayPosition;
	public FxBindingMode BindingMode;
	public Vector3 BindingOffset;                // x相对于caster的右边偏移距离, z则为caster身前偏移距离
    [Tooltip("是否在技能被打断时关闭CastEffect")]
    public bool CastEffectDeactiveOnEnd = true;
	public bool CastBindRot = false;
    public CastType CastType = CastType.Self;
    public PrecastType PrecastType = PrecastType.None;
    public float Range;
    public Sprite Icon;
    [TextArea(3,10)]
    public string Description;

    [HideInInspector] [SerializeField] public List<RawEffectConfig> rawEffects = new List<RawEffectConfig>();

    public void Init(GameUnit owner)
    {
        if (passive)
        {
            EffectUtil.createEffect(passive.gameObject, owner.transform.position, owner.transform.rotation, owner,
                owner.gameObject, owner, endureLevel);
        }
    }

    // 技能释放时触发
    public void OnActiveSkill(GameUnit caster, SkillCastParameter castParameter)
    {
        if (castParameter.Valid)
        {
            OnTriggerSkill(caster, castParameter.TargetObject, castParameter.TargetLoc, HitTriggerType.Trigger, castParameter.StageId);
        }
    }

    // 技能命中时触发
    public void OnTriggerSkill(GameUnit caster, GameObject target, Vector3 contactPoint, HitTriggerType hitTriggerType, int stage, 
        int comboIndex = 0)
    {
        if (rawEffects != null)
        {
            GameUnit targetUnit = target.GetComponent<GameUnit>();
            for (int index = 0; index < rawEffects.Count; index++)
            {
                RawEffectConfig config = rawEffects[index];
                if (checkType(config.triggerType, hitTriggerType, config.triggerStage, stage, targetUnit))
                {
                    if (config.effectObject)
                    {
                        EffectUtil.createEffect(config.effectObject.gameObject, contactPoint, target.transform.rotation,
                            caster, target, targetUnit, index, endureLevel, comboIndex);
                    }
                    else
                    {
                        Debug.LogWarning("EffectObject is null");
                    }
                }
            }
        }
    }

    private bool checkType(HitTriggerType configTriggerType, HitTriggerType hitTriggerType, int configTriggerStage, int stage, GameUnit targetUnit)
    {
        if (configTriggerType == HitTriggerType.All || hitTriggerType == HitTriggerType.All)
        {
            return true;
        }
        else if (configTriggerType == hitTriggerType && configTriggerStage == stage)
        {
            return true;
        }
        return false;
    }

}