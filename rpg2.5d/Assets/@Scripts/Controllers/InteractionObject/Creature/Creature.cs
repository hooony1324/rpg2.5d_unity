using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static Define;
using static UnityEngine.UI.GridLayoutGroup;

public class Creature : InteractionObject
{
    [SerializeField] private InteractionObject _target;
    public InteractionObject Target
    {
        get => _target;
        set { _target = value; }
    }


    private NavMeshAgent _agent;
    public Rigidbody Rigid => _rigidbody;
    
    protected float AttackRange = 1.5f;
    public float MoveSpeed { get; set; } = 5.0f;
    public float AttackSpeedRate { get; set; } = 1.0f;

    public SkillComponent Skills { get; set; }
    protected Vector3 InitPos { get; set; }
    public Vector3 MoveDir { get; set; } = Vector3.zero;
    [SerializeField] protected ECreatureState _creatureState = ECreatureState.Idle;
    public NavMeshAgent Agent => _agent;

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

        //Test
        //_tmpDebug = Util.FindChild<TextMeshPro>(gameObject, "DebugText");
        


        return true;
    }

    public virtual void SetInfo(int templateId)
    {

        UpdateAnimation();

        StartCoroutine(CoUpdateState());
    }

    protected virtual void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                PlayAnimation(AnimName.IDLE);
                break;
            case ECreatureState.Cooltime:
                break;
            case ECreatureState.Move:
                PlayAnimation(AnimName.RUN);
                break;
            case ECreatureState.OnDamaged:
                PlayAnimation(AnimName.TAKE_HIT);
                break;
            case ECreatureState.Death:
                PlayAnimation(AnimName.DEATH);
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

    void PlayAnimation(int stateNameHash)
    {
        Anim.Play(stateNameHash);
    }

    protected virtual IEnumerator CoUpdateState()
    {
        while(true)
        {
            switch(CreatureState)
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

            yield return null;
        }
    }

    protected virtual void BeginIdle() { }
    protected virtual void UpdateIdle()
    {
        UnityEngine.Debug.Log("idle");
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

        //TEST
        //°ø¼Ó
        //AttackSpeedRate = 2.0f;
        //Anim.speed = AttackSpeedRate;



    }

    protected virtual void BeginDeath() { }
    protected virtual void UpdateDeath()
    {

    }

    protected virtual void BeginDamaged() { }
    protected virtual void UpdateDamaged() { }

    public override void OnDamage(InteractionObject attacker, float value)
    {
        if (attacker.IsValid() == false)
            return;

        Creature creatureAttacker = attacker as Creature;
        if (creatureAttacker == null)
            return;

        EDamageResult damageResult = EDamageResult.Hit;
        Managers.Object.ShowDamageFont(OverheadPosition, 10, transform, damageResult);
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

}
