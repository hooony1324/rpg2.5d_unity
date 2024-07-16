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
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hero : Creature, IItemAccessible
{
    InputController _inputController;

    [SerializeField] private EHeroMoveState _heroMoveState = EHeroMoveState.None;

    LayerMask _uiCheckMask;
    public EHeroMoveState HeroMoveState
    {
        get => _heroMoveState;
        set
        {
            if (_heroMoveState == value)
                return;

            _heroMoveState = value;

        }
    }
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Hero;

        _uiCheckMask.AddLayer(ELayer.UI);
        _inputController = gameObject.GetOrAddComponent<InputController>();
        _inputController.Init(this);

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

    public override void SetInfo(int templateId)
    {
        //_heroInfo = Managers.Hero.GetHeroInfo(templateId);
        //Managers.Game.OnBroadcastEvent -= HandleOnBroadcast;
        //Managers.Game.OnBroadcastEvent += HandleOnBroadcast;
        base.SetInfo(templateId);
    }


    private void Update()
    {
        _inputController.HandleInput();
        _inputController.HandleMovement();
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

    protected override float CalculateFinalStat(float baseValue, ECalcStatType calcStatType)
    {
        float finalValue = baseValue;

        finalValue += Effects.GetStatModifier(calcStatType, EStatModType.Add);
        //              + Managers.Inventory.GetStatModifier(calcStatType, EStatModType.Add)
        //              + Managers.Game.GetTrainingStatModifier(calcStatType, EStatModType.Add);

        finalValue *= 1 + Effects.GetStatModifier(calcStatType, EStatModType.PercentAdd);
        //                + Managers.Inventory.GetStatModifier(calcStatType, EStatModType.PercentAdd)
        //                + Managers.Game.GetTrainingStatModifier(calcStatType, EStatModType.PercentAdd);

        finalValue *= 1 + Effects.GetStatModifier(calcStatType, EStatModType.PercentMult);
        

        return finalValue;
    }


    public ItemHolder TargetItemHolder { get; set; } = null;
    public void TrySetTargetItemHolder(ItemHolder itemHolder)
    {
        if (TargetItemHolder == null)
            TargetItemHolder = itemHolder;
        else
        {
            float originTarget = Vector3.SqrMagnitude(TargetItemHolder.Position - Position);
            float newTarget = Vector3.SqrMagnitude(itemHolder.Position - Position);

            if (newTarget < originTarget)
                TargetItemHolder = itemHolder;
        }
    }
}
