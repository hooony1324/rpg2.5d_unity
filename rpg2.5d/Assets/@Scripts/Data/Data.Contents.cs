using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace Data
{
    #region TextData
    [Serializable]
    public class TextData
    {
        public string DataId;
        public string KOR;
    }
    [Serializable]
    public class TextDataLoader : ILoader<string, TextData>
    {
        public List<TextData> texts = new List<TextData>();
        public Dictionary<string, TextData> MakeDict()
        {
            Dictionary<string, TextData> dict = new Dictionary<string, TextData>();
            foreach (TextData text in texts)
                dict.Add(text.DataId, text);
            return dict;
        }
    }
    #endregion

    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int TemplateId;
        public string Description;
        public string DescriptionTextID;
        public string AnimationControllerName;
        public float ColliderOffsetX;
        public float ColliderOffsetY;
        public float ColliderRadius;
        public float MaxHp;
        public float UpMaxHpBonus;
        public float Atk;
        public float MissChance;
        public float AtkBonus;
        public float MoveSpeed;
        public float CriRate;
        public float CriDamage;
        public string IconImage;

        //todo...
        public int DefaultSkillId;
        public int EnvSkillId;
        public int SkillAId;
        public int SkillBId;
    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> creatures = new List<CreatureData>();
        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (CreatureData creature in creatures)
                dict.Add(creature.TemplateId, creature);
            return dict;
        }
    }
    #endregion

    #region HeroData
    [Serializable]
    public class HeroData : CreatureData
    {
    }

    [Serializable]
    public class HeroDataLoader : ILoader<int, HeroData>
    {
        public List<HeroData> creatures = new List<HeroData>();
        public Dictionary<int, HeroData> MakeDict()
        {
            Dictionary<int, HeroData> dict = new Dictionary<int, HeroData>();
            foreach (HeroData creature in creatures)
                dict.Add(creature.TemplateId, creature);
            return dict;
        }
    }
    #endregion

    #region MonsterData
    [Serializable]
    public class MonsterData : CreatureData
    {
        public int DropItemId;
        public int MonsterSize;
    }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> creatures = new List<MonsterData>();
        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData creature in creatures)
                dict.Add(creature.TemplateId, creature);
            return dict;
        }
    }
    #endregion

    #region SkillData
    [Serializable]
    public class SkillData
    {
        public int TemplateId;
        public string Name;
        public string NameTextId;
        public string ClassName;
        public string Description;
        public string DescriptionTextId;
        public int ProjectileId;
        public string IconLabel;
        public string AnimName;
        public float CoolTime;
        public float Duration;
        public string CastingAnimname;
        public string CastingSound;
        public float SkillRange;
        public int TargetCount;
        public List<int> EffectIds = new List<int>();
        public int NextLevelId;
        public int AoEId;
        public EEffectSize EffectSize;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.TemplateId, skill);
            return dict;
        }
    }
    #endregion

    #region EffectData
    [Serializable]
    public class EffectData
    {
        public int DataId;
        public string Name;
        public string Description;
        public string DescriptionTextID;
        public string PrefabName;
        public string IconLabel;
        public string SoundLabel;
        public float Amount;
        public float PercentAdd;
        public float PercentMult;
        public float TickTime;
        public float TickCount;
        public EEffectType EffectType;
        public ECalcStatType CalcStatType;
    }
    [Serializable]
    public class EffectDataLoader : ILoader<int, EffectData>
    {
        public List<EffectData> effects = new List<EffectData>();
        public Dictionary<int, EffectData> MakeDict()
        {
            Dictionary<int, EffectData> dict = new Dictionary<int, EffectData>();
            foreach (EffectData effect in effects)
                dict.Add(effect.DataId, effect);
            return dict;
        }
    }
    #endregion

    #region DropTable
    [Serializable]
    public class RewardData
    {
        public int ItemTemplateId;
        public int Probability; // 100ºÐÀ²
        public int Count;
    }

    [Serializable]
    public class DropTableData
    {
        public int TemplateId;
        public int RewardExp;
        public string Description;
        public List<RewardData> Rewards = new List<RewardData>();
    }

    [Serializable]
    public class DropTableDataLoader : ILoader<int, DropTableData>
    {
        public List<DropTableData> dropTables = new List<DropTableData>();

        public Dictionary<int, DropTableData> MakeDict()
        {
            Dictionary<int, DropTableData> dict = new Dictionary<int, DropTableData>();

            foreach (DropTableData tempData in dropTables)
            {
                dict.Add(tempData.TemplateId, tempData);
            }

            return dict;
        }
    }
    #endregion
    #region Item

    [Serializable]
    public class BaseData
    {
        public int DataId;
        public string Description;
        public string DescriptionTextID;
        public string SpriteName;
    }

    [Serializable]
    public class ItemData : BaseData
    {
        public string Name;
        public EItemGroupType ItemGroupType;
        public EItemType Type;
        public EItemSubType SubType;
        public EItemGrade Grade;
        public int MaxStack;
    }

    [Serializable]
    public class EquipmentData : ItemData
    {
        public int ItemLevel;
        public ECalcStatType CalcStatType;
        public EStatModType StatModType;
        public int BonusValue;
        public int SubOptionCount;
    }

    [Serializable]
    public class ConsumableData : ItemData
    {
        public double Value;
        public int CoolTime;
    }

    [Serializable]
    public class CurrencyData : ItemData
    {
        public ECurrencyType currencyType;
    }

    [Serializable]
    public class ItemDataLoader<T> : ILoader<int, T> where T : BaseData
    {
        public List<T> items = new List<T>();

        public Dictionary<int, T> MakeDict()
        {
            Dictionary<int, T> dict = new Dictionary<int, T>();
            foreach (T item in items)
                dict.Add(item.DataId, item);

            return dict;
        }
    }
    #endregion

    #region ItemOption

    [Serializable]
    public class EquipmentOptionData
    {
        public int DataId;
        public string Name;
        public int ItemLevel;
        public EItemGrade OptionGrade;

        public ECalcStatType CalcStatType;
        public EStatModType StatModType;
        public float OptionValue;
    }

    [Serializable]
    public class EquipmentOptionDataLoader : ILoader<int, EquipmentOptionData>
    {
        public List<EquipmentOptionData> datas = new List<EquipmentOptionData>();
        public Dictionary<int, EquipmentOptionData> MakeDict()
        {
            Dictionary<int, EquipmentOptionData> dict = new Dictionary<int, EquipmentOptionData>();
            foreach (EquipmentOptionData data in datas)
                dict.Add(data.DataId, data);
            return dict;
        }
    }

    #endregion

    #region ProjectileData
    //HitSound	ProjRange	ProjSpeed

    [Serializable]
    public class ProjectileData
    {
        public int DataId;
        public string Name;
        public EProjetionMotion ProjectileMotion;
        public string SpriteName;
        public string SpineName;
        public float Duration;
        public float HitSound;
        public float ProjRange;
        public float ProjSpeed;
        public int PenetrateCount;
    }
    [Serializable]
    public class ProjectileDataLoader : ILoader<int, ProjectileData>
    {
        public List<ProjectileData> projectiles = new List<ProjectileData>();

        public Dictionary<int, ProjectileData> MakeDict()
        {
            Dictionary<int, ProjectileData> dict = new Dictionary<int, ProjectileData>();
            foreach (ProjectileData projectile in projectiles)
                dict.Add(projectile.DataId, projectile);
            return dict;
        }
    }
    #endregion
}
