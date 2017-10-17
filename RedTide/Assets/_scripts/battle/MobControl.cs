using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MobControl : MonoBehaviour, IColliderListener, IMovingListener
{
    public static float ON_HIT_DURATION = 1.3f;
    private Animator _anim;
    private bool _canMove;
    private SkillStageControl _stageControl = new SkillStageControl();

    public bool CanMove()
    {
        return _canMove && _mobUnit.FilterOnMove();
    }

    public bool CanRotate()
    {
        if (_currentSkill != null)
        {// 前摇过程中
            return _currentSkill.CanRotate && !_hasTakenEffect;
        }
        return true;
    }

    private Dictionary<HitTriggerType, List<ColliderTrigger>> _triggers;
    private SkillConfig _currentSkill;
    private bool _hasTakenEffect = false;    // 用于标记是否成功触发过, 否则在结束时不计算CD

    private MobUnit _mobUnit;
    private AudioSource _audioSource;
    private HitTriggerType _latestTriggerType;
    private MobAIPath _mobAiPath;
    private Dictionary<SkillConfig, float> _skillCdMap;
    private float _animMoveSpeed;

    // for debug
    private Vector3 _latestHitPoint;

    private float _onHitStamp;    // 上次被命中时间
    private int _onHitMulti;      // 被连续命中次数

    public PatrolRouteProvider PatrolProvider { set; private get; }

    public delegate void SkillHandler(SkillConfig skillConfig);
    public event SkillHandler beginSkillEvent;
    public event SkillHandler endSkillEvent;

    // 技能被打断时的通知
    public delegate void OnHitCancelDelegate();
    public event OnHitCancelDelegate OnHitCancel;
    
    private void Awake()
    {
        _stageControl.Reset();
    }

    private void Start()
    {
        _mobUnit = GetComponent<MobUnit>();
        _audioSource = GetComponent<AudioSource>();
        _anim = GetComponentInChildren<Animator>();
        _canMove = true;
        _triggers = new Dictionary<HitTriggerType, List<ColliderTrigger>>();
        _skillCdMap = new Dictionary<SkillConfig, float>();
        foreach (var attackSkill in _mobUnit.Config.attackSkills)
        {
            if (attackSkill.MaxCooldown > 0)
            {
                _skillCdMap[attackSkill] = Time.time + attackSkill.MinCooldown +
                           Random.value* Mathf.Max(0, attackSkill.MaxCooldown - attackSkill.MinCooldown);
            }
            else
            {
                _skillCdMap[attackSkill] = 0f;
            }
        }


        RefreshColliders();
        _mobAiPath = GetComponent<MobAIPath>();
        if (_mobAiPath)
        {
            _mobAiPath.MovingListener = this;
        }

        _animMoveSpeed = _mobUnit.Config.AnimMoveSpeed;

        if (PatrolProvider != null)
        {
            // 设置Target, AI激发后,关闭PatrolMode
            StartCoroutine(InitPatrol(PatrolProvider));
        }

    }


    private IEnumerator InitPatrol(PatrolRouteProvider provider)
    {
        if (provider.PatrolDelay > 0f)
        {
            yield return new WaitForSeconds(provider.PatrolDelay);
        }
        var mobAiPath = GetComponent<MobAIPath>();
        if (mobAiPath)
        {
            _mobUnit.AddEffect(new UnRegularMoveEffect(_mobUnit));
            mobAiPath.MovingTargetProvider = PatrolProvider;
            mobAiPath.SetTargetPos(PatrolProvider.GetNextTargetLoc(true));
        }
        mobAiPath.enabled = true;
    }

    private void Update()
    {
        _mobAiPath.speed = _mobUnit.MoveSpeed * GlobalFactor.Instance.NpcSpeed;
        if (Time.time - _onHitStamp > 3f)
        {
            _onHitMulti = 0;
        }
    }

    private void RefreshColliders()
    {
        _triggers.Clear();
        ColliderTrigger[] gameObjects = GetComponentsInChildren<ColliderTrigger>();
        foreach (ColliderTrigger child in gameObjects)
        {
            List<ColliderTrigger> tagTrigger;
            if (!_triggers.TryGetValue(child.type, out tagTrigger))
            {
                tagTrigger = new List<ColliderTrigger>();
                _triggers[child.type] = tagTrigger;
            }
            tagTrigger.Add(child);
        }
    }

    public bool CanActiveSkill(MobSkillConfig skillConfig)
    {
        if (!_mobUnit.FilterOnActiveSkill())
        {
            return false;
        }

        if (skillConfig != null && CheckCd(skillConfig))
        {
            return true;
        }
        return false;
    }

    public bool DoSimpleAttackWithoutCheck(Vector3 dir, SkillConfig skillConfig)
    {
        if (_anim)
        {
            _stageControl.Reset();
            _currentSkill = skillConfig;
            _hasTakenEffect = false;
            if (_currentSkill)
            {
                //Debug.Log(Time.time + " " + gameObject.name + " do skill: " + skillConfig.name);
                dir.Normalize();
                transform.rotation = Quaternion.LookRotation(dir);

                if (String.IsNullOrEmpty(_currentSkill.animTrigger))
                {
                    PlayCastEffect();
                    // 由EffectObject来决定退出时间
                    ActiveSkill();
                    if (_currentSkill.animTime > 0)
                    {
                        StartCoroutine(DeactiveCoroutine(_currentSkill.animTime));
                    }
                }
                else
                {
                    _anim.SetTrigger(_currentSkill.animTrigger);
                }
                _canMove = _currentSkill.CanMove;
                // 此处cd用于标记不可用,实际CD在EnableInput中进行
                _skillCdMap[_currentSkill] = Time.time + Math.Max(_currentSkill.MaxCooldown, _currentSkill.MinCooldown);
                if (beginSkillEvent != null)
                {
                    beginSkillEvent(skillConfig);
                }
//                Debug.Log("Time: " + Time.time + name + " do attack: " + _currentSkill.name);
                return true;
            }
        }
        else
        {
            Debug.LogWarning("Mob " + gameObject.name + " does not have Animator component");
        }
        return false;
    }

    private bool CheckCd(MobSkillConfig skillConfig)
    {
        return _currentSkill == null && Time.time >= _skillCdMap[skillConfig];
    }

    // Animation Event: For turning on root motion
    void MoveOn()
    {
        _anim.applyRootMotion = true;
    }

    // Animation Event: For turning off root motion when the animation is fully played
    void MoveOff(string input)
    {
        _anim.applyRootMotion = false;
    }

    // Animation Event: 可以取消这次动作, 进行下一次动作了, 或者类似deactiveSkill
    void EnableInput()
    {
        // 若存在强制位移, 由强制位移重置_currentSkill
        if (_mobUnit && _mobUnit.GetEffect(EffectConfig.EffectContextID.ForceMove) == null)
        {
            if (_currentSkill && _hasTakenEffect)
            {
                // 需判断是否成功触发过
                _skillCdMap[_currentSkill] = Time.time + _currentSkill.MinCooldown +
                                             Random.value * Mathf.Max(0, _currentSkill.MaxCooldown - _currentSkill.MinCooldown);

                if (endSkillEvent != null)
                {
                    endSkillEvent(_currentSkill);
                }
            }
            //Debug.Log(Time.time + " Skill: " + _currentSkill + " deactive");
            _currentSkill = null;
            _canMove = true;
        }
    }

    // Animation Event: 触发技能释放, 与EnableContact/DisableContact互斥
    void ActiveSkill()
    {
        if (_currentSkill)
        {
            _hasTakenEffect = true;
            // 设置技能初始位置
            SkillCastParameter castParameter = new SkillCastParameter();
            castParameter.TargetLoc = transform.position + Vector3.up;
            castParameter.TargetObject = gameObject;
            if (_currentSkill.CastType == CastType.Location || _currentSkill.CastType == CastType.Target)
            {
                var targetUnit = _mobUnit.GetTarget();
                if (targetUnit != null)
                {// TODO 存疑? 为什么是location类型技能而不是Target类型
                    castParameter.TargetLoc = targetUnit.transform.position;
                }
            }
            else if (_currentSkill.CastType == CastType.Self)
            {
                if (!_currentSkill.BindingOffset.Equals(Vector3.zero))
                {
                    // 自身位置 + BindingOffset 
                    castParameter.TargetLoc = transform.position + _currentSkill.BindingOffset.x * transform.right
                                              + _currentSkill.BindingOffset.y * Vector3.up +
                                              _currentSkill.BindingOffset.z * transform.forward;
                }
                else
                {
                    castParameter.TargetLoc = transform.position;
                }
            }
            _stageControl.NewStage(HitTriggerType.Trigger);
            SkillStage stage;
            if (_stageControl.stages.TryGetValue(HitTriggerType.Trigger, out stage))
            {
                castParameter.StageId = stage.stageId;
            }
            _currentSkill.OnActiveSkill(_mobUnit, castParameter);
        }
    }

    // Animation Event: Enables a set of Collider Triggers
    void EnableContact(string triggerType)
    {
        _hasTakenEffect = true;
        HitTriggerType type;
        if (TryParseTriggerType(triggerType, out type))
        {
            _stageControl.NewStage(type);
            List<ColliderTrigger> colliderTriggers;
            if (_triggers.TryGetValue(type, out colliderTriggers))
            {
                foreach (ColliderTrigger trigger in colliderTriggers)
                {
                    trigger.GetComponent<Collider>().enabled = true;
                }
                _latestTriggerType = type;
            }
        }
    }

    // Animation Event
    private void PlayCastSound()
    {
        if (_currentSkill != null && _currentSkill.CastSound != null)
        {
            Instantiate(_currentSkill.CastSound, transform.position, Quaternion.identity);
        }
    }

    // Animation Event
    private void PlayCastEffect()
    {
        // 通过名字索引
        if (_currentSkill != null && _currentSkill.CastEffect != null)
        {
            GameObject fx = Instantiate(_currentSkill.CastEffect);
            FxPlayControl playControl = fx.AddComponent<FxPlayControl>();
            playControl.TargetUnit = _mobUnit;
            playControl.FxPlayPosition = _currentSkill.PlayPosition;
            playControl.FxBindingMode = _currentSkill.BindingMode;
            if (_currentSkill.BindingMode == FxBindingMode.InitUnitPosition)
            {
                fx.transform.position = _mobUnit.transform.position;
                fx.transform.rotation = _mobUnit.transform.rotation;
            }
            else if (playControl.FxBindingMode != FxBindingMode.None)
            {
                fx.transform.SetParent(_mobUnit.transform);
            }
            else
            {
                fx.transform.position = transform.position + _currentSkill.BindingOffset.x * transform.right
                        + _currentSkill.BindingOffset.y * Vector3.up + _currentSkill.BindingOffset.z * transform.forward;
            }
            
            // 停止该技能后, 关闭castEffect
            if (_currentSkill.CastEffectDeactiveOnEnd)
            {
                 OnHitCancel += playControl.SetDeactive;
            }
        }
    }
    
    // Animation Event: Play foot sound
    private void LeftFootOnGround()
    {
        PlayFootSound(true);
    }
    private void RightFootOnGround()
    {
        PlayFootSound(false);
    }

    private void PlayFootSound(bool left)
    {
        AudioClip clip = _mobUnit.Config.LeftFootSound;
        if (!left)
        {
            clip = _mobUnit.Config.RightFootSound;
        }
        if (clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
//            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }

    // Animation Event: Disables a set of Collider Triggers
    void DisableContact(string triggerType)
    {
        HitTriggerType type;
        if (TryParseTriggerType(triggerType, out type))
        {
            List<ColliderTrigger> colliderTriggers;
            if (_triggers.TryGetValue(type, out colliderTriggers))
            {
                foreach (ColliderTrigger trigger in colliderTriggers)
                {
                    trigger.GetComponent<Collider>().enabled = false;
                }
            }
        }
    }

    private bool TryParseTriggerType(string triggerType, out HitTriggerType type)
    {
        try
        {
            type = (HitTriggerType) Enum.Parse(typeof(HitTriggerType), triggerType);
            return true;
        }
        catch
        {
            type = HitTriggerType.All;
            return false;
        }
    }

    public void OnColliderTrigger(HitTriggerType attackTriggerType, Collider attackTrigger, Collider victim,
        Vector3 contactVelocity, Vector3 contactPoint, GameObject hitFx, GameObject nonUnitHitFx)
    {
        SkillStage stage;
        if (_stageControl.stages.TryGetValue(attackTriggerType, out stage))
        {
            HashSet<GameObject> victimList = stage.hitSet;
            if (!victimList.Contains(victim.gameObject))
            {
                if (_currentSkill != null)
                {
                    _currentSkill.OnTriggerSkill(_mobUnit, victim.gameObject, contactPoint, attackTriggerType, stage.stageId);
                }

                victimList.Add(victim.gameObject);
            }
        }
    }

    public MobSkillConfig GetSkillConfig(int index)
    {
        if (index < 0) return null;
        if (_mobUnit.Config.attackSkills != null)
        {
            return _mobUnit.Config.attackSkills[index];
        }
        return null;
    }

    // 处理被击动画
    public void OnHit(GameUnit srcUnit, float baseDamage, int hitEndureLevel)
    {
        // TODO 检测霸体属性
        int currentEndureLevel = 0;
        if (_currentSkill)
        {
            currentEndureLevel = _currentSkill.endureLevel;
        }
        if (hitEndureLevel >= currentEndureLevel)
        {
            // 关闭现在正在处理的技能动作
            DisableContact(_latestTriggerType.ToString());
            MoveOff(_latestTriggerType.ToString());
            if (!_hasTakenEffect && OnHitCancel != null)
            {
                OnHitCancel();
            }
            EnableInput();

            var toCaster = srcUnit.transform.position - transform.position;
            toCaster.Normalize();
            var angle = Vector3.Angle(transform.forward, toCaster);
            _anim.SetFloat("HitDirection", angle);
            // 加入硬直保护, 连续处于硬直中, 则播放速度变快
            var onHitSpeed = GetOnHitSpeed();
            _anim.SetFloat("OnHitSpeed", onHitSpeed);

            float duration =  ON_HIT_DURATION / onHitSpeed;
            _mobUnit.AddEffect(new UncontrollableEffectConfig(srcUnit, _mobUnit, UncontrollableType.OnHit, duration));
            _anim.SetTrigger("OnHit");
            _onHitStamp = Time.time;
            _onHitMulti += 1;
            
            _mobUnit.FilterOnBreak(srcUnit);

            if (Random.value < 0.3f && _mobUnit.Config.BeHitSounds.Length > 0)
            {
                var clip = _mobUnit.Config.BeHitSounds[Random.Range(0, _mobUnit.Config.BeHitSounds.Length)];
                _audioSource.clip = clip;
                _audioSource.Play();
//                AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }
    }

    private float GetOnHitSpeed()
    {
        // TODO 改变被击动画速度
        return 1f;
    }

    public void NotifyMoving(bool moving)
    {
        _anim.SetBool("isWalking", moving);
        if (moving)
        {
            // 改成实际移动速度(被障碍物挡住)
            _anim.SetFloat("Speed", _mobUnit.MoveSpeed * GlobalFactor.Instance.NpcSpeed / _animMoveSpeed);
        }
    }

    public Dictionary<SkillConfig, int> GetAvailableSkills()
    {
        float currentTime = Time.time;
        Dictionary<SkillConfig, int> availableSkills = new Dictionary<SkillConfig, int>();
        int index = 0;
        foreach (var skillConfig in _mobUnit.Config.attackSkills)
        {
            if (currentTime >= _skillCdMap[skillConfig])
            {
                availableSkills.Add(skillConfig, index);
            }
            index++;
        }
        return availableSkills;
    }

    public delegate void CollideWithDelegate(GameObject hitObj);

    public event CollideWithDelegate CollideEvent;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (CollideEvent != null)
        {
            CollideEvent(hit.gameObject);
        }
    }

    void FixedUpdate()
    {
        _latestHitPoint = Vector3.zero;
    }

//    void OnDrawGizmos()
//    {
//        if (!_latestHitPoint.Equals(Vector3.zero))
//        {
//            Gizmos.DrawWireCube(_latestHitPoint, new Vector3(0.1f, 0.1f, 0.1f));
//        }
//    }

    // 初始化技能初始CD
    public void InitSkillCd()
    {
        var skills = _mobUnit.Config.attackSkills;
        foreach (var skillConfig in skills)
        {
            if (skillConfig.InitialCD > 0f)
            {
                _skillCdMap[skillConfig] = Time.time + skillConfig.InitialCD;
            }
        }
    }

    public SkillConfig GetCurrentSkill()
    {
        return _currentSkill;
    }

    public void DeactiveCurrentSkill()
    {
        EnableInput();
    }

    // 延迟一段时间后退出该技能; 
    private IEnumerator DeactiveCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        EnableInput();
    }

    private void OnDisable()
    {
        EnableInput();
    }
}