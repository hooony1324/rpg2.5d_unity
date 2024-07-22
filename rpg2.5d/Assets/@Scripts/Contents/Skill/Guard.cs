using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Guard : SkillBase
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


    }

    public override void CancelSkill()
    {
        base.CancelSkill();

        Owner.IsGuardActivated = false;
        Owner.Anim.SetBool("IsGuarding", false);
        Owner.CreatureState = ECreatureState.Idle;
    }

    public override void DoSkill()
    {
        if (Owner.IsGuardActivated)
        {
            return;
        }

        Owner.IsGuardActivated = true;
        Owner.Anim.SetBool("IsGuarding", true);
    }
}
