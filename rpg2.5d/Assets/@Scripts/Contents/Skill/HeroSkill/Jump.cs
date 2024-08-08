using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;
using static UnityEngine.UI.GridLayoutGroup;

public class Jump : SkillBase
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

        foreach(AnimationClip clip in Owner.Anim.runtimeAnimatorController.animationClips)
        {
            if ("Jump" == clip.name)
            {
                SkillAnimDuration = clip.length;
            }
        }
    }

    public override void DoSkill()
    {
        base.DoSkill();

        Owner.Anim.Play(AnimName.JUMP);

        Vector3 jumpDir = new Vector3(Owner.MoveDir.x, 1.0f, Owner.MoveDir.z).normalized;
        Owner.Launch(jumpDir, Owner.JumpForce);
        StartCoroutine(CoNormalJump());
    }

    IEnumerator CoNormalJump()
    {
        yield return new WaitUntil(() => (Owner.IsFalling));
        Owner.Anim.Play(AnimName.FALL);

        yield return new WaitUntil(() => (Owner.IsGrounded));
        Owner.CreatureState = ECreatureState.Idle;
    }
}
