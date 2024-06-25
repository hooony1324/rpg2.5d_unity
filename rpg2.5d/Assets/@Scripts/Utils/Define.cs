using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public const char MAP_TOOL_WALL = '0';
    public const char MAP_TOOL_WALKABLE = '1';
    //public const char MAP_TOOL_SEMI_WALL = '2';


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
        Skill,// ���ӽð��� �ִ� �⺻���� ����Ʈ 
        External, // �ܺ�(���ǽ�ų)���� ����Ʈ�� ����(���ӽð��� ������ ��������)
    }
    public enum EEffectClearType
    {
        TimeOut,// �ð��ʰ��� ���� Effect ����
        ClearSkill,// ��ȭ ��ų�� ���� Effect ����
        TriggerOutAoE,// AoE��ų�� ��� ����
        EndOfCC,// ���, �˹��� ���� ��� ȣ��Ǵ� ����
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
        SourceHp,//������ ü��
        SourceAtk,//������ ���ݷ�

        Hp,
        MaxHp,
        Critical,
        CriticalDamage,
        ReduceDamageRate,
        ReduceDamage,
        LifeStealRate,
        ThornsDamageRate,

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
}
