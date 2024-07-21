using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;

public class Define
{
    public const char MAP_TOOL_WALL = '0';
    public const char MAP_TOOL_WALKABLE = '1';
    //public const char MAP_TOOL_SEMI_WALL = '2';

    public static readonly float[] ITEM_GRADE_PROB = new float[]
    {
        0,
        0.10f,   // Normal 확률
        0.15f,   // Rare 확률
        0.20f,   // Epic 확률
        0.55f,  // Legendary 확률
    };

    public static readonly Dictionary<Type, Array> _enumDict = new Dictionary<Type, Array>();

    public enum EScene
    {
        Unknown,
        TitleScene,
        LobbyScene,
        GameScene,

        // ArtTestScene,
    }
    public enum ECameraState
    {
        Following,
        Targeting,
    }
    public enum ESizeUnits
    {
        Byte,
        KB,
        MB,
        GB
    }

    public enum ESound
    {
        Bgm,
        SubBg,
        Effect,
        Max,
    }
    public enum UIEvent
    {
        Click,
        Preseed,
        PointerDown,
        PointerUp,
        BeginDrag,
        Drag,
        EndDrag,
    }
    public enum ELayer
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        Dummy1 = 3,
        Water = 4,
        UI = 5,
        Hero = 6,
        Monster = 7,
        Boss = 8,
        //
        Env = 11,
        Obstacle = 12,
        //
        Projectile = 20,
    }
    public enum ELanguage
    {
        Korean,
        English,
        French,
        SimplifiedChinese,
        TraditionalChinese,
        Japanese
    }

    public enum ETextType
    {
        Description,
        Message,
        Name,
    }
        
    public enum EDamageResult
    {
        None,
        Hit,
        CriticalHit,
        Miss,
        Heal,
        CriticalHeal
    }
    public static class SortingLayers
    {
        public const int SPELL_INDICATOR = 200;
        public const int HERO = 300;
        public const int NPC = 300;
        public const int MONSTER = 300;
        public const int BOSS = 300;
        public const int GATHERING_RESOURCES = 300;
        public const int PROJECTILE = 310;
        public const int DROP_ITEM = 310;
        public const int SKILL_EFFECT = 315;
        public const int WROLD_FONT = 410;
    }

    public enum ECreatureState
    {
        Idle,
        Fall,
        Move,
        Cooltime,
        Skill,
        OnDamaged,
        Death,
    }
    public enum EHeroMoveState
    {
        None,
        //TargetMonster,
        //CollectEnv,
        //ReturnToCamp,
        ForceMove,
        //ForcePath,
    }
    public enum EEnvState
    {
        Idle,
        OnDamaged,
        Dead
    }
    public enum ESkillSlot
    {
        Default,
        Env,
        A,
        B

        // Q, W, E, R, ...
    }

    public enum ESkillType
    {
        None,
        Normal,
        //AreaSkill,
        ComboSkill,
        //ProjectileSkill,
        //SingleTargetSkill,
        //SupportSkill,
        PassiveSkill
    }
    public enum EObjectType
    {
        None,
        Hero,
        Monster,
        Env,
        ItemHolder,
        Npc,
        Projectile,
    }
    public enum ECellCollisionType
    {
        None,
        Wall,
    }

    public static class AnimName
    {
        public static readonly int IDLE = Animator.StringToHash("Idle");
        public static readonly int RUN = Animator.StringToHash("Run");
        public static readonly int JUMP = Animator.StringToHash("Jump");
        public static readonly int FALL = Animator.StringToHash("Fall");
        public static readonly int TAKE_HIT = Animator.StringToHash("TakeHit");
        public static readonly int DEATH = Animator.StringToHash("Death");
        public static readonly int DODGE = Animator.StringToHash("Dodge");
        public static readonly int MELEEATTACK = Animator.StringToHash("MeleeAttack");

        public static readonly int TRIGGER_HIT = Animator.StringToHash("Hit");
    }
    public static class EquipmentUIColors
    {
        #region 장비 이름 색상
        public static readonly Color COMMON_NAME = HexToColor("3E4C68");
        public static readonly Color RARE_NAME = HexToColor("1D75E2");
        public static readonly Color EPIC_NAME = HexToColor("73438E");
        public static readonly Color LEGEND_NAME = HexToColor("C2590E");
        #endregion
        #region 테두리 색상
        public static readonly Color COMMON_OUTLINE = HexToColor("949DB3");
        public static readonly Color RARE_OUTLINE = HexToColor("6C9BF2");
        public static readonly Color EPIC_OUTLINE = HexToColor("A876C4");
        public static readonly Color LEGEND_OUTLINE = HexToColor("F19451");
        #endregion
        // #region 배경색상
        // public static readonly Color EpicBg = HexToColor("D094FF");
        // public static readonly Color LegendaryBg = HexToColor("F8BE56");
        // public static readonly Color MythBg = HexToColor("FF7F6E");
        // #endregion
    }

    public static class DirVec
    {
        public static Vector3 ZERO = Vector3.zero;
        public static Vector3 UP = new Vector3(0, 0, 1);
        public static Vector3 UP_LEFT = new Vector3(-0.894f, 0, 0.894f);
        public static Vector3 UP_RIGHT = new Vector3(0.894f, 0, 0.894f);

        public static Vector3 DOWN = new Vector3(0, 0, -1);
        public static Vector3 DOWN_LEFT = new Vector3(-0.894f, 0, -0.894f);
        public static Vector3 DOWN_RIGHT = new Vector3(0.894f, 0, -0.894f);

        public static Vector3 RIGHT = new Vector3(1, 0, 0);
        public static Vector3 LEFT = new Vector3(-1, 0, 0);
    }

    public enum EEffectType
    {
        Instant,
        Buff,
        Debuff,
        Dot,
        Infinite,
        Knockback,
        Airborne,
        Freeze,
        Stun,
        Pull,
    }
    public enum EEffectSpawnType
    {
        Skill,// 지속시간이 있는 기본적인 이펙트 
        External, // 외부(장판스킬)에서 이펙트를 관리(지속시간에 영향을 받지않음)
    }
    public enum EEffectClearType
    {
        TimeOut,// 시간초과로 인한 Effect 종료
        ClearSkill,// 정화 스킬로 인한 Effect 종료
        TriggerOutAoE,// AoE스킬을 벗어난 종료
        EndOfCC,// 에어본, 넉백이 끝난 경우 호출되는 종료
        Disable
    }
    public enum EEffectSize
    {
        CircleSmall,
        CircleNormal,
        CircleBig,
        ConeSmall,
        ConeNormal,
        ConeBig,
        Single,
    }
    public enum ECalcStatType
    {
        None,
        Default,
        SourceHp,   //상대방의 체력
        SourceAtk,  //상대방의 공격력

        Hp,
        MaxHp,
        Critical,
        CriticalDamage,
        ReduceDamageRate,
        ReduceDamage,
        LifeStealRate,
        ThornsDamageRate,   //데미지 반사

        AttackSpeedRate,
        MissChance,
        Atk,
        MoveSpeed,
        CooldownReduction,
        Thorns,

        Count,
    }

    public enum EStatModType
    {
        Add,
        PercentAdd,
        PercentMult,
    }

    public enum EItemGrade
    {
        None,
        Normal,
        Rare,
        Epic,
        Legendary
    }

    public enum EItemGroupType
    {
        None,
        Equipment,
        Consumable,
        Currency
    }

    public enum EItemType
    {
        None,
        Equipment,
        Potion,
        Scroll
    }

    public enum EItemSubType
    {
        None,
        PinkRune,
        RedRune,
        YellowRune,
        MintRune,

        //EnchantWeapon,
        //EnchantArmor,

        HealthPotion,
        ManaPotion,
    }

    public enum ECurrencyType
    {
        None,
        Gold,
        Wood,
        Mineral,
        Meat,
        Dia,
    }
    public enum EProjetionMotion
    {
        Straight,
        Parabola
    }

    public enum EEquipSlotType
    {
        None,
        Red = 1,
        Pink = 2,
        Mint = 3,
        Yellow = 4,
        EquipMax,

        Inventory = 100,
        WareHouse = 200,
    }
    public enum EBroadcastEventType
    {
        None,
        KillMonster,
        HeroLevelUp,
        DungeonClear,
        ChangeInventory,
        //ChangeTeam,
        PlayerLevelUp,
        UnlockTraining,
        ChangeCurrency,
        //ChangeCampState,
        ChangeSetting,
        HeroDead,
        QuestCompleted,     // 보상받기 누르면 발생
    }

    public enum EToastPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
    public enum EToastColor
    {
        Black,
        Red,
        Purple,
        Magenta,
        Blue,
        Green,
        Yellow,
        Orange
    }
    public enum EQuestPeriodType
    {
        Once, // 단발성
        Daily,
        Weekly,
        Infinite, // 무한으로
    }
    public enum EQuestObjectiveType
    {
        KillMonster,
        EarnMeat,
        SpendMeat,
        EarnWood,
        SpendWood,
        EarnMineral,
        SpendMineral,
        EarnGold,
        SpendGold,
        UseItem,
        Survival,
        ClearDungeon,
        Click,
    }
    public enum EQuestRewardType
    {
        Gold,
        Mineral,
        Meat,
        Wood,

        Item,

    }

    public enum EQuestState
    {
        None,
        Processing,
        Completed,
        Rewarded,
    }
}
