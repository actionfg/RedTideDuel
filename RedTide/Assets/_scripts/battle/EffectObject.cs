using UnityEngine;
using System.Collections.Generic;

public class EffectObject : MonoBehaviour
{
    [Tooltip("只能通过附带的碰撞体才能触发")]
    public bool colliderOnly;                         // True: 只能通过EffectObject附带的碰撞体才能触发
    public bool includeTriggerCollider = false;        // 碰到Trigger类型的Collider是否生效
    public GameObject bleedHitFx;
    public GameObject noBleedHitFx;
    public string EffectName;
    public Sprite Icon;
    [TextArea(3,10)]
    public string Description;

    private Effect[] _effects;
    private Collider _collider;

    [HideInInspector] public GameUnit caster;
    public int Index { get; set; }

    [HideInInspector] private int SkillEndureLevel;
    [HideInInspector] private int ComboIndex;

    private RemovalControl[] _removalControls;
    private AuraEffect _auraEffect;

    public HashSet<GameObject> hitSet = new HashSet<GameObject>();
    private bool activated = false;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _effects = GetComponents<Effect>();
        _removalControls = GetComponents<RemovalControl>();
        _auraEffect = GetComponent<AuraEffect>();
    }

    private void FixedUpdate()
    {
        if (!_auraEffect)
        {
            if (activated && _removalControls.Length == 0)
            {
                foreach (var effect in _effects)
                {
                    effect.DoTriggerOnCollideCompleted(this, caster, hitSet);
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!includeTriggerCollider && other.collider.isTrigger)
        {
            return;
        }
        CollideWith(other.gameObject);
    }

    private void OnTriggerEnter(Collider victim)
    {
        if (!includeTriggerCollider && victim.isTrigger)
        {
            return;
        }
        CollideWith(victim.gameObject);
    }

    // 可通过附着的Collider碰撞触发
    private void CollideWith(GameObject victim)
    {
        if (!_auraEffect && caster != null)
        {
            activated = true;
            if (!hitSet.Contains(victim))
            {
                hitSet.Add(victim);

                var target = victim;
                var targetUnit = target.GetComponent<GameUnit>();
                bool affected = false;
                foreach (var effect in _effects)
                {
                    affected |= effect.OnTrigger(this, caster, target, targetUnit, SkillEndureLevel, ComboIndex);
                }

                if (affected)
                {// 技能起效才播放命中效果
                    PlayHitFx(victim);
                    if (_removalControls.Length > 0)
                    {
                        foreach (var control in _removalControls)
                        {
                            control.notifyCollideWith(caster, victim);
                        }
                    }
                }
            }
        }
    }

    private void PlayHitFx(GameObject victim)
    {
        GameObject playingFx = noBleedHitFx;
        if (victim)
        {
            var gameUnit = victim.GetComponent<GameUnit>();

            if (gameUnit && bleedHitFx != null)
            {
                playingFx = bleedHitFx;
            }
        }
        if (playingFx)
        {
            Instantiate(playingFx, transform.position, transform.rotation);
        }
    }

    // 可通过武器ColliderTrigger触发, 还可通过SkillConfig的OnActiveSkill触发
    public void OnTrigger(GameObject target, GameUnit targetUnit, GameUnit casterUnit = null)
    {
        if (casterUnit != null)
        {
            this.caster = casterUnit;
        }
        if (_removalControls.Length > 0)
        {
            foreach (var control in _removalControls)
            {
                control.SetCaster(caster);
            }
        }
        if (!_auraEffect)
        {
            if (!colliderOnly)
            {
                bool affected = false;
                foreach (Effect effect in _effects)
                {
                    affected |= effect.OnTrigger(this, caster, target, targetUnit, SkillEndureLevel, ComboIndex);
                }
                if (affected)
                {
                    PlayHitFx(target);
                }

                if (_removalControls.Length == 0)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            _auraEffect.Caster = caster;
        }
    }

    public bool isActivated()
    {
        return activated;
    }

    public void Active()
    {
        activated = true;
    }

    public int GetSkillEndureLevel()
    {
        return SkillEndureLevel;
    }

    public void SetSkillEndureLevel(int endureLevel)
    {
        SkillEndureLevel = endureLevel;
        if (_auraEffect)
        {
            _auraEffect.SkillEndureLevel = SkillEndureLevel;
        }
    }

    public void SetComboIndex(int comboIndex)
    {
        this.ComboIndex = comboIndex;
    }
}