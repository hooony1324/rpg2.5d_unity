using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class GameSaveData 
{
    public int PlayerLevel = 1;
    public int PlayerExp = 0;

    public int MaxWood = 500;
    public int MaxMineral = 500;
    public int MaxMeat = 500;

    public int ItemDbIdGenerator = 1;


    public List<ItemSaveData> Items = new List<ItemSaveData>();
    //public List<StorageSaveData> Storages = new List<StorageSaveData>();
    //public List<QuestSaveData> AllQuests = new List<QuestSaveData>();

    public int CurrentStageIndex = 0;
    public Vector3Int LastWorldPos; // 재접시 위치
}


[Serializable]
public class ItemSaveData
{
    public int InstanceId;
    public int DbId;
    public int TemplateId;
    public int Count;
    public int EquipSlot; // 장착 + 인벤 + 창고
    public int EnchantCount; 
    public bool IsLock;
    public List<int> OptionIds = new List<int>();
}

//[Serializable]
//public class QuestSaveData
//{
//    public int TemplateId;
//    public EQuestState State = EQuestState.Processing;
//    public List<int> TaskProgressCount = new List<int>();//Task의 Count
//    public List<EQuestState> TaskStates = new List<EQuestState>();//Task의 Count
//    public DateTime NextResetTime;
//    public int DailyScore;
//    public int WeeklyScore;
//}

//[Serializable]
//public class StorageSaveData
//{
//    public int TemplateId;
//    public DateTime LastRewardTime;
//    public int StoredResources;
//}
