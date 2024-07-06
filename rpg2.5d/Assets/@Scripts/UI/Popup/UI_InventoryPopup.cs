using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_InventoryPopup : UI_Popup
{
    enum GameObjects
    {
        Header,
        ItemList,
        SelectedItemGuide,
    }

    enum Buttons
    {
        CloseButton,
    }

    enum Images
    {
        SelectedItemImage,
    }

    enum Texts
    {
        SelectedItemText,
        SelectedItemGoldText,
        ItemInfoText,

        CurrencyGoldText,
        CurrencyWoodText,
        CurrencyMineralText,
        CurrencyMeatText,
        CurrencyDiaText,
    }
    
    enum Toggles
    {
        EquipmentToggle,
        ConsumableToggle,
        CurrencyToggle,
    }

    List<UI_InventoryPopup_SlotItem> _slotItems = new List<UI_InventoryPopup_SlotItem>();

    private Toggle _equipmentToggle;
    private Toggle _consumableToggle;
    private Toggle _currencyToggle;
    Item _selectedItem;
    RectTransform _view;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _view = Util.FindChild(gameObject, "View", true).GetComponent<RectTransform>();

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindToggle(typeof(Toggles));

        _equipmentToggle = GetToggle((int)Toggles.EquipmentToggle);
        _consumableToggle = GetToggle((int)Toggles.ConsumableToggle);
        _currencyToggle = GetToggle((int)Toggles.CurrencyToggle);

        _equipmentToggle.gameObject.BindEvent(Refresh);
        _consumableToggle.gameObject.BindEvent(Refresh);
        _currencyToggle.gameObject.BindEvent(Refresh);

        GetObject((int)GameObjects.Header).gameObject.BindEvent(null, OnDragHeader, Define.UIEvent.Drag);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetObject((int)GameObjects.SelectedItemGuide).gameObject.SetActive(false);

        Transform parent = GetObject((int)GameObjects.ItemList).transform;
        parent.gameObject.DestroyChilds();

        for (int i = 0; i < InventoryManager.DEFAULT_INVENTORY_SLOT_COUNT; i++)
        {
            UI_InventoryPopup_SlotItem item = Managers.UI.MakeSubItem<UI_InventoryPopup_SlotItem>(parent);
            _slotItems.Add(item);
        }

        return true;
    }
    private void OnEnable()
    {
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcastEvent;
        Managers.Game.OnBroadcastEvent += HandleOnBroadcastEvent;
    }
    private void OnDisable()
    {
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcastEvent;
        GetObject((int)GameObjects.SelectedItemGuide).SetActive(false);
    }

    void HandleOnBroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
    {
        switch (eventType)
        {
            case EBroadcastEventType.ChangeInventory:
                Refresh();
                break;
            case EBroadcastEventType.ChangeCurrency:
                RefreshCurrency(currencyType);
                break;
        }
    }

    public void SetInfo()
    {
        Refresh();
    }

    void Refresh()
    {
        RefreshCurrency(ECurrencyType.Gold);
        RefreshCurrency(ECurrencyType.Wood);
        RefreshCurrency(ECurrencyType.Mineral);
        RefreshCurrency(ECurrencyType.Meat);
        RefreshCurrency(ECurrencyType.Dia);
        RefreshInventoryList();
        RefreshItemInfoData();
    }
    void RefreshCurrency(ECurrencyType currencyType)
    {
        switch(currencyType)
        {
            case ECurrencyType.Gold:
                int gold = Managers.Inventory.GetCurrency(ECurrencyType.Gold);
                GetText((int)Texts.CurrencyGoldText).text = gold.ToString();
                break;
            case ECurrencyType.Wood:
                int wood = Managers.Inventory.GetCurrency(ECurrencyType.Wood);
                GetText((int)Texts.CurrencyWoodText).text = wood.ToString();
                break;
            case ECurrencyType.Mineral:
                int mineral = Managers.Inventory.GetCurrency(ECurrencyType.Mineral);
                GetText((int)Texts.CurrencyMineralText).text = mineral.ToString();
                break;
            case ECurrencyType.Meat:
                int meat = Managers.Inventory.GetCurrency(ECurrencyType.Meat);
                GetText((int)Texts.CurrencyMeatText).text = meat.ToString();
                break;
            case ECurrencyType.Dia:
                int dia = Managers.Inventory.GetCurrency(ECurrencyType.Dia);
                GetText((int)Texts.CurrencyDiaText).text = dia.ToString();
                break;
        }
    }

    void RefreshItemInfoData()
    {
        if (_selectedItem == null)
            return;

        //장비이름

        //아이템 잠금
        //_selectedItem.IsLock
    }

    void RefreshInventoryList()
    {
        int MAX_ITEM_COUNT = InventoryManager.DEFAULT_INVENTORY_SLOT_COUNT;

        if (_equipmentToggle.isOn)
        {
            List<Item> items = Managers.Inventory.GetItemsByGroupType(EItemGroupType.Equipment);

            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                if (i < items.Count)
                {
                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(items[i], this);
                }
                else
                {
                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(null, this);
                }
            }
        }
        else if (_consumableToggle.isOn)
        {
            List<Item> items = Managers.Inventory.GetItemsByGroupType(EItemGroupType.Consumable);

            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                if (i < items.Count)
                {
                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(items[i], this);
                }
                else
                {
                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(null, this);
                }
            }
        }
        else
        {
            List<Item> items = Managers.Inventory.GetItemsByGroupType(EItemGroupType.Currency);

            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                if (i < items.Count)
                {
                    switch (((CurrencyData)items[i].TemplateData).currencyType)
                    {
                        case ECurrencyType.Wood:
                        case ECurrencyType.Mineral:
                        case ECurrencyType.Meat:
                        case ECurrencyType.Gold:
                        case ECurrencyType.Dia:
                            _slotItems[i].gameObject.SetActive(false);
                            continue;
                    }

                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(items[i], this);
                }
                else
                {
                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(null, this);
                }
            }
        }
    }

    public void SelectItem(Item item)
    {
        if (item == null)
            return;

        GetObject((int)GameObjects.SelectedItemGuide).SetActive(true);

        _selectedItem = item;
        Refresh_SelectedItem();
    }

    void Refresh_SelectedItem()
    {
        string gradeString = "";
        switch (_selectedItem.TemplateData.Grade)
        {
            case Define.EItemGrade.None:
                return;

            case Define.EItemGrade.Normal:
                gradeString = "Normal";
                break;
            case Define.EItemGrade.Rare:
                gradeString = "Rare";
                break;
            case Define.EItemGrade.Epic:
                gradeString = "Epic";
                break;
            case Define.EItemGrade.Legendary:
                gradeString = "Legendary";
                break;
        }

        GetImage((int)Images.SelectedItemImage).sprite = Managers.Resource.Load<Sprite>(_selectedItem.TemplateData.SpriteName);

        GetText((int)Texts.SelectedItemText).text = _selectedItem.TemplateData.Name;

        string itemDesc = Managers.GetText(_selectedItem.TemplateData.DescriptionTextID, ETextType.Description); 
        GetText((int)Texts.ItemInfoText).text = itemDesc;
    }

    void OnDragHeader(PointerEventData pointerEventData)
    {
        _view.anchoredPosition += pointerEventData.delta;
    }
    void OnClickCloseButton()
    {
        Managers.UI.GetSceneUI<UI_GameScene>().CloseInventoryPopup();
    }



}
