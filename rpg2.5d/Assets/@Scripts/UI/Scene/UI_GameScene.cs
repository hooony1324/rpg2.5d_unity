using Castle.Core.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Define;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        InputGuide,
        QuestList,
    }

    enum Texts
    {
        InputText,
        InputGuideText,
    }

    enum Buttons
    {
        QuestDropdownButton,
    }

    GameObject _questList;
    RectTransform _questListRect;
    List<UI_QuestList_SubItem> _questListItems = new List<UI_QuestList_SubItem>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.InputGuide).gameObject.SetActive(false);

        _questList = GetObject((int)GameObjects.QuestList).gameObject;
        _questListRect = _questList.GetComponent<RectTransform>();

        GetButton((int)Buttons.QuestDropdownButton).gameObject.BindEvent(OnToggleQuestDropdown);

        for (int i = 0; i < QuestManager.DEFAULT_QUEST_SLOT_COUNT; i++)
        {
            UI_QuestList_SubItem subItem = Managers.UI.MakeSubItem<UI_QuestList_SubItem>(_questList.transform);
            _questListItems.Add(subItem);
        }

        Refresh();
        return true;
    }

    public void SetInfo()
    {
        Managers.Game.OnBroadcastEvent += HandleOnBroadcastEvent;
        Refresh();
    }

    void HandleOnBroadcastEvent(EBroadcastEventType type, ECurrencyType currencyType, int value)
    {
        switch (type)
        {
            case EBroadcastEventType.ChangeCurrency:
            case EBroadcastEventType.HeroLevelUp:
            case EBroadcastEventType.ChangeInventory:
            case EBroadcastEventType.KillMonster:
                Refresh();
                break;
        }
    }

    void Refresh()
    {
        // Level, Exp, ...

        RefreshQuest();
    }

    #region Quest
    void RefreshQuest()
    {
        List<Quest> quests = Managers.Quest.CompletedOrProcessingQuests;
        int count = quests.Count;
        if (quests.Count == 0)
        {
            for (int i = 0; i < QuestManager.DEFAULT_QUEST_SLOT_COUNT; i++)
            {
                _questListItems[i].gameObject.SetActive(false);
            }

            _questList.SetActive(false);
            return;
        }

        for (int i = 0; i < QuestManager.DEFAULT_QUEST_SLOT_COUNT; i++)
        {
            if (count - 1 < i)
            {
                _questListItems[i].RefreshDisplay(null);
                _questListItems[i].gameObject.SetActive(false);
            }
            else
            {
                _questListItems[i].RefreshDisplay(quests[i]);
                _questListItems[i].gameObject.SetActive(true);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_questListRect);
    }

    void OnToggleQuestDropdown()
    {
        GameObject button = GetButton((int)Buttons.QuestDropdownButton).gameObject;
        bool bEnabled;
        if (_questList.active == true)
        {
            _questList.SetActive(false);
            bEnabled = false;
        }
        else
        {
            _questList.SetActive(true);
            bEnabled = true;
        }

        RectTransform rect = button.GetComponent<RectTransform>();
        Vector3 currentRotation = rect.localEulerAngles;
        currentRotation.z = bEnabled ? 270 : 90;

        rect.localEulerAngles = currentRotation;
    }
    #endregion

    #region InputGuide
    public void ActivateInputGuide(string textId)
    {
        // TODO:
        // "F" -> Mapping정보에서 받아오고
        // "아이템 획득" -> TextDic
        string guide = Managers.GetText(textId, ETextType.Message);
        if (guide.IsNullOrEmpty())
            return;

        GetText((int)Texts.InputText).SetText("F");
        GetText((int)Texts.InputGuideText).SetText(guide);

        GetObject((int)GameObjects.InputGuide).gameObject.SetActive(true);
    }

    public void DeActivateInputGuide()
    {
        GetObject((int)GameObjects.InputGuide).gameObject.SetActive(false);
    }
    #endregion

    #region Inventory
    UI_InventoryPopup _inventoryPopup;
    public void ToggleInventoryPopup()
    {
        if (_inventoryPopup == null)
        {
            _inventoryPopup = Managers.UI.ShowPopupUI<UI_InventoryPopup>();
            _inventoryPopup.SetInfo();
        }
        else
        {
            CloseInventoryPopup();
        }
    }

    public void CloseInventoryPopup()
    {
        Managers.UI.ClosePopupUI(_inventoryPopup);
        _inventoryPopup = null;
    }
    #endregion
}
