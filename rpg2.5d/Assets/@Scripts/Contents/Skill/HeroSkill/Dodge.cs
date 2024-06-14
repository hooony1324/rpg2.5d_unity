using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Dodge : SkillBase
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

    public override void DoSkill()
    {
        base.DoSkill();

        

    }
}
