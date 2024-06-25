using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Diagnostics;
using static Define;
using static UnityEditor.Recorder.OutputPath;
using Random = UnityEngine.Random;

public class Monster : Creature
{
    protected float ChaseRange = 8.0f;
    protected float _patrolRange = 5.0f;

    private Vector3 _originPos = default;
    private LayerMask _targetMask;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Monster;

        _originPos = Position;


        Agent.avoidancePriority = 40; // Hero : 50
        

        return true;
    }

    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        // stat
        
        _targetMask = LayerMask.GetMask("Hero");



    }

    protected override float CalculateFinalStat(float baseValue, ECalcStatType calcStatType)
    {
        float finalValue = baseValue;
        //finalValue += Effects.GetStatModifier(calcStatType, EStatModType.Add);
        //finalValue *= 1 + Effects.GetStatModifier(calcStatType, EStatModType.PercentAdd);
        //finalValue *= 1 + Effects.GetStatModifier(calcStatType, EStatModType.PercentMult);

        return finalValue;
    }

    bool IsAnimationRunning(int animHash)
    {
        int code = Anim.GetCurrentAnimatorStateInfo(0).GetHashCode();
        if (Anim.GetCurrentAnimatorStateInfo(0).GetHashCode() == animHash)
        {
            float normalizedTime = Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

            return normalizedTime != 0 && normalizedTime < 1f;
        }

        return false;
    }

    float UpdateAITick = 0;
    protected override IEnumerator CoUpdateState()
    {
        while (true)
        {
            Target = GetTargetInRange(ChaseRange);

            switch (CreatureState)
            {
                case ECreatureState.Idle:
                    UpdateIdle();
                    UpdateAITick = 0.1f;
                    break;
                //case ECreatureState.Cooltime:
                //    UpdateCooltime();
                //    UpdateAITick = 0.1f;
                //break;
                //case ECreatureState.OnDamaged:

                case ECreatureState.Move:
                    UpdateAITick = 0.0f;
                    UpdateMove();
                    break;
                case ECreatureState.Skill:
                    UpdateAITick = 0.1f;
                    UpdateSkill();
                    break;
                case ECreatureState.Death:
                    UpdateAITick = 1f;
                    UpdateDeath();
                    break;
            }

            if (UpdateAITick > 0)
                yield return new WaitForSeconds(UpdateAITick);
            else
                yield return null;
        }
    }

    protected override void BeginIdle()
    {

        if (Target.IsValid() == false)
        {
            StartWait(Random.RandomRange(1, 5), () => { CreatureState = ECreatureState.Move; });
        }
    }
    protected override void UpdateIdle()
    {

        if (Target.IsValid())
        {
            CancelWait();
            CreatureState = ECreatureState.Move;
            return;
        }
        
    }

    protected override void UpdateMove()
    {

        //Patrol
        if (Target.IsValid() == false)
        {
            PatrolRandomPosition();
            return;
        }

        LookAtTarget(Target.Position);

        if (Position.IsTargetInRange(Target.Position, 1.5f))
        {//Attack

            Skills.TrySkill(ESkillSlot.Default);
            CreatureState = ECreatureState.Skill;
            Anim.SetFloat("MoveSpeed", 0);
            return;
        }
        else
        {//Chase
            Agent.SetDestination(Target.Position);
            return;
        }

    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();

    }

    #region Map
    Vector3 _curDestPos;
    public void PatrolRandomPosition()
    {
        if (_curDestPos.EqualsEx(Agent.destination))
        {// PatrolDest설정됨

            if (Agent.remainingDistance <= Agent.stoppingDistance)
            {// PatrolDest도착

                _curDestPos = Vector3.zero;
                CreatureState = ECreatureState.Idle;
                return;
            }

            return;
        }

        //TODO : Z값을 고려하지 않은 randomPos, 갈 수 있는 범위인지를 고려하지 않은 randomPos
        //TODO : GetRandomPoint => Map Manger에서 관리
        Vector3 patrolPos = Position + Util.GetRandomPoint(_patrolRange); 
        LookAtTarget(patrolPos);

        Agent.SetDestination(patrolPos);
        _curDestPos = Agent.destination;
    }


    #endregion

    #region Combat
    InteractionObject GetTargetInRange(float range)
    {
        var targets = Physics.OverlapSphere(Position, range, _targetMask);

        return targets.Length > 0 ? targets[0].GetComponent<InteractionObject>() : null;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, ChaseRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, 1.5f);
    }
}
