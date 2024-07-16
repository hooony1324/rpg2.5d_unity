using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Define;

public class QuestManager
{
    public const int DEFAULT_QUEST_SLOT_COUNT = 4;

    public Dictionary<int /*DataId*/, Quest> AllQuests = new Dictionary<int, Quest>();
    public List<Quest> CompletedOrProcessingQuests => AllQuests.Values.Where(quest => (quest.State == EQuestState.Processing) || (quest.State == EQuestState.Completed)).ToList();

    
    public Quest MainQuest
    {
        get
        {
            int mainQuestId = 8;
            return AllQuests[mainQuestId];
        }
    }

    public void Init()
    {
        Managers.Game.OnBroadcastEvent -= OnHandleBroadcastEvent;
        Managers.Game.OnBroadcastEvent += OnHandleBroadcastEvent;
    }
    void OnHandleBroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
    {
        foreach (Quest quest in AllQuests.Values)
        {
            if (quest.State == EQuestState.Processing)
                quest.OnHandleBroadcastEvent(eventType, currencyType, value);
        }
    }


    public void CheckProcessingQuests()
    {
        foreach (Quest quest in AllQuests.Values)
        {
            if (quest.State == EQuestState.Processing)
                quest.UpdateTasksData();
        }
    }

    public void Clear()
    {
        AllQuests.Clear();
    }


    public Quest GetQuest(int templateId)
    {
        if (AllQuests.TryGetValue(templateId, out Quest quest))
        {
            return quest;
        }

        return null;
    }

    // 재접속 시 유효한 퀘스트 확인하여 추가
    public void AddUnknownQuests()
    {
        foreach (QuestData questData in Managers.Data.QuestDic.Values.ToList())
        {
            if (AllQuests.ContainsKey(questData.TemplateId))
                continue;

            QuestSaveData questSaveData = new QuestSaveData()
            {
                TemplateId = questData.TemplateId,
                State = Define.EQuestState.None,
                NextResetTime = DateTime.MaxValue,
            };

            for (int i = 0; i < questData.QuestTasks.Count; i++)
                questSaveData.TaskProgressCount.Add(0);

            AddQuest(questSaveData);
        }
    }
    public Quest AddQuest(QuestSaveData questInfo)
    {
        Quest quest = Quest.MakeQuest(questInfo);
        if (quest == null)
            return null;

        AllQuests.Add(quest.TemplateId, quest);

        return quest;
    }
}
