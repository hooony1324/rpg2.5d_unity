using System.Text;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using static Define;
using System;
using System.Collections.Generic;
using TMPro;

public class UI_QuestList_SubItem : UI_SubItem
{
    const int MAX_REWARD_ITEM_SLOT = 6;
    enum Buttons
    {
        QuestCompleteButton,
    }

    enum Texts
    {
        QuestTitleText,
        QuestTaskText,

        RewardText1,
        RewardText2,
        RewardText3,
        RewardText4,
        RewardText5,
        RewardText6
    }

    enum Images
    {
        RewardImage1,
        RewardImage2,
        RewardImage3,
        RewardImage4,
        RewardImage5,
        RewardImage6,
    }

    Quest _quest;
    GameObject _completeButton;

    List<TMP_Text> _rewardTexts = new List<TMP_Text>();
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        _completeButton = GetButton((int)Buttons.QuestCompleteButton).gameObject;
        _completeButton.SetActive(false);
        _completeButton.BindEvent(OnHandleComplete);

        for (int i = 0; i < MAX_REWARD_ITEM_SLOT; i++)
        {
            GetImage(i).transform.parent.gameObject.SetActive(false);
        }

        for (int i = (int)Texts.RewardText1; i <= (int)Texts.RewardText6; i++)
        {
            _rewardTexts.Add(GetText(i));
        }

        return true;
    }

    public void RefreshInfo(Quest quest)
    {
        if (quest == null)
            return;

        _quest = quest;

        GetText((int)Texts.QuestTitleText).SetText(Managers.GetText(quest.QuestData.NameTextId, ETextType.Name));

        StringBuilder sb = new StringBuilder();
        foreach (QuestTask task in quest.QuestTasks)
        {
            sb.Append(Managers.GetText(task.TaskData.DescriptionTextId, ETextType.Description));
            if (task.TaskData.ObjectiveCount > 0)
            {
                string achievement = $" {task.Count}/{task.TaskData.ObjectiveCount}";
                sb.AppendLine(achievement);
            }
        }

        GetText((int)Texts.QuestTaskText).SetText(sb.ToString());

        RefreshRewardImages();

        _completeButton.SetActive(_quest.IsTasksCompleted());
    }

    private void RefreshRewardImages()
    {
        if (_quest == null)
            return;

        for (int i = 0; i < MAX_REWARD_ITEM_SLOT; i++)
        {
            Image image = GetImage(i);
            if (i > _quest.QuestRewards.Count - 1)
            {
                image.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                image.sprite =  Managers.Resource.Load<Sprite>($"{_quest.QuestRewards[i].SpriteName}");
                image.transform.parent.gameObject.SetActive(true);

                _rewardTexts[i].text = _quest.QuestRewards[i].RewardCount.ToString();
            }
        }
    }
    public void OnHandleComplete()
    {
        if (_quest != null)
        {
            _quest.TryCompleteQuest();
        }
    }
}
