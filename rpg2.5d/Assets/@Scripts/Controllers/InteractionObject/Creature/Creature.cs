using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using static Define;

public class Creature : InteractionObject
{
    public float MoveSpeed { get; set; } = 5.0f;
    public SkillComponent Skills { get; set; }

    [Header("AI")][SerializeField] private InteractionObject _target;
    public InteractionObject Target
    {
        get => _target;
        set { _target = value; }
    }
    protected Vector3 InitPos { get; set; }
    public Vector3 MoveDir { get; set; } = Vector3.zero;
    [SerializeField] protected ECreatureState _creatureState = ECreatureState.Idle;
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

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        _rigidbody = GetComponent<Rigidbody>();

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
            case ECreatureState.Dead:
                PlayAnimation(AnimName.DEAD);
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
                case ECreatureState.Dead:
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

    public bool IsGrounded = false;
    public bool IsFalling => _rigidbody.velocity.y <= 0;
    public float JumpForce = 10.0f;
    public void Launch(Vector3 launchDir, float launchForce)
    {
        _rigidbody.velocity = launchDir * launchForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
        }
    }
}
