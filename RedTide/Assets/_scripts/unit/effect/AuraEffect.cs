using UnityEngine;
using System.Collections.Generic;

public class AuraEffect : Effect
{
    public EffectObject auraEffect;
    public GameObject auraFx;

    [Range(0.1f, 10f)]
    public float triggerInterval = 1f;

    [Header("Use 0 for infinite duration")] public float duration = 0f;

    private float _timeLeft;
    private float _acc;
    private float _canTrigger;
    private GameObject _fx;
    public GameUnit Caster { get; set; }
    public int SkillEndureLevel { get; set; }

    private HashSet<Collider> _colliders = new HashSet<Collider>();

    void Awake()
    {
        _timeLeft = duration;
        _acc = triggerInterval;
    }

    void Start()
    {
        if (auraFx)
        {
            _fx = Instantiate(auraFx, Caster.transform.position, Quaternion.identity);
            _fx.transform.SetParent(gameObject.transform);
        }
//        gameObject.transform.SetParent(Caster.transform);
        gameObject.transform.position = Caster.transform.position;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (duration > 0f)
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0)
            {
                Destroy(gameObject);
            }
        }

        _acc += Time.deltaTime;
        if (_acc >= triggerInterval)
        {
            _acc -= triggerInterval;
            _colliders.RemoveWhere(c => !c);
            foreach (Collider c in _colliders)
            {
                if (c)
                {
                    GameObject target = c.gameObject;
                    OnTrigger(auraEffect, Caster, target, target.GetComponent<GameUnit>(), SkillEndureLevel);
                }
            }
        }

        if (!Caster)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.transform.position = Caster.transform.position;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        _colliders.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
    }

    public override void DoTrigger(EffectObject effectObject, GameUnit caster, GameObject target, GameUnit targetUnit, int skillEndureLevel, int comboIndex)
    {
        EffectUtil.createEffect(effectObject.gameObject, target.transform.position, target.transform.rotation, caster, target,
            targetUnit, skillEndureLevel);
    }
}