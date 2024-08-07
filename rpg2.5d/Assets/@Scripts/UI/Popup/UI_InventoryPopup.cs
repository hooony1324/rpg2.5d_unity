using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        SelectedItemButton,
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

        _equipmentToggle.gameObject.BindEvent(OnClickTab);
        _consumableToggle.gameObject.BindEvent(OnClickTab);
        _currencyToggle.gameObject.BindEvent(OnClickTab);

        GetObject((int)GameObjects.Header).gameObject.BindEvent(null, OnDragHeader, Define.UIEvent.Drag);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetObject((int)GameObjects.SelectedItemGuide).gameObject.SetActive(false);
        GetButton((int)Buttons.SelectedItemButton).gameObject.BindEvent(OnClickSelectedItemButton);
        GetButton((int)Buttons.SelectedItemButton).gameObject.SetActive(false);

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
    private void OnClickTab()
    {
        _selectedItem = null;
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
        RefreshSelectedItemInfo();
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

        _selectedItem = item;
        RefreshSelectedItemInfo();
    }

    void RefreshSelectedItemInfo()
    {
        if (_selectedItem == null)
        {
            GetObject((int)GameObjects.SelectedItemGuide).SetActive(false);
            return;
        }

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

        GetText((int)Texts.SelectedItemText).text = _selectedItem.TemplateData.NameTextId;

        string itemDesc = Managers.GetText(_selectedItem.TemplateData.DescriptionTextID, ETextType.Description); 
        GetText((int)Texts.ItemInfoText).text = itemDesc;

        GameObject button = GetButton((int)Buttons.SelectedItemButton).gameObject;
        switch (_selectedItem.ItemType)
        {
            case EItemType.Equipment:
                Util.FindChild(button, "SelectedItemButtonText").GetComponent<TMP_Text>().text = Managers.GetText("Button_SelectedItem_Equip", ETextType.Name);
                button.SetActive(true);
                break;
            case EItemType.Potion:
            case EItemType.Scroll:
                Util.FindChild(button, "SelectedItemButtonText").GetComponent<TMP_Text>().text = Managers.GetText("Button_SelectedItem_Use", ETextType.Name);
                button.SetActive(true);
                break;
            default:
                button.SetActive(false);
                break;
        }

        GetObject((int)GameObjects.SelectedItemGuide).SetActive(true);
    }

    void OnDragHeader(PointerEventData pointerEventData)
    {
        _view.anchoredPosition += pointerEventData.delta;
    }
    void OnClickCloseButton()
    {
        Managers.UI.GetSceneUI<UI_GameScene>().CloseInventoryPopup();
    }

    void OnClickSelectedItemButton()
    {
        if (_selectedItem == null)
            return;

        if (_selectedItem.ItemType == EItemType.Potion || _selectedItem.ItemType == EItemType.Scroll)
        {
            Managers.Inventory.UseConsumableItem(_selectedItem.TemplateId);
            _selectedItem = null;
            Refresh();
        }
    }

}
