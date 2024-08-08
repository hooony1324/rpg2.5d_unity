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
        SkillData = Managers.Data.SkillDic[skillId];
        RemainCoolTime = SkillData.CoolTime - Owner.CooldownReduction;
        _activated = true;
       
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected void OnOwnerAnimEventHandler() { }

    public virtual void OnChangedSkillData() { }
    public virtual void CancelSkill() 
    {
            
    }

    public virtual void DoSkill()
    {
        
        RemainCoolTime = SkillData.CoolTime;

        Owner.Skills.CurrentSkill = null;

        StartCoolDown();

    }


    protected void StartCoolDown()
    {
        StartCoroutine(CoCooldown());
    }

    IEnumerator CoCooldown()
    {
        _activated = false;
        
        yield return new WaitForSeconds(RemainCoolTime);

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
