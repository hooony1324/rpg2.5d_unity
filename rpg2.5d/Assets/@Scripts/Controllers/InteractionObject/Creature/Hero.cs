using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Diagnostics;
using static Define;
using static UnityEngine.GraphicsBuffer;

public class Hero : Creature
{

    [SerializeField] private EHeroMoveState _heroMoveState = EHeroMoveState.None;

    StringBuilder sb = new StringBuilder();
    public EHeroMoveState HeroMoveState
    {
        get => _heroMoveState;
        set
        {
            if (_heroMoveState == value)
                return;

            _heroMoveState = value;
            switch (value)
            {
                //case EHeroMoveState.CollectEnv:
                //    NeedArange = true;
                //    break;
                //case EHeroMoveState.TargetMonster:
                //    NeedArange = true;
                //    break;
                //case EHeroMoveState.ForceMove:
                //    Target = null;
                //    NeedArange = true;
                //    break;
            }


            sb.Clear();
            sb.AppendLine(Enum.GetName(typeof(ECreatureState), CreatureState));
            sb.AppendLine(Enum.GetName(typeof(EHeroMoveState), value));
            _tmpDebug.text = sb.ToString();
        }
    }

    //Test
    TextMeshPro _tmpDebug;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;


        SetInfo(0);

        _tmpDebug = Util.FindChild<TextMeshPro>(gameObject, "DebugText");

        return true;
    }

    //Test
    public override ECreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            base.CreatureState = value;

            sb.Clear();
            sb.AppendLine(Enum.GetName(typeof(ECreatureState), value));
            sb.AppendLine(Enum.GetName(typeof(EHeroMoveState), HeroMoveState));
            _tmpDebug.text = sb.ToString();
        }
    }


    public void SetInfo(int templateId)
    {
        base.SetInfo(0);

        Managers.Resource.LoadAsync<RuntimeAnimatorController>("Knight", (result) =>
        {
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>("Knight");
            
            Anim.runtimeAnimatorController = animController;

            
        });

        SetSkill();

    }

    private void SetSkill()
    {
        Skills.RegisterSkill(0);
    }

    private void Update()
    {
        HandleInput();


    }

    void HandleInput()
    {
        Vector2 inputDir = Vector3.zero;
        MoveDir = DirVec.ZERO;
        if (Input.GetKey(KeyCode.W))
        {
            inputDir.y += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputDir.y -= 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputDir.x += 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputDir.x -= 1;
        }

        if (Input.GetKey(KeyCode.Space) && IsGrounded)
        {
            if (Skills.SetCurrentSkill("NormalJump"))
            {
                CreatureState = ECreatureState.Skill;
            }
        }

        if (!(inputDir.sqrMagnitude > 0))
        {
            HeroMoveState = EHeroMoveState.None;
            return;
        }

        // Set Direction
        HeroMoveState = EHeroMoveState.ForceMove;
        float angle = Mathf.Atan2(inputDir.x, inputDir.y) * 180 / Mathf.PI;
        if (angle > 15f && angle <= 75f)
            MoveDir = DirVec.UP_LEFT;
        else if (angle > 75f && angle <= 105f)
            MoveDir = DirVec.LEFT;
        else if (angle > 105f && angle <= 160f)
            MoveDir = DirVec.DOWN_LEFT;
        else if (angle > 160f || angle <= -160f)
            MoveDir = DirVec.DOWN;
        else if (angle < -15f && angle >= -75f)
            MoveDir = DirVec.UP_RIGHT;
        else if (angle < -75f && angle >= -105f)
            MoveDir = DirVec.RIGHT;
        else if (angle < -105f && angle >= -160f)
            MoveDir = DirVec.DOWN_RIGHT;
        else
            MoveDir = DirVec.UP;

        if (!(MoveDir == DirVec.UP || MoveDir == DirVec.DOWN))
            LookLeft = MoveDir.x < 0;
    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                HeroMoveState = EHeroMoveState.None;
                break;
            case ECreatureState.Skill:
                HeroMoveState = EHeroMoveState.None;
                break;
            case ECreatureState.Move:
                break;
            case ECreatureState.OnDamaged:
                break;
            case ECreatureState.Dead:
                break;
            default:
                break;
        }
    }

    protected override void UpdateIdle()
    {
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
        }
    }

    protected override void UpdateMove()
    {
        // TODO: ForceMove���� üũ�ϰ� �̵�
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            transform.position += MoveDir * MoveSpeed * Time.deltaTime;
            return;
        }

        if (HeroMoveState == EHeroMoveState.None)
            CreatureState = ECreatureState.Idle;
    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();

        if (HeroMoveState == EHeroMoveState.ForceMove && !IsGrounded)
        {
            transform.position += MoveDir * MoveSpeed * 0.5f * Time.deltaTime;
            return;
        }
    }

    
}