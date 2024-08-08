using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static Define;
using static UnityEngine.UI.GridLayoutGroup;

public class Creature : InteractionObject
{
    #region Stat Value
    public float MaxHpBase { get; set; }
    public float AtkBase { get; set; }
    public float CriRateBase { get; set; }
    public float CriDamageBase { get; set; }
    public float MissBase { get; set; }
    public float ReduceDamageRateBase { get; set; }
    public float ReduceDamageBase { get; set; }
    public float LifeStealRateBase { get; set; }
    public float ThornsDamageRateBase { get; set; }
    public float MoveSpeedBase { get; set; }
    public float AttackSpeedRateBase { get; set; }
    public float CooldownReductionBase { get; set; }
    [field: SerializeField] public float MaxHp { get; set; }
    [field: SerializeField] public float Atk { get; set; }
    [field: SerializeField] public float CriRate { get; set; }
    [field: SerializeField] public float CriDamage { get; set; }
    [field: SerializeField] public float MissChance { get; set; }
    [field: SerializeField] public float ReduceDamageRate { get; set; }
    [field: SerializeField] public float ReduceDamage { get; set; }
    [field: SerializeField] public float LifeStealRate { get; set; }
    [field: SerializeField] public float ThornsDamageRate { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public float AttackSpeedRate { get; set; }
    [field: SerializeField] public float CooldownReduction { get; set; }
    [field: SerializeField] private bool _dirty = false;

    #endregion
    [field: SerializeField] public Data.CreatureData CreatureData { get; protected set; }

    public SkillComponent Skills { get; set; }
    protected Vector3 InitPos { get; set; }
    public Vector3 MoveDir { get; set; } = Vector3.zero;
    [SerializeField] protected ECreatureState _creatureState = ECreatureState.Idle;

    [SerializeField] protected float _hp;
    public virtual float Hp
    {
        get => _hp;
        set => _hp = value;
    }

    public Rigidbody Rigid => _rigidbody;

    private NavMeshAgent _agent;
    public NavMeshAgent Agent => _agent;
    [SerializeField] private InteractionObject _target;
    public InteractionObject Target
    {
        get => _target;
        set { _target = value; }
    }

    protected HurtFlashEffect _hurtFlash;
    public virtual ECreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            if (_creatureState != value)
            {
                _creatureState = value;
                CancelWait();

                UpdateAnimation();
                OnStateBegin();
            }
        }
    }

    private Rigidbody _rigidbody;

    //Test
    protected TextMeshPro _tmpDebug;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        _rigidbody = GetComponent<Rigidbody>();

        _agent = Util.GetOrAddComponent<NavMeshAgent>(gameObject);

        _agent.speed = 3.5f;
        _agent.angularSpeed = 0;
        _agent.acceleration = 20f;
        _agent.stoppingDistance = 1.5f;

        _hurtFlash = gameObject.GetOrAddComponent<HurtFlashEffect>();
        _hurtFlash.Init();

        Hp = MaxHp;


        return true;
    }

    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        UpdateAnimation();
        StartCoroutine(CoUpdateState());

        if (ObjectType == EObjectType.Hero)
            CreatureData = Managers.Data.HeroDic[templateId];
        else
            CreatureData = Managers.Data.MonsterDic[templateId];

        RuntimeAnimatorController rac = Managers.Resource.Load<RuntimeAnimatorController>(CreatureData.AnimationControllerName);
        Anim.runtimeAnimatorController = rac;

        InitCreatureStat();
        CalculateStat();
        SetSkill();

        CreatureState = ECreatureState.Idle;
        Anim.SetBool("IsDead", false);
        OnStateBegin();
    }

    protected virtual void SetSkill() 
    {
        Skills.UpdateSkill(CreatureData.DefaultSkillId, ESkillSlot.Default);
        Skills.UpdateSkill(CreatureData.EnvSkillId, ESkillSlot.Env);
        Skills.UpdateSkill(CreatureData.SkillAId, ESkillSlot.A);
        Skills.UpdateSkill(CreatureData.SkillBId, ESkillSlot.B);
    }

    protected virtual void InitCreatureStat()
    {
        Hp = CreatureData.MaxHp;

        //BaseValue
        MaxHp = MaxHpBase = CreatureData.MaxHp;
        Atk = AtkBase = CreatureData.Atk;
        CriRate = CriRateBase = CreatureData.CriRate;
        CriDamage = CriDamageBase = CreatureData.CriDamage;
        MissChance = MissBase = 0.0f;
        ReduceDamageRate = ReduceDamageRateBase = 0;
        ReduceDamage = ReduceDamageBase = 0;
        LifeStealRate = LifeStealRateBase = 0;
        ThornsDamageRate = ThornsDamageRateBase = 0;
        MoveSpeed = MoveSpeedBase = CreatureData.MoveSpeed;
        AttackSpeedRate = AttackSpeedRateBase = 1;
        CooldownReductionBase = 0;
    }

    public virtual void CalculateStat()
    {
        float prevMaxHp = MaxHp;
        MaxHp = CalculateFinalStat(MaxHpBase, ECalcStatType.MaxHp);
        Atk = CalculateFinalStat(AtkBase, ECalcStatType.Atk);

        CriRate = CalculateFinalStat(CriRateBase, ECalcStatType.Critical);
        CriDamage = CalculateFinalStat(CriDamageBase, ECalcStatType.CriticalDamage);
        MissChance = CalculateFinalStat(MissBase, ECalcStatType.MissChance);
        ReduceDamageRate = CalculateFinalStat(ReduceDamageRateBase, ECalcStatType.ReduceDamageRate);
        ReduceDamage = CalculateFinalStat(ReduceDamageBase, ECalcStatType.ReduceDamage);
        LifeStealRate = CalculateFinalStat(LifeStealRateBase, ECalcStatType.LifeStealRate);
        ThornsDamageRate = CalculateFinalStat(ThornsDamageRateBase, ECalcStatType.ThornsDamageRate);
        MoveSpeed = CalculateFinalStat(MoveSpeedBase, ECalcStatType.MoveSpeed);
        AttackSpeedRate = CalculateFinalStat(AttackSpeedRateBase, ECalcStatType.AttackSpeedRate);
        CooldownReduction = CalculateFinalStat(CooldownReduction, ECalcStatType.CooldownReduction);

        // 최대 HP번경
        if (prevMaxHp != MaxHp)
        {
            float hpRatio = Hp / prevMaxHp;
            Hp = MaxHp * hpRatio;
            Hp = Mathf.Clamp(Hp, 0, MaxHp);
        }

        float ratio = Hp / MaxHp;
        _hpBar?.Refresh(ratio);
    }

    protected virtual float CalculateFinalStat(float baseValue, ECalcStatType calcStatType)
    {
        return 0;
    }


    protected virtual void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                Anim.SetFloat("MoveSpeed", 0);
                Anim.Play("Locomotion");
                
                break;
            case ECreatureState.Cooltime:
                break;
            case ECreatureState.Move:
                Anim.SetFloat("MoveSpeed", 1);
                Anim.Play("Locomotion");
                break;
            case ECreatureState.OnDamaged:
                break;
            case ECreatureState.Death:
                Anim.SetBool("IsDead", true);
                break;
            case ECreatureState.Skill:
                break;
            default:
                break;
        }
    }
    protected virtual void OnStateBegin()
    {
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                BeginIdle();
                break;
            case ECreatureState.Cooltime:
                BeginCooltime();
                break;
            case ECreatureState.Move:
                BeginMove();
                break;
            case ECreatureState.OnDamaged:
                BeginDamaged();
                break;
            case ECreatureState.Death:
                BeginDeath();
                break;
            case ECreatureState.Skill:
                BeginSkill();
                break;
            default:
                break;
        }
    }

    protected virtual IEnumerator CoUpdateState()
    {
        while(true)
        {
            yield return null;

            switch (CreatureState)
            {
                case ECreatureState.Idle:
                    UpdateIdle();
                    break;
                case ECreatureState.Move:
                    UpdateMove();
                    break;
                    //case ECreatureState.Cooltime:
                    //    UpdateCooltime();
                    //    break;
                case ECreatureState.Skill:
                    UpdateSkill();
                    break;
                case ECreatureState.Death:
                    UpdateDeath();
                    break;
            }
        }
    }

    protected virtual void BeginIdle() { }
    protected virtual void UpdateIdle()
    {
        
    }
    protected virtual void BeginMove() { }
    protected virtual void UpdateMove()
    {

    }
    protected virtual void BeginCooltime() { }
    protected virtual void UpdateCooltime()
    {

    }

    protected virtual void BeginSkill() 
    {
        if (Skills.CurrentSkill == null)
        {
            CreatureState = ECreatureState.Idle;
            return;
        }

        Skills.CurrentSkill.DoSkill();
    }
    protected virtual void UpdateSkill()
    {
    }

    protected virtual void BeginDeath() 
    {
        OnDead();
    }
    
    protected virtual void UpdateDeath()
    {
    }

    protected virtual void BeginDamaged() 
    {
        Skills.CurrentSkill?.CancelSkill();

    }
    protected virtual void UpdateDamaged() { }

    public override void OnDamage(InteractionObject attacker, float damage)
    {
        if (attacker.IsValid() == false)
            return;

        if (damage > 0)
        {
            Anim.Play(AnimName.TAKE_HIT);
            _hurtFlash.Flash();
        }

        Creature creatureAttacker = attacker as Creature;
        if (creatureAttacker == null)
            return;

        bool isCritical = Util.CheckProbability(creatureAttacker.CriRate);
        EDamageResult damageResult = EDamageResult.Hit;
        if (damage < 0)
        {
            if (isCritical)
            {
                damageResult = EDamageResult.CriticalHeal;
                damage *= creatureAttacker.CriDamage;
            }
            else
            {
                damageResult = EDamageResult.Heal;
            }
        }
        else if (Util.CheckProbability(MissChance))
        {
            damageResult = EDamageResult.Miss;
            Managers.Object.ShowDamageFont(OverheadPosition, damage, transform, damageResult);
            return;
        }
        else if (isCritical)
        {
            damage *= creatureAttacker.CriDamage;
            damageResult = EDamageResult.CriticalHit;
        }

        if (damageResult != EDamageResult.Heal)
        {
            damage -= ReduceDamage;
            damage -= damage * ReduceDamageRate;
        }

        Managers.Object.ShowDamageFont(OverheadPosition, damage, transform, damageResult);

        Hp = Mathf.Clamp(Hp - damage, 0, MaxHp);
        float ratio = Hp / MaxHp;
        _hpBar?.Refresh(ratio);

        if (Hp == 0)
        {
            CreatureState = ECreatureState.Death;
        }
    }

    protected virtual void OnDead()
    {
        CancelWait();
        base.OnDead();
    }


    #region Wait

    protected Coroutine _coWait;
    protected Action _waitEndAction;

    public void StartWait(float seconds, Action waitEndAction = null)
    {
        CancelWait();
        _waitEndAction = waitEndAction;
        _coWait = StartCoroutine(CoWait(seconds));
    }

    IEnumerator CoWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _waitEndAction?.Invoke();
        _coWait = null;
    }

    public void CancelWait()
    {
        if (_coWait != null)
            StopCoroutine(_coWait);
        _coWait = null;
    }

    #endregion

    public bool IsGrounded { get; private set; }
    public bool IsFalling => _rigidbody.velocity.y <= 0;
    public float JumpForce = 5.0f;
    public void Launch(Vector3 launchDir, float launchForce)
    {
        IsGrounded = false;

        if (Agent.enabled)
        {
            Agent.updatePosition = false;
            Agent.updateRotation = false;
            Agent.isStopped = true;
        }
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;

        _rigidbody.AddRelativeForce(launchDir * launchForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (IsGrounded == false)
            {
                if (Agent.enabled)
                {
                    Agent.nextPosition = Position;
                    Agent.updatePosition = true;
                    Agent.updateRotation = true;
                    Agent.isStopped = false;
                }
                _rigidbody.isKinematic = true;
                _rigidbody.useGravity = false;
                IsGrounded = true;
            }
        }
    }
    public IEnumerator CoLerpInDirection(Vector3 direction, float duration, float distance = 1.0f)
    {
        Vector3 startPosition = Position;
        Vector3 targetPosition = startPosition + direction * distance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 1.0f, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        Agent.Warp(targetPosition);
    }

}
