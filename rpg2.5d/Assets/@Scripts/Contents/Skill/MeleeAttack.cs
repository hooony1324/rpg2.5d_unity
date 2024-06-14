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
            _targetMask = LayerMask.GetMask("Monster");
        else if (Owner.ObjectType == EObjectType.Monster)
            _targetMask = LayerMask.GetMask("Hero");

    }

    public override void CancelSkill()
    {
        base.CancelSkill();
    }

    public override void DoSkill()
    {
        Owner.Anim.SetFloat("MeleeAttackBlend", 0);
        Owner.Anim.SetTrigger(AnimName.MELEEATTACK);

        StartCoroutine(CoMeleeAttack());
    }

    IEnumerator CoMeleeAttack()
    {

        yield return new WaitForSeconds(SkillAnimDuration);
        Owner.CreatureState = ECreatureState.Idle;
    }

    void OnAttackEvent()
    {
        // TODO
        // 1. TargetSelcetion
        // 2. Effect ╫ц╫╨еш

        Vector3 frontPosition = Owner.Position + (Owner.LookLeft ? Vector3.left : Vector3.right) + Vector3.up;

        Collider[] hitColliders = Physics.OverlapBox(frontPosition, Vector3.one * 2, Quaternion.identity, _targetMask);

        if (hitColliders.Length > 0)
        {
            InteractionObject obj = hitColliders[0].gameObject.GetComponent<InteractionObject>();

            obj.OnDamage(Owner, 10);
        }
    }
}
