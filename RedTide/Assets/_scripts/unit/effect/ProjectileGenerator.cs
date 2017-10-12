using System.Collections;
using UnityEngine;

// 类似于粒子生成器, 产生多个弹道
public class ProjectileGenerator : MonoBehaviour
{
    [Tooltip("自身或者父体附着的EffectObject")]
    public EffectObject ParentEffect;
    [Tooltip("发射的弹道")]
    public EffectObject Projectile;
    public float MaxAngle;
    [Tooltip("不为0, 则一轮中每个弹道角度间隔固定为IntervalAngle")]
    public float IntervalAngle = 0f;
    [Tooltip("重复轮数")] public int Rounds = 1;
    [Tooltip("每轮间隔")] public float RoundInterval = 0;
    [Tooltip("每轮重新瞄准")] public bool ReAim = false;
    [Tooltip("每一轮中弹道数量")]
    public int ProjectileCount;
    [Tooltip("起始延迟时间")]
    public float StartDelay = 3;
    [Tooltip("每一轮中每个弹道的时间间隔")]
    public float Interval = 0f;

    private void Start()
    {
        StartCoroutine(Generate(StartDelay));
        
    }

    private IEnumerator Generate(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int j = 0; j < Rounds; j++)
        {
            if (RoundInterval > 0f)
            {
                StartCoroutine(GenerateRoundCoroutine(j * RoundInterval));
            }
            else
            {
                GenerateEachRound();
            }
        }
    }

    private IEnumerator GenerateRoundCoroutine(float delay)
    {// 开始新一轮弹道发射
        yield return new WaitForSeconds(delay);

        if (ReAim)
        {
            var caster = ParentEffect.caster;
            if (caster)
            {
                var mob = caster as MobUnit;
                if (mob)
                {
                    var target = mob.GetTarget();
                    if (target)
                    {
                        var dir = target.transform.position - mob.transform.position;
                        dir.Normalize();
                        var rotation = Quaternion.LookRotation(dir);
                        ParentEffect.transform.localRotation = rotation;
                        caster.transform.rotation = Quaternion.LookRotation(dir);
                    }
                }
            }
        }
        GenerateEachRound();
    }

    private void GenerateEachRound()
    {
        var caster = ParentEffect.caster;
        var angle = 0f;
        for (int i = 0; i < ProjectileCount; i++)
        {
            if (IntervalAngle <= 0.001f)
            {// 随机角度
                angle = Random.value * MaxAngle - MaxAngle / 2f;
            }
            else
            {
                angle += IntervalAngle;
            }
            if (Interval > 0f)
            {
                StartCoroutine(GenerateProjectileCoroutine(i * Interval, caster, angle));
            }
            else
            {
                GenerateProjectile(caster, angle);
            }
        }
    }

    private IEnumerator GenerateProjectileCoroutine(float delay, GameUnit caster, float angle)
    {
        yield return new WaitForSeconds(delay);
        
        GenerateProjectile(caster, angle);
    }

    private void GenerateProjectile(GameUnit caster, float angle)
    {
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        Quaternion pRot = Quaternion.LookRotation(rot * transform.forward);
        EffectUtil.createEffect(Projectile.gameObject, transform.position, pRot, caster, null, null,
            ParentEffect.GetSkillEndureLevel());
    }
}