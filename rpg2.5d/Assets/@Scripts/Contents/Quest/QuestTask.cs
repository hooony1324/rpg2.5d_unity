using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class QuestTask
{
    public EQuestState TaskState { get; set; }
    public QuestTaskData TaskData;
    public Quest _owner;
    private int _count;
    public int Count
    {
        get => _count;
        set
        {
            if (_count != value)
            {
                _count = value;
                IsCompleted();
            }
        }
    }

    public QuestTask(int idx, QuestTaskData data, int count, EQuestState state, Quest owner)
    {
        TaskData = data;
        Count = count;
        TaskState = state;
        _owner = owner;
    }

    public bool IsCompleted()
    {
        if (TaskData.ObjectiveCount <= Count)
        {
            if (TaskState != EQuestState.Rewarded)
                TaskState = EQuestState.Completed;
            return true;
        }
        return false;
    }

    //public void GiveReward()
    //{
    //    if (TaskState == EQuestState.Rewarded)
    //        return;
    //    TaskState = EQuestState.Rewarded;

    //    switch (TaskData.RewardType)
    //    {
    //        case EQuestRewardType.Gold:
    //            Managers.Inventory.EarnCurrency(ECurrencyType.Gold, TaskData.RewardCount);
    //            break;
    //        case EQuestRewardType.Meat:
    //            Managers.Inventory.EarnCurrency(ECurrencyType.Meat, TaskData.RewardCount);
    //            break;
    //        case EQuestRewardType.Mineral:
    //            Managers.Inventory.EarnCurrency(ECurrencyType.Mineral, TaskData.RewardCount);
    //            break;
    //        case EQuestRewardType.Wood:
    //            Managers.Inventory.EarnCurrency(ECurrencyType.Wood, TaskData.RewardCount);
    //            break;
    //        case EQuestRewardType.Item:

    //            break;
    //        case EQuestRewardType.DailyScore:
    //            _owner.DailyScore += TaskData.RewardCount;
    //            break;
    //        case EQuestRewardType.WeeklyScore:
    //            _owner.WeeklyScore += TaskData.RewardCount;
    //            break;
    //    }

    //    _owner.UpdateQuest();
    //}

    public void OnHandleBroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
    {
        int absValue = Mathf.Abs(value);
        switch (TaskData.ObjectiveType)
        {
            case EQuestObjectiveType.KillMonster:
                if (eventType == EBroadcastEventType.KillMonster)
                {
                    // 1. TaskData.ObjectiveDataId가 0이면 모든 몬스터를 잡을 때마다 카운트가 증가
                    // 2. 특정 몬스터(ID가 value와 일치할 때)만 카운트가 증가
                    if (TaskData.ObjectiveDataId == 0 || TaskData.ObjectiveDataId == value)
                    {
                        Count++;
                    }
                }
                break;
            case EQuestObjectiveType.SpendMeat:
                if (currencyType == ECurrencyType.Meat && value < 0)
                {
                    Count += absValue;
                }
                break;
            case EQuestObjectiveType.EarnWood:
                if (currencyType == ECurrencyType.Wood && value > 0)
                {
                    Count += absValue;
                }
                break;
            case EQuestObjectiveType.EarnMineral:
                if (currencyType == ECurrencyType.Mineral && value > 0)
                {
                    Count += absValue;
                }
                break;
            case EQuestObjectiveType.SpendWood:
                if (currencyType == ECurrencyType.Wood && value < 0)
                {
                    Count += absValue;
                }
                break;
            case EQuestObjectiveType.SpendMineral:
                if (currencyType == ECurrencyType.Mineral && value < 0)
                {
                    Count += absValue;
                }
                break;
            case EQuestObjectiveType.SpendGold:
                if (currencyType == ECurrencyType.Gold && value < 0)
                {
                    Count += absValue;
                }
                break;
            case EQuestObjectiveType.UseItem:
                break;
            case EQuestObjectiveType.Survival:
                break;
            case EQuestObjectiveType.ClearDungeon:
                if (eventType == EBroadcastEventType.DungeonClear)
                {
                    Count += absValue;
                }
                break;
        }

    }
}
