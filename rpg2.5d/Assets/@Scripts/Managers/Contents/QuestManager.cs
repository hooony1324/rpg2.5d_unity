using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Define;

public class QuestManager
{
    public const int DEFAULT_QUEST_SLOT_COUNT = 4;

    public Dictionary<int /*DataId*/, Quest> AllQuests = new Dictionary<int, Quest>();
    public List<Quest> ProcessingQuests => AllQuests.Values.Where(quest => quest.State == EQuestState.Processing).ToList();

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

    public Quest AddQuest(QuestSaveData questInfo)
    {
        Quest quest = Quest.MakeQuest(questInfo);
        if (quest == null)
            return null;

        AllQuests.Add(quest.TemplateId, quest);

        return quest;
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
}
