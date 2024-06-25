using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public void Init()
    {
        TextDic = LoadJson<Data.TextDataLoader, string, Data.TextData>("TextData").MakeDict();
        HeroDic = LoadJson<Data.HeroDataLoader, int, Data.HeroData>("HeroData").MakeDict();
        MonsterDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        SkillDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
        EffectDic = LoadJson<Data.EffectDataLoader, int, Data.EffectData>("EffectData").MakeDict();
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
