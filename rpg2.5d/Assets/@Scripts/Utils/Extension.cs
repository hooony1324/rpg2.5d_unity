using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static void BindEvent(this GameObject go, Action action = null, Action<PointerEventData> dragAction = null, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, dragAction, type);
    }

    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }

    public static bool IsValid(this BaseObject bc)
    {
        if (bc == null || bc.isActiveAndEnabled == false)
            return false;

        switch (bc.ObjectType)
        {
            case Define.EObjectType.Monster:
            case Define.EObjectType.Hero:
                return ((Creature)bc).CreatureState != Define.ECreatureState.Death;

            case Define.EObjectType.Env:
                return ((Env)bc).EnvState != Define.EEnvState.Dead;

        }
        return true;
    }

    public static void MakeMask(this ref LayerMask mask, List<Define.ELayer> list)
    {
        foreach (Define.ELayer layer in list)
            mask |= (1 << (int)layer);
    }

    public static void AddLayer(this ref LayerMask mask, Define.ELayer layer)
    {
        mask |= (1 << (int)layer);
    }

    public static void RemoveLayer(this ref LayerMask mask, Define.ELayer layer)
    {
        mask &= ~(1 << (int)layer);
    }

    public static bool ContainsLayer(this ref LayerMask mask, Define.ELayer layer)
    {
        return (mask & (1 << (int)layer)) != 0;
    }

    public static bool ContainsLayer(this ref LayerMask mask, int layerIndex)
    {
        return (mask & (1 << layerIndex)) != 0;
    }

    public static void DestroyChilds(this GameObject go)
    {
        Transform[] children = new Transform[go.transform.childCount];
        for (int i = 0; i < go.transform.childCount; i++)
        {
            children[i] = go.transform.GetChild(i);
        }

        // 모든 자식 오브젝트 삭제
        foreach (Transform child in children)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);//swap
        }
    }

    public static bool EqualsEx(this Vector3 origin, Vector3 dest)
    {
        return origin.IsTargetInRange(dest, float.Epsilon);
    }
    public static bool IsTargetInRange(this Vector3 origin, Vector3 dest, float range)
    {// 일정 거리 안에 있나
        return Vector3.SqrMagnitude(dest - origin) <= range * range;
    }

    public static void Initialize<T>(this T[,] array, T initVal)
    {
        for (int j = 0; j < array.GetLength(0); j++)
        {
            for (int i = 0; i < array.GetLength(1); i++)
            {
                array[j, i] = initVal;
            }
        }
    }

}
