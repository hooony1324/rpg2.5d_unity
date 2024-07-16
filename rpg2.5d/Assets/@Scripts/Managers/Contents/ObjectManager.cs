using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
public class ObjectManager
{

    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public Hero Hero { get; private set; }
    public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
    //public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();
    public HashSet<Env> Envs { get; } = new HashSet<Env>();
    //public HashSet<Npc> Npcs { get; } = new HashSet<Npc>();
    public HashSet<ItemHolder> ItemHolders { get; } = new HashSet<ItemHolder>();

    public Transform HeroRoot { get { return GetRootTransform("@Heroes"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    //public Transform ProjectileRoot { get { return GetRootTransform("@Projectiles"); } }
    public Transform EnvRoot { get { return GetRootTransform("@Envs"); } }
    //public Transform EffectRoot { get { return GetRootTransform("@Effects"); } }
    //public Transform NpcRoot { get { return GetRootTransform("@Npc"); } }
    //public Transform ItemHolderRoot { get { return GetRootTransform("@ItemHolders"); } }

    public void LoadMap(string mapName)
    {
        GameObject objMap = Managers.Resource.Instantiate(mapName);
        objMap.transform.position = Vector3.zero;
        objMap.name = "@Map";
    }

    public void ShowDamageFont(Vector3 pos, float damage, Transform parent, EDamageResult result)
    {
        string prefabName = "DamageFont";

        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        DamageFont damageText = go.GetComponent<DamageFont>();
        damageText.SetInfo(pos, damage, parent, result);
    }

    public T Spawn<T>(object position, int templateID = 0, string prefabName = "") where T : BaseObject
    {
        System.Type type = typeof(T);

        //Vector3 spawnPos = new Vector3();
        //if (position is Vector3)
        //{
        //    spawnPos = (Vector3)position;
        //}
        //else if (position is Vector3Int)
        //{
        //    spawnPos = Managers.Map.CellGrid.GetCellCenterWorld((Vector3Int)position);
        //}
        Vector3 spawnPos = (Vector3)position;

        if (type == typeof(Hero))
        {
            GameObject go = Managers.Resource.Instantiate("HeroPrefab");
            string textId = Managers.Data.HeroDic[templateID].TextID;
            go.name = Managers.GetText(textId, ETextType.Name);
            go.transform.position = spawnPos;
            go.transform.parent = HeroRoot;
            Hero hc = go.GetOrAddComponent<Hero>();
            hc.SetInfo(templateID);
            Hero = hc;
            return hc as T;
        }
        if (type == typeof(Env))
        {
            //GameObject go = Managers.Resource.Instantiate(Managers.Data.EnvDic[templateID].PrefabLabel,
            //    pooling: true);

            GameObject go = Managers.Resource.Instantiate("EnvPrefab");

            go.transform.position = spawnPos;
            go.transform.parent = EnvRoot;
            Env gr = go.GetOrAddComponent<Env>();
            Envs.Add(gr);
            gr.SetInfo(templateID);
            return gr as T;
        }
        //if (type == typeof(Npc))
        //{
        //    GameObject go = Managers.Resource.Instantiate(Managers.Data.NpcDic[templateID].PrefabLabel, pooling: true);
        //    go.transform.position = spawnPos;
        //    go.transform.parent = NpcRoot;
        //    Npc npc = go.AddComponent<Npc>();
        //    Npcs.Add(npc);
        //    npc.SetInfo(templateID);

        //    return npc as T;
        //}
        if (type == typeof(Monster))
        {
            GameObject go = Managers.Resource.Instantiate("MonsterPrefab", pooling: true);
            go.transform.position = spawnPos;
            go.transform.parent = MonsterRoot;
            Monster mc = go.GetOrAddComponent<Monster>();
            Monsters.Add(mc);
            mc.SetInfo(templateID);
            return mc as T;
        }
        if (type == typeof(ItemHolder))
        {
            GameObject go = Managers.Resource.Instantiate("ItemHolder", pooling: true);
            go.transform.position = spawnPos;
            ItemHolder itemHolder = go.GetOrAddComponent<ItemHolder>();
            ItemHolders.Add(itemHolder);
            return itemHolder as T;
        }
        //if (type == typeof(AoEBase))
        //{
        //    GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        //    go.transform.position = spawnPos;
        //    AoEBase aoe = go.GetOrAddComponent<AoEBase>();
        //    go.transform.parent = ProjectileRoot;
        //    return aoe as T;
        //}
        //if (type == typeof(HeroCamp))
        //{
        //    GameObject go = Managers.Resource.Instantiate("CampPrefab");
        //    go.transform.position = spawnPos;
        //    go.transform.parent = ProjectileRoot;
        //    go.name = "***CampPoint***";
        //    HeroCamp = go.GetOrAddComponent<HeroCamp>();
        //    return HeroCamp as T;
        //}

        return null;
    }

    public void Despawn<T>(T obj) where T : BaseObject
    {
        System.Type type = typeof(T);

        if (type == typeof(Hero))
        {
            //Heroes.Remove(obj as Hero);
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(Monster))
        {
            Monsters.Remove(obj as Monster);
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(Env))
        {
            Envs.Remove(obj as Env);
            Managers.Resource.Destroy(obj.gameObject);
        }
        //else if (type == typeof(Npc))
        //{
        //    Npcs.Remove(obj as Npc);
        //    Managers.Resource.Destroy(obj.gameObject);
        //}
        else if (type == typeof(InteractionObject))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(ItemHolder))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }

        //Managers.Map.RemoveObject(obj);
    }

    public GameObject SpawnGameObject(Vector3 position, string prefabName)
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;

        return go;
    }

    public void DespawnGameObject<T>(T obj) where T : BaseObject
    {
        System.Type type = typeof(T);

        if (typeof(EffectBase).IsAssignableFrom(type))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
        //else if (typeof(AoEBase).IsAssignableFrom(type))
        //{
        //    Managers.Resource.Destroy(obj.gameObject);
        //}
    }
}
