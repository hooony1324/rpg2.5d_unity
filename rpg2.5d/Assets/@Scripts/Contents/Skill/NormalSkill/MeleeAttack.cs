using System.Collections;
using System.Collections.Generic;
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


        //TODO: "Jump" -> SkillDataDic[skillId].AnimName
        foreach (AnimationClip clip in Owner.Anim.runtimeAnimatorController.animationClips)
        {
            if ("Attack_1" == clip.name)
            {
                SkillAnimDuration = clip.length;
            }
        }
    }
}
