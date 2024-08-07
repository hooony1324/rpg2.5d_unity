using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static UnityEngine.UI.GridLayoutGroup;

public class SkillComponent : MonoBehaviour
{
    [SerializeField] private List<SkillBase> _skillList = new List<SkillBase>();

    public List<SkillBase> SkillList
    {
        get { return _skillList; }
    }

    [SerializeField] public List<SkillBase> ReadySkills = new List<SkillBase>();

    public SkillBase EnvSkill { get; private set; }
    public SkillBase DefaultSkill { get; private set; }
    public SkillBase ASkill { get; private set; }
    public SkillBase BSkill { get; private set; }

    public SkillBase CurrentSkill;
    private Creature _owner;
    private GameObject _mesh;
    public void Awake()
    {
        _owner = GetComponent<Creature>();

        _mesh = Util.FindChild(gameObject, "Mesh");

    }

    public void AddSkill(int skillId, ESkillSlot skillSlot)
    {
        if (skillId == 0)
            return;

        string className = Managers.Data.SkillDic[skillId].ClassName;
        SkillBase skill = _mesh.gameObject.AddComponent(Type.GetType(className)) as SkillBase;

        if (!skill)
            return;

        skill.SetInfo(skillId);
        SkillList.Add(skill);
        
        switch (skillSlot)
        {
            case ESkillSlot.Default:
                DefaultSkill = skill;
                break;
            case ESkillSlot.Env:
                EnvSkill = skill;
                break;
            case ESkillSlot.A:
                ASkill = skill;
                AddReadySkill(skill);
                break;
            case ESkillSlot.B:
                BSkill = skill;
                AddReadySkill(skill);
                break;
        }
    }
    public void UpdateSkill(int skillId, ESkillSlot skillSlot)
    {
        if (skillId == 0)
            return;

        switch (skillSlot)
        {
            case ESkillSlot.Default:
                SkillList.Remove(DefaultSkill);
                ReadySkills.Remove(DefaultSkill);
                break;
            case ESkillSlot.Env:
                SkillList.Remove(EnvSkill);
                ReadySkills.Remove(EnvSkill);
                break;
            case ESkillSlot.A:
                SkillList.Remove(ASkill);
                ReadySkills.Remove(ASkill);
                break;
            case ESkillSlot.B:
                SkillList.Remove(BSkill);
                ReadySkills.Remove(BSkill);
                break;
        }

        string className = Managers.Data.SkillDic[skillId].ClassName;
        SkillBase skill = _mesh.gameObject.GetComponent(Type.GetType(className)) as SkillBase;
        Destroy(skill);

        AddSkill(skillId, skillSlot);
    }

    private void AddReadySkill(SkillBase skill)
    {
        if (skill.SkillType != ESkillType.PassiveSkill)
        {
            ReadySkills.Add(skill);
        }
    }

    public void TrySkill(ESkillSlot skillSlot)
    {
        SkillBase skill = null;
        switch (skillSlot)
        {
            case ESkillSlot.Default:
                skill = DefaultSkill;
                break;
            case ESkillSlot.Env:
                skill = EnvSkill;
                break;
            case ESkillSlot.A:
                skill = ASkill;
                break;
            case ESkillSlot.B:
                skill = BSkill;
                break;
        }

        // 스킬 등록 X
        if (skill == null)
            return;

        if (skill.Activated)
        {
            CurrentSkill = skill;
            _owner.CreatureState = ECreatureState.Skill;
        }
    }

    public void CancleCurrentSkill()
    {
        if (CurrentSkill == null)
            return;

        CurrentSkill.CancelSkill();
        CurrentSkill = null;
        _owner.CreatureState = ECreatureState.Idle;
    }

    public void CancelSkill(ESkillSlot skillSlot)
    {
        switch (skillSlot)
        {
            case ESkillSlot.A:
                if (ASkill.Activated)
                    ASkill.CancelSkill();
                break;
        }
    }

    public void Clear()
    {
        ReadySkills.Clear();
        if (ASkill != null)
            ASkill.Clear();
        if (BSkill != null)
            BSkill.Clear();
    }
}
