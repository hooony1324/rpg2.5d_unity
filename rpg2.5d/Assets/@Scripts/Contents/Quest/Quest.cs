using Data;
using System;
using System.Collections.Generic;
using static Define;

public class Quest
{
    public QuestSaveData SaveData { get; set; }
    public QuestData QuestData;


    private List<QuestTask> _questTasks = new List<QuestTask>();
    public List<QuestTask> QuestTasks => _questTasks;

    private List<QuestRewardData> _questRewards = new List<QuestRewardData>();
    public List<QuestRewardData> QuestRewards => _questRewards;
    public event Action<Quest> OnQuestCompleted;
    public int TemplateId
    {
        get { return SaveData.TemplateId; }
        set { SaveData.TemplateId = value; }
    }

    public EQuestState State
    {
        get { return SaveData.State; }
        set { SaveData.State = value; }
    }
    public Quest(QuestSaveData saveData)
    {
        SaveData = saveData;
        State = saveData.State;
        QuestData = Managers.Data.QuestDic[TemplateId];

        _questTasks.Clear();
        for (int i = 0; i < QuestData.QuestTasks.Count; i++)
        {
            _questTasks.Add(new QuestTask(i, QuestData.QuestTasks[i], saveData.TaskProgressCount[i], saveData.TaskStates[i], this));
        }

        int rewardTableId = QuestTasks[0].TaskData.QuestRewardId;
        _questRewards = Managers.Data.QuestRewardDic[rewardTableId].Rewards;
    }

    public bool IsTasksCompleted()
    {
        for (int i = 0; i < QuestData.QuestTasks.Count; i++)
        {
            if (i >= SaveData.TaskProgressCount.Count)
                return false;

            QuestTaskData questTaskData = QuestData.QuestTasks[i];

            int progressCount = SaveData.TaskProgressCount[i];
            if (progressCount < questTaskData.ObjectiveCount)
                return false;
        }

        State = EQuestState.Completed;
        return true;
    }

    public void StartQuest()
    {
        State = EQuestState.Processing;

        for (int i = 0; i < _questTasks.Count; i++)
        {
            if (_questTasks[i].TaskState == EQuestState.None)
                _questTasks[i].TaskState = EQuestState.Processing;
        }
    }

    public List<QuestTask> GetProcessingTasks()
    {
        List<QuestTask> tasks = new List<QuestTask>();

        for (int i = 0; i < _questTasks.Count; i++)
        {
            if (_questTasks[i].TaskState == EQuestState.Processing)
                tasks.Add(_questTasks[i]);
        }

        return tasks;
    }

    public static Quest MakeQuest(QuestSaveData saveData)
    {
        if (Managers.Data.QuestDic.TryGetValue(saveData.TemplateId, out QuestData questData) == false)
            return null;

        Quest quest = null;
        quest = new Quest(saveData);

        return quest;
    }
    public void OnHandleBroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
    {
        foreach (QuestTask task in GetProcessingTasks())
        {
            task.OnHandleBroadcastEvent(eventType, currencyType, value);
        }

        UpdateTasksData();
    }

    public void UpdateTasksData()
    {
        //Task들의 Count 업데이트
        for (int i = 0; i < _questTasks.Count; i++)
        {
            SaveData.TaskProgressCount[i] = _questTasks[i].Count;
            SaveData.TaskStates[i] = _questTasks[i].TaskState;
        }

    }

    public void TryCompleteQuest()
    {

        if (IsTasksCompleted()) //퀘스트 클리어
        {
            if (State == EQuestState.Rewarded)
                return;

            OnQuestCompleted?.Invoke(this);

            foreach (QuestTask task in QuestTasks)
            {
                int nextId = task.TaskData.NextQuestId;
                if (task.TaskData.NextQuestId > 0)
                {
                    Quest nextQuest = Managers.Quest.GetQuest(nextId);
                    nextQuest.StartQuest();
                }
            }

            GetReward();
        }
    }
    private void GetReward()
    {
        if (State == EQuestState.Rewarded)
            return;

        State = EQuestState.Rewarded;

        foreach (QuestRewardData reward in _questRewards)
        {
            switch (reward.RewardType)
            {
                case EQuestRewardType.Mineral:
                    Managers.Inventory.EarnCurrency(ECurrencyType.Mineral, reward.RewardCount);
                    break;
                case EQuestRewardType.Meat:
                    Managers.Inventory.EarnCurrency(ECurrencyType.Meat, reward.RewardCount);
                    break;
                case EQuestRewardType.Gold:
                    Managers.Inventory.EarnCurrency(ECurrencyType.Gold, reward.RewardCount);
                    break;
                case EQuestRewardType.Wood:
                    Managers.Inventory.EarnCurrency(ECurrencyType.Wood, reward.RewardCount);
                    break;
                case EQuestRewardType.Item:
                    Item item = Managers.Inventory.MakeItem(reward.RewardDataId);
                    Managers.UI.ShowToast($"Items : {Managers.GetText(item.TemplateData.NameTextId, ETextType.Name)}");
                    break;
            }
        }

    }

}
