using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class SkillBase : BaseObject
{
    public Creature Owner { get; set; }

    int level = 0;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public ESkillType SkillType { get; set; }
    [SerializeField]
    private Data.SkillData _skillData;
    public Data.SkillData SkillData
    {
        get
        {
            return _skillData;
        }
        set
        {
            _skillData = value;
        }
    }

    public float RemainCoolTime { get; set; }
    public float SkillAnimDuration = 0;
    public InteractionObject SkillTarget { get; set; }

    protected bool _activated = true;
    public bool Activated { get { return _activated; } }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public virtual void SetInfo(int skillId)
    {
        Owner = GetComponent<Creature>();
        //Owner.SkeletonAnim.AnimationState.Event -= OnOwnerAnimEventHandler;
        //Owner.SkeletonAnim.AnimationState.Event += OnOwnerAnimEventHandler;
        //// Owner.SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
        //// Owner.SkeletonAnim.AnimationState.Complete += OnAnimCompleteHandler;
        //SkillData = Managers.Data.SkillDic[skillId];
        //RemainCoolTime = SkillData.CoolTime - Owner.CooldownReduction;
        _activated = true;

        //Owner.AEC = transform.Find("Animation").GetOrAddComponent<AnimEventController>();
        //Animation aa = Owner.GetComponent<Animation>();
        //aa.GetClip("Attack").AddEvent(new AnimationEvent{time = 0.1f,  });

        //foreach (AnimationClip clip in Owner.Anim.runtimeAnimatorController.animationClips)
        //{
        //    if (clip.name == "Jump")
        //        clip.length;
        //}
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        //    if (Managers.Game == null)
        //        return;
        //    if (Owner.IsValid() == false)
        //        return;
        //    if (Owner.SkeletonAnim == null)
        //        return;
        //    if (Owner.SkeletonAnim.AnimationState == null)
        //        return;

        //    Owner.SkeletonAnim.AnimationState.Event -= OnOwnerAnimEventHandler;
        //    // Owner.SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
        //Owner.AEC.ClearEvent();
    }

    protected void OnOwnerAnimEventHandler() { }

    public virtual void OnChangedSkillData() { }
    public virtual void CancelSkill() { }

    public virtual void DoSkill()
    {
        //RemainCoolTime = SkillData.CoolTime;
        //SkillTarget = Owner.Target;
        //준비된스킬에서 해제
        if (Activated == false)
            return;

        //공속은 기본스킬에만 적용


        Owner.Skills.CurrentSkill = null;
        //SkillAnimDuration = skill.Animation.Duration;
        //float timeScale = Owner.AttackSpeedRate;

        // PlayAnimation & Anim Time

        // Start CoolDown
        StartCoroutine(CoCooldown());
    }

    private IEnumerator CoCooldown()
    {
        RemainCoolTime = /*_skillData.CoolTime;*/ 4;
        _activated = false;
        yield return new WaitForSeconds(/*_skillData.CoolTime*/4);
        RemainCoolTime = 0;

        _activated = true;

    }
}
