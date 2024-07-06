using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<string, Data.TextData> TextDic { get; private set; } = new Dictionary<string, Data.TextData>();
    public Dictionary<int, Data.HeroData> HeroDic { get; private set; } = new Dictionary<int, Data.HeroData>();
    public Dictionary<int, Data.MonsterData> MonsterDic { get; private set; } = new Dictionary<int, Data.MonsterData>();
    public Dictionary<int, Data.SkillData> SkillDic { get; private set; } = new Dictionary<int, Data.SkillData>();
    public Dictionary<int, Data.EffectData> EffectDic { get; private set; } = new Dictionary<int, Data.EffectData>();
    public Dictionary<int, Data.DropTableData> DropTableDic { get; private set; } = new Dictionary<int, Data.DropTableData>();
    public Dictionary<int, Data.ItemData> ItemDic { get; private set; } = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.EquipmentData> EquipmentDic { get; private set; } = new Dictionary<int, Data.EquipmentData>();
    public Dictionary<int, Data.ConsumableData> ConsumableDic { get; private set; } = new Dictionary<int, Data.ConsumableData>();
    public Dictionary<int, Data.CurrencyData> CurrencyDic { get; private set; } = new Dictionary<int, Data.CurrencyData>();
    public Dictionary<int, Data.EquipmentOptionData> EquipmentOptionDic { get; private set; } = new Dictionary<int, Data.EquipmentOptionData>();
    public Dictionary<int, Data.HeroLevelData> HeroLevelDic { get; private set; } = new Dictionary<int, Data.HeroLevelData>();
    public Dictionary<int, Data.PlayerLevelData> PlayerLevelDic { get; private set; } = new Dictionary<int, Data.PlayerLevelData>();

    public void Init()
    {
        HeroDic = LoadJson<Data.HeroDataLoader, int, Data.HeroData>("HeroData").MakeDict();
        MonsterDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        SkillDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
        EffectDic = LoadJson<Data.EffectDataLoader, int, Data.EffectData>("EffectData").MakeDict();

        DropTableDic = LoadJson<Data.DropTableDataLoader, int, Data.DropTableData>("DropTableData").MakeDict();
        EquipmentDic = LoadJson<Data.ItemDataLoader<Data.EquipmentData>, int, Data.EquipmentData>("Item_EquipmentData").MakeDict();
        ConsumableDic = LoadJson<Data.ItemDataLoader<Data.ConsumableData>, int, Data.ConsumableData>("Item_ConsumableData").MakeDict();
        CurrencyDic = LoadJson<Data.ItemDataLoader<Data.CurrencyData>, int, Data.CurrencyData>("Item_CurrencyData").MakeDict();

        EquipmentOptionDic = LoadJson<Data.EquipmentOptionDataLoader, int, Data.EquipmentOptionData>("EquipmentOptionData").MakeDict();
        HeroLevelDic = LoadJson<Data.HeroLevelDataLoader, int, Data.HeroLevelData>("HeroLevelData").MakeDict();
        PlayerLevelDic = LoadJson<Data.PlayerLevelDataLoader, int, Data.PlayerLevelData>("PlayerLevelData").MakeDict();

        ItemDic.Clear();
        foreach (var item in EquipmentDic)
            ItemDic.Add(item.Key, item.Value);

        foreach (var item in ConsumableDic)
            ItemDic.Add(item.Key, item.Value);

        foreach (var item in CurrencyDic)
            ItemDic.Add(item.Key, item.Value);

        TextDic.AddRange(LoadJson<Data.TextDataLoader, string, Data.TextData>("Text_NameData").MakeDict());
        TextDic.AddRange(LoadJson<Data.TextDataLoader, string, Data.TextData>("Text_DescriptionData").MakeDict());
        TextDic.AddRange(LoadJson<Data.TextDataLoader, string, Data.TextData>("Text_MessageData").MakeDict());

    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        Debug.Log($"{path}");
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }


    #region HeroInfoData
    //[Serializable]
    //public class HeroInfoData
    //{
    //    public int DataId;
    //    public string DescriptionTextId;
    //    public string Rarity;
    //    public float HireSpawnWeight;
    //    public float GachaWeight;
    //    public string IconImage;
    //}

    //[Serializable]
    //public class HeroInfoDataLoader : ILoader<int, HeroInfoData>
    //{
    //    public List<HeroInfoData> heroInfo = new List<HeroInfoData>();
    //    public Dictionary<int, HeroInfoData> MakeDict()
    //    {
    //        Dictionary<int, HeroInfoData> dict = new Dictionary<int, HeroInfoData>();
    //        foreach (HeroInfoData info in heroInfo)
    //            dict.Add(info.DataId, info);
    //        return dict;
    //    }
    //}
    #endregion

}
