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

    //[SerializeField] public List<SkillBase> ReadySkills = new List<SkillBase>();

    [SerializeField] public Dictionary<string, SkillBase> ReadySkills = new Dictionary<string, SkillBase>();

    //public SkillBase DodgeSkill { get; private set; }
    //public SkillBase JumpSkill { get; private set; }
    //public SkillBase AttackSkill_A { get; private set; }
    //public SkillBase AttackSkill_B { get; private set; }
    public SkillBase CurrentSkill;
    private Creature _owner;
    private GameObject _animator;
    public void Awake()
    {
        _owner = GetComponent<Creature>();

        _animator = Util.FindChild(gameObject, "Animator");

    }

    public bool TryActivateSkill(string skillName)
    {
        CurrentSkill = GetActivatedSkill(skillName);
        return CurrentSkill != null;
    }

    public SkillBase GetActivatedSkill(string skillName)
    {
        ReadySkills.TryGetValue(skillName, out SkillBase skill);
        return skill.Activated ? skill : null;
    }

    // Skill List : 배운 스킬들
    // Registered Skills : 스킬 슬롯 등록(당장 사용가능한 스킬)

    //public void RegisterSkill(int skillId)
    //{
    //    string className = Managers.Data.SkillDic[skillId].ClassName;
    //    SkillBase skill = gameObject.AddComponent(Type.GetType(className)) as SkillBase;

    //    if (!skill)
    //        return;

    //    skill.SetInfo(0);
    //    ReadySkills[className] = skill;
    //}

    // 임시, DataSheet나오면 없어질 예정
    public void RegisterSkill(string className)
    {
        // Animatinor와 같은 자식에 있어야 EventTrigger의 Receiver로 인식함
        SkillBase skill = _animator.AddComponent(Type.GetType(className)) as SkillBase;

        if (!skill)
            return;

        skill.SetInfo(0);
        ReadySkills[className] = skill;
    }

}
