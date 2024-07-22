using Castle.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class MeleeAttack : SkillBase
{


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SkillType = ESkillType.Normal;


        return true;
    }


    public override void SetInfo(int skillId)
    {
        base.SetInfo(skillId);

        foreach (AnimationClip clip in Owner.Anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Contains("Attack_1"))
            {
                SkillAnimDuration = clip.length;
            }
        }


        if (Owner.ObjectType == EObjectType.Hero)
        { 
            _targetMask.AddLayer(ELayer.Monster);
            _targetMask.AddLayer(ELayer.Env);
        }
        else if (Owner.ObjectType == EObjectType.Monster)
            _targetMask = LayerMask.GetMask("Hero");

    }

    public override void CancelSkill()
    {
        base.CancelSkill();        
    }

    public override void DoSkill()
    {
        base.DoSkill();

        Owner.Anim.SetFloat("MeleeAttackBlend", 0);
        Owner.Anim.SetTrigger(AnimName.MELEEATTACK);

        StopCoroutine(CoMeleeAttack());
        StartCoroutine(CoMeleeAttack());
    }

    IEnumerator CoMeleeAttack()
    {

        yield return new WaitForSeconds(SkillAnimDuration);

        if (Owner.IsValid())
            Owner.CreatureState = ECreatureState.Idle;
    }

    void OnAttackEvent()
    {
        Vector3 frontPosition = Owner.Position + (Owner.LookLeft ? Vector3.left : Vector3.right) + Vector3.up;

        Collider[] hitColliders = Physics.OverlapSphere(frontPosition, 1.5f, _targetMask);

        if (hitColliders.Length > 0)
        {
            InteractionObject obj = hitColliders[0].gameObject.GetComponent<InteractionObject>();

            //TODO: Env or Monster
            if (obj.ObjectType == EObjectType.Env)
            {
                obj.OnDamage(Owner, 10);
            }
            else if (obj.ObjectType == EObjectType.Monster)
            {
                ApplyEffects(obj);
            }
            else if (obj.ObjectType == EObjectType.Hero)
            {
                ApplyEffects(obj);
            }

        }
    }

    private void OnDrawGizmos()
    {
        if (Owner.CreatureState == ECreatureState.Skill)
        {
            Vector3 frontPosition = Owner.Position + (Owner.LookLeft ? Vector3.left : Vector3.right) + Vector3.up;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(frontPosition, 1.5f);
        }
    }
}
