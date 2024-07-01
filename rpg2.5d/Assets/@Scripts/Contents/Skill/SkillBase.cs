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

    protected float SkillCoolTime = 0;      
    public float RemainCoolTime { get; set; } // SkillCoolTime + 버프
    public float SkillAnimDuration = 0;
    public InteractionObject SkillTarget { get; set; }

    protected bool _activated = true;
    public bool Activated { get { return _activated; } }
    protected LayerMask _targetMask;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public virtual void SetInfo(int skillId)
    {
        Owner = transform.parent.GetComponent<Creature>();
        //Owner.SkeletonAnim.AnimationState.Event -= OnOwnerAnimEventHandler;
        //Owner.SkeletonAnim.AnimationState.Event += OnOwnerAnimEventHandler;
        //// Owner.SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
        //// Owner.SkeletonAnim.AnimationState.Complete += OnAnimCompleteHandler;
        SkillData = Managers.Data.SkillDic[skillId];
        RemainCoolTime = SkillData.CoolTime - Owner.CooldownReduction;
        _activated = true;
        

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
    public virtual void CancelSkill() 
    {
            
    }

    public virtual void DoSkill()
    {
        //RemainCoolTime = SkillData.CoolTime;
        //SkillTarget = Owner.Target;


        //SkillAnimDuration = skill.Animation.Duration;
        //float timeScale = Owner.AttackSpeedRate;
        
        // PlayAnimation & Anim Time
        float animDuration = 1.0f;
        // animDuration / Owner.AttackSpeedRate -> 공속

        //Owner.StartWait(animDuration, OnSkillAnimEnd);

        

        StartCoolDown();
    }


    protected void StartCoolDown()
    {
        StartCoroutine(CoCooldown());
    }

    IEnumerator CoCooldown()
    {
        RemainCoolTime = 3;
        _activated = false;
        
        yield return new WaitForSeconds(3);

        CancelSkill();
        RemainCoolTime = 0;
        _activated = true;
    }

    protected List<EffectBase> ApplyEffects(InteractionObject target)
    {
        if (SkillData.EffectIds != null)
            return target.Effects.GenerateEffects(SkillData.EffectIds, EEffectSpawnType.Skill, Owner);
        return null;
    }

    public void Clear()
    {
        StopAllCoroutines();
        RemainCoolTime = SkillData.CoolTime;
    }
}
