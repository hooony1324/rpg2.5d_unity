using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class Creature : InteractionObject
{
    [SerializeField] private InteractionObject _target;
    public InteractionObject Target
    {
        get => _target;
        set { _target = value; }
    }


    protected NavMeshAgent _agent;
    protected IBTNode _root;
    protected float _attackRange = 1.5f;
    protected float _chaseRange = 8.0f;

    public float MoveSpeed { get; set; } = 5.0f;
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


        //Test
        _tmpDebug = Util.FindChild<TextMeshPro>(gameObject, "DebugText");

        return true;
    }

    public virtual void SetInfo(int templateId)
    {

        UpdateAnimation();


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
                    UpdateDead();
                    break;
            }

            yield return null;
        }
    }

    protected virtual void UpdateIdle()
    {

    }
    protected virtual void UpdateMove()
    {

    }
    protected virtual void UpdateCooltime()
    {

    }
    protected virtual void UpdateSkill()
    {


        Skills.CurrentSkill?.DoSkill();

        //float animDuration;
        //animDuration = Skills.CurrentSkill.SkillAnimDuration;

        //float delay = animDuration;
        //StartWait(animDuration);
    }

    protected virtual void UpdateDead()
    {

    }

    protected override void OnDamaged(InteractionObject attacker, float value)
    {
        base.OnDamaged(attacker, value);

    }

    protected virtual void OnDead()
    {
        
        CancelWait();
        base.OnDead();
    }


    #region Wait

    protected Coroutine _coWait;

    protected void StartWait(float seconds)
    {
        CancelWait();
        _coWait = StartCoroutine(CoWait(seconds));
    }

    IEnumerator CoWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coWait = null;
    }

    protected void CancelWait()
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
