using BehaviorTree;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static BehaviorTree.IBTNode;
using static Define;
using static UnityEditor.Recorder.OutputPath;

public class Monster : Creature
{

    private Vector3 _originPos = default;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;


        Agent.speed = 3.5f;
        Agent.angularSpeed = 0;
        Agent.acceleration = 20f;
        Agent.stoppingDistance = 1.5f;

        _originPos = Position;

        IBTNode SelectorNode = new SelectorNode
            (
                new List<IBTNode>()
                {
                    // Attack Sequence
                    new SequenceNode
                    (
                        new List<IBTNode>()
                        {
                            new ActionNode(CheckAttacking),
                            new ActionNode(CheckTargetInAttackRange),
                            new ActionNode(DoAttack),
                        }
                    ),

                    // Chase Sequence
                    new SequenceNode
                    (
                        new List<IBTNode>()
                        {
                            new ActionNode(CheckTargetToChase),
                            new ActionNode(MoveToTarget)
                        }
                    ),

                    // Patrol or etc...
                    new ActionNode(MoveToOriginPosition)

                }
            );

        _root = SelectorNode;

        SetInfo(0);

        return true;
    }

    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);



        // stat
        CreatureState = ECreatureState.Idle;

        StartBT();
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
    IBTNode.ENodeState CheckAttacking()
    {
        if (IsAnimationRunning(AnimName.ATTACK))
        {
            return IBTNode.ENodeState.Running;
        }

        if (_coWait != null)
        {
            return IBTNode.ENodeState.Running;
        }

        return IBTNode.ENodeState.Success;
    }

    IBTNode.ENodeState CheckTargetInAttackRange()
    {
        if (Target != null)
        {
            if (Vector3.SqrMagnitude(Target.Position - Position) < _attackRange * _attackRange)
            {
                return IBTNode.ENodeState.Success;
            }
        }


        return IBTNode.ENodeState.Failure;
    }

    IBTNode.ENodeState DoAttack()
    {
        if (Target != null)
        {
            LookLeft = (Target.Position - Position).x < 0;
            Anim.Play(AnimName.ATTACK); 
            StartWait(2.0f);


            return IBTNode.ENodeState.Success;
        }

        return IBTNode.ENodeState.Failure;
    }

    IBTNode.ENodeState CheckTargetToChase()
    {
        var overlapColliders = Physics.OverlapSphere(Position, _chaseRange, LayerMask.GetMask("Hero"));

        if (overlapColliders != null && overlapColliders.Length > 0)
        {
            Target = overlapColliders[0].gameObject.GetComponent<InteractionObject>();

            _originPos = Position;

            CreatureState = ECreatureState.Move;
            return IBTNode.ENodeState.Success;
        }

        Target = null;
        return IBTNode.ENodeState.Failure;
    }

    IBTNode.ENodeState MoveToTarget()
    {
        if (Target != null)
        {
            Vector3 targetVec = Target.Position - Position;
            if (Vector3.SqrMagnitude(targetVec) < (_attackRange * _attackRange))
            {
                CreatureState = ECreatureState.Skill;
                return IBTNode.ENodeState.Success;
            }

            MoveDir = targetVec.normalized;
            LookLeft = MoveDir.x < 0;

            _agent.nextPosition = Vector3.MoveTowards(Position, Target.Position, Time.deltaTime * MoveSpeed);

            return IBTNode.ENodeState.Running;
        }

        return IBTNode.ENodeState.Failure;
    }

    IBTNode.ENodeState MoveToOriginPosition()
    {
        //Machine Epsilon 보다 작거나 같다면 두 실수는 같은 값이라 정의
        if (Vector3.SqrMagnitude(_originPos - Position) < float.Epsilon * float.Epsilon)
        {
            return IBTNode.ENodeState.Success;
        }
        else
        {
            transform.position = Vector3.MoveTowards(Position, _originPos, Time.deltaTime);
            return IBTNode.ENodeState.Running;
        }
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        
    }

    protected override void UpdateMove()
    {
        base.UpdateMove();


    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();
    }


    public void StartBT()
    {
        StartCoroutine(CoEvaluate());
    }

    public void StopBT()
    {
        StopCoroutine(CoEvaluate());
    }

    IEnumerator CoEvaluate()
    {
        while (true)
        {
            _root.Evaluate();

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, _chaseRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, _attackRange);
    }
}
