using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.AI.Navigation;
using Unity.Services.Analytics.Internal;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Diagnostics;
using static Define;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.GridLayoutGroup;

public class Hero : Creature
{
    [SerializeField] private EHeroMoveState _heroMoveState = EHeroMoveState.None;
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

        }
    }
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;


        return true;
    }


    public override ECreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            base.CreatureState = value;
        }
    }


    public void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        ObjectType = EObjectType.Hero;

        SetupBasicSkills();
    }

    public void SetupBasicSkills()
    {
        //Skills.RegisterSkill("Jump");
        Skills.RegisterSkill("ComboAttack");
    }

    private void Update()
    {
        HandleInput();
    }

    public void HandleMovement()
    {
        Agent.nextPosition = Position + MoveDir * MoveSpeed * Time.deltaTime;
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

        if (!(inputDir.sqrMagnitude > 0))
        {
            HeroMoveState = EHeroMoveState.None;
        }
        else
        {
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
        }


        //if (!(MoveDir == DirVec.UP || MoveDir == DirVec.DOWN))

        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.transform.gameObject.layer);
            }

            //if (Skills.TryActivateSkill("ComboAttack"))
            //{
            //    CreatureState = ECreatureState.Skill;
            //    return;
            //}
        }

    }

    protected override void UpdateAnimation()
    {
        //base.UpdateAnimation();
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                HeroMoveState = EHeroMoveState.None;
                Anim.SetFloat("MoveSpeed", 0);
                Anim.SetTrigger("Locomotion");
                break;
            case ECreatureState.Skill:
                HeroMoveState = EHeroMoveState.None;
                break;
            case ECreatureState.Move:
                Anim.SetFloat("MoveSpeed", 1);
                Anim.SetTrigger("Locomotion");
                break;
            case ECreatureState.OnDamaged:
                break;
            case ECreatureState.Death:
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
        // TODO: ForceMove관련 체크하고 이동
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            //transform.position += MoveDir * MoveSpeed * Time.deltaTime;
            LookLeft = MoveDir.x < 0;
            Agent.nextPosition = Position + MoveDir * MoveSpeed * Time.deltaTime;
            return;
        }

        if (HeroMoveState == EHeroMoveState.None)
            CreatureState = ECreatureState.Idle;
    }


    protected override void UpdateSkill()
    {
        base.UpdateSkill();

        
    }
}
