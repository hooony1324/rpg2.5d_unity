using System;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;
using Transform = UnityEngine.Transform;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static T FindAncestor<T>(GameObject go) where T : UnityEngine.Object
    {
        Transform t = go.transform;
        while (t != null)
        {
            T component = t.GetComponent<T>();
            if (component != null)
                return component;
            t = t.parent;
        }
        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static Color HexToColor(string color)
    {
        Color parsedColor;

        if (color.Contains("#") == false)
            ColorUtility.TryParseHtmlString("#" + color, out parsedColor);
        else
            ColorUtility.TryParseHtmlString(color, out parsedColor);

        return parsedColor;
    }

    // Animator ������Ʈ ���� Ư�� �ִϸ��̼� Ŭ���� �����ϴ��� Ȯ���ϴ� �Լ�
    public static bool HasAnimationClip(Animator animator, string clipName)
    {
        if (animator.runtimeAnimatorController == null)
        {
            return false;
        }

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return true;
            }
        }

        return false;
    }

    //Enum���� ������ ��ȯ
    public static T GetRandomEnumValue<T>() where T : struct, Enum
    {
        Type type = typeof(T);

        if (!_enumDict.ContainsKey(type))
            _enumDict[type] = Enum.GetValues(type);

        Array values = _enumDict[type];

        int index = UnityEngine.Random.Range(0, values.Length);
        return (T)values.GetValue(index);
    }

    //public static EItemGrade ChooseItemGrade()
    //{
    //    float randValue = UnityEngine.Random.value;

    //    float cumulative = 0.0f;
    //    for (int i = 0; i < ITEM_GRADE_PROB.Length; i++)
    //    {
    //        cumulative += ITEM_GRADE_PROB[i];
    //        if (randValue < cumulative)
    //        {
    //            return (EItemGrade)i;
    //        }
    //    }

    //    return EItemGrade.None;
    //}

    //string�� ���� Enum�� ã��
    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static Vector2 GenerateRandomPositionOnCircle(Vector2 center, float radius)
    {
        int randomUnit = Random.Range(0, 10);
        float randomAngle = randomUnit * 36f;
        float radians = Mathf.Deg2Rad * randomAngle;
        float x = center.x + radius * Mathf.Cos(radians);
        float y = center.y + radius * Mathf.Sin(radians);

        return new Vector2(x, y);
    }

    //public static EObjectType DetermineTargetType(EObjectType ownerObjectType, bool isAllies)
    //{
    //    if (ownerObjectType == Define.EObjectType.Hero)
    //    {
    //        return isAllies ? EObjectType.Hero : EObjectType.Monster;
    //    }
    //    else if (ownerObjectType == Define.EObjectType.Monster)
    //    {
    //        return isAllies ? EObjectType.Monster : EObjectType.Hero;
    //    }

    //    return EObjectType.None;
    //}

    //public static float GetEffectRadius(EEffectSize size)
    //{
    //    switch (size)
    //    {
    //        case EEffectSize.CircleSmall:
    //            return EFFECT_SMALL_RADIUS;
    //        case EEffectSize.CircleNormal:
    //            return EFFECT_NORMAL_RADIUS;
    //        case EEffectSize.CircleBig:
    //            return EFFECT_BIG_RADIUS;
    //        case EEffectSize.ConeSmall:
    //            return EFFECT_SMALL_RADIUS * 2f;
    //        case EEffectSize.ConeNormal:
    //            return EFFECT_NORMAL_RADIUS * 2f;
    //        case EEffectSize.ConeBig:
    //            return EFFECT_BIG_RADIUS * 2f;
    //        default:
    //            return 0.5f;
    //    }
    //}

    //public static int GetAngleRange(EEffectSize size)
    //{
    //    if ((int)size <= 2) //0,1,2
    //    {
    //        return 360;
    //    }
    //    else
    //    {
    //        return 90;
    //    }
    //}

    public static bool CheckProbability(float probability)
    {
        float randomValue = Random.Range(0.0f, 1.0f);
        return randomValue <= probability;
    }

    #region ParseText For UI

    //public static string ParseEquipOptionValue(ECalcStatType type, EStatModType modType, float value)
    //{
    //    switch (type)
    //    {
    //        case ECalcStatType.Critical:
    //        case ECalcStatType.CriticalDamage:
    //        case ECalcStatType.LifeStealRate:
    //        case ECalcStatType.ThornsDamageRate:
    //        case ECalcStatType.ReduceDamageRate:
    //            return $"+{value * 100}%";
    //        case ECalcStatType.AttackSpeedRate:
    //        case ECalcStatType.MissChance:
    //        case ECalcStatType.MoveSpeed:
    //        case ECalcStatType.Thorns:
    //            return $"+{value}%";
    //        case ECalcStatType.ReduceDamage:
    //        case ECalcStatType.CooldownReduction:
    //            return $"+{value}";
    //        case ECalcStatType.Atk:
    //        case ECalcStatType.MaxHp:
    //            if (modType == EStatModType.Add)
    //            {
    //                return $"+{value}";
    //            }
    //            else
    //            {
    //                return $"+{value}%";
    //            }
    //        case ECalcStatType.SourceHp:
    //        case ECalcStatType.SourceAtk:
    //        case ECalcStatType.Hp:
    //        case ECalcStatType.Count:
    //        case ECalcStatType.Default:
    //        default:
    //            return $"+{value}%";

    //    }
    //}

    //public static Color GetTextColor(EItemGrade type)
    //{
    //    switch (type)
    //    {
    //        case EItemGrade.None:
    //        case EItemGrade.Normal:
    //            return EquipmentUIColors.COMMON_NAME;
    //        case EItemGrade.Rare:
    //            return EquipmentUIColors.RARE_NAME;
    //        case EItemGrade.Epic:
    //            return EquipmentUIColors.EPIC_NAME;
    //        case EItemGrade.Legendary:
    //            return EquipmentUIColors.LEGEND_NAME;
    //        default:
    //            return EquipmentUIColors.COMMON_NAME;
    //    }
    //}

    //public static Color GetOutlineColor(EItemGrade type)
    //{
    //    switch (type)
    //    {
    //        case EItemGrade.None:
    //        case EItemGrade.Normal:
    //            return EquipmentUIColors.COMMON_OUTLINE;
    //        case EItemGrade.Rare:
    //            return EquipmentUIColors.RARE_OUTLINE;
    //        case EItemGrade.Epic:
    //            return EquipmentUIColors.EPIC_OUTLINE;
    //        case EItemGrade.Legendary:
    //            return EquipmentUIColors.LEGEND_OUTLINE;
    //        default:
    //            return EquipmentUIColors.COMMON_OUTLINE;
    //    }
    //}

    #endregion

    #region Size

    public static long OneGB = 1000000000;
    public static long OneMB = 1000000;
    public static long OneKB = 1000;

    /// <summary> ����Ʈ <paramref name="byteSize"/> ����� �°Բ� ������ ���� <see cref="ESizeUnits"/> Ÿ���� �����´� </summary>
    public static ESizeUnits GetProperByteUnit(long byteSize)
    {
        if (byteSize >= OneGB)
            return ESizeUnits.GB;
        else if (byteSize >= OneMB)
            return ESizeUnits.MB;
        else if (byteSize >= OneKB)
            return ESizeUnits.KB;
        return ESizeUnits.Byte;
    }

    /// <summary> ����Ʈ�� <paramref name="byteSize"/> <paramref name="unit"/> ������ �°� ���ڸ� ��ȯ�Ѵ� </summary>
    public static long ConvertByteByUnit(long byteSize, ESizeUnits unit)
    {
        return (long)((byteSize / (double)System.Math.Pow(1024, (long)unit)));
    }

    /// <summary> ����Ʈ�� <paramref name="byteSize"/> ������ �Բ� ����� ������ ���ڿ� ���·� ��ȯ�Ѵ� </summary>
    public static string GetConvertedByteString(long byteSize, ESizeUnits unit, bool appendUnit = true)
    {
        string unitStr = appendUnit ? unit.ToString() : string.Empty;
        return $"{ConvertByteByUnit(byteSize, unit).ToString("0.00")}{unitStr}";
    }

    #endregion
}