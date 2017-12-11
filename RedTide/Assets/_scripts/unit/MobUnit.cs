using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Random = UnityEngine.Random;

public class MobUnit : GameUnit {

    private int level;
    private float _attackPower;
    private float _spellPower;
    private MobControl _mobControl;
    private GameUnit _awakeRouser;        // 唤醒自己的GameUnit
    private MobAIPath _mobAiPath;

    public MobConfig Config { get; private set; }
    public int SpecialWeapon { get; set; }

    protected override void Start()
    {
        base.Start();
        _mobControl = GetComponent<MobControl>();
        
        // 加入NavmeshCut
        StartCoroutine(InitNavmeshCutDelay());    
    }

    public void Init(MobConfig config, int level)
    {
        this.level = level;
        Mass = config.Mass;
        Config = Instantiate(config);
        Init(config.CreateBasicAttributeConfig(this.level));
        _attackPower = config.attackFactor * this.level * 2f;
        _spellPower = config.spellFactor * this.level * 2f;
    }

    public override float GetAttackPower()
    {
        return _attackPower;
    }

    public override float GetSpellPower()
    {
        return _spellPower;
    }

    protected override bool OnDeath()
    {
        // 死亡特效
        if (Config.DeathFx)
        {
            Instantiate(Config.DeathFx, transform.position, transform.rotation);
        }
        if (!Dead)
        {
            Dead = true;

            // TODO 将死亡技能改由Rogdoll触发
            if (Config.deathSkill && !Config.DeathSkillBindRd)
            {    // 死亡时触发的技能
                _mobControl.DeactiveCurrentSkill();
                _mobControl.DoSimpleAttackWithoutCheck(transform.forward, Config.deathSkill);

                StartCoroutine(DestroyAfterDeathSkill(Config.deathSkill.animTime));
            }
            else
            {
                if (FilterOnBroken() && Config.FractureObject != null)
                {
                    Destroy(gameObject);
                    Instantiate(Config.FractureObject, transform.position, transform.rotation);
                }
                else if (Config.RagModel != null)
                {   // 启用ragdoll
                    Destroy(gameObject);
                    var ragdollObj = Instantiate(Config.RagModel, transform.position, transform.rotation, RagdollRoot.Root.transform);
                    ragdollObj.AddComponent<RagdollParentMarker>();
                    {
                        ragdollObj.AddComponent<BodyExplodeEffect>();
                    }
                    
                    // 尸体加入NavmeshCut
                    var navmeshCut = ragdollObj.AddComponent<NavmeshCut>();
                    navmeshCut.type = NavmeshCut.MeshType.Circle;
                    navmeshCut.circleRadius = Radius * 1.2f;
                    navmeshCut.updateDistance = 1f;
                }
                else
                {
                    var rdB = GetComponent<Rigidbody>();
                    if (rdB)
                        rdB.isKinematic = false;
                    StartCoroutine(DeferedDeath(2f));
                }
            }
        }
        return true;
    }

    private IEnumerator DestroyAfterDeathSkill(float delay)
    {
        // 注意预留绑定特效的存续时间
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    public override bool isEnemy(GameUnit otherUnit)
    {
        if (otherUnit.GetType() == typeof(MobUnit) && ((MobUnit) otherUnit).PlayerId == this.PlayerId)
        {
            return false;
        }
        return true;
    }

    IEnumerator DeferedDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    protected override void OnHit(GameUnit srcUnit, float actualDamage, int hitEndureLevel)
    {
        if (actualDamage > 0)
        {
            if (_mobControl)
            {
                _mobControl.OnHit(srcUnit, actualDamage, hitEndureLevel);
            }
            var mobAi = transform.GetComponent<AI>();
            if (mobAi && !mobAi.enabled)
            {// 被攻击后唤醒AI
                ActiveAI(srcUnit);
            }
        }
    }

    protected override void OnFalling()
    {
        // 掉落至场景外, 直接死亡, TODO 测试掉落位置是否合法
        this.CurrentHp = 0;
    }

    // 此函数只考虑技能CD
    public Dictionary<SkillConfig, int> GetAvailableSkills()
    {
        return _mobControl.GetAvailableSkills();
    }

    public override void ActiveAI(GameUnit rouser)
    {
        var mobAi = transform.GetComponent<AI>();
        if (mobAi)
        {
            mobAi.enabled = true;
            _mobAiPath = mobAi.GetComponent<MobAIPath>();
            if (_mobAiPath)
            {
                _mobAiPath.enabled = true;
                _mobAiPath.MovingTargetProvider = null;
            }
        }
        if (_mobControl)
        {
            _mobControl.InitSkillCd();
        }

    }

    public GameUnit GetRouser()
    {
        return _awakeRouser;
    }

    public GameUnit GetTarget()
    {
        var ai = GetComponent<MobAI>();
        if (ai)
        {
            return ai.GetTarget();
        }
        return null;
    }

    private IEnumerator InitNavmeshCutDelay()
    {
        yield return new WaitForSeconds(0.1f);
        
        var navmeshCut = gameObject.AddComponent<NavmeshCut>();
        navmeshCut.type = NavmeshCut.MeshType.Circle;
        navmeshCut.circleRadius = Radius * 2f;
        navmeshCut.updateDistance = 0.6f;
    }

    public void MoveTo(Vector3 pos)
    {
        // TODO 整合进AI中
        _mobAiPath = GetComponent<MobAIPath>();
        if (_mobAiPath)
        {
            _mobAiPath.SetTargetPos(pos);
            _mobAiPath.endReachedDistance = 0.2f;
            _mobAiPath.slowdownDistance = 0.5f;
            _mobAiPath.EnableTrace(true);
        }
    }
}
