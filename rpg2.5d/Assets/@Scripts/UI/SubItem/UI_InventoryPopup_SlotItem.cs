using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InventoryPopup_SlotItem : UI_SubItem
{
    enum Images
    {
        ItemSlotImage,
        ItemImage,
        ItemFrameImage,
        SelectedSlotImage,
        EquippedSlotImage,
    }
    
    enum Texts
    {
        ItemCountText,
    }

    private Item _item;

    UI_InventoryPopup _inventoryPopupUI;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        gameObject.BindEvent(OnClickObject);
        gameObject.BindEvent(null, OnBeginDrag, Define.UIEvent.BeginDrag);
        gameObject.BindEvent(null, OnDrag, Define.UIEvent.Drag);
        gameObject.BindEvent(null, OnEndDrag, Define.UIEvent.EndDrag);

        GetImage((int)Images.ItemSlotImage).gameObject.SetActive(false);
        GetImage((int)Images.SelectedSlotImage).gameObject.SetActive(false);
        GetImage((int)Images.EquippedSlotImage).gameObject.SetActive(false);
        GetText((int)Texts.ItemCountText).gameObject.SetActive(false);

        Refresh();

        return true;
    }
    public void SetInfo(Item item, UI_InventoryPopup popup)
    {
        _item = item;
        _inventoryPopupUI = popup;

        Refresh();
    }

    void Refresh()
    {
        if (_item == null || _inventoryPopupUI == null)
        {
            GetText((int)Texts.ItemCountText).gameObject.SetActive(false);
            GetImage((int)Images.ItemSlotImage).gameObject.SetActive(false);
            return;
        }
        
        GetImage((int)Images.ItemSlotImage).gameObject.SetActive(true);
        GetImage((int)Images.ItemImage).sprite = Managers.Resource.Load<Sprite>(_item.TemplateData.SpriteName);
        GetImage((int)Images.ItemImage).gameObject.SetActive(true);

        GetText((int)Texts.ItemCountText).gameObject.SetActive(false);
        if (_item.TemplateData.ItemGroupType == Define.EItemGroupType.Currency)
        {
            // 소모형 아이템만 숫자 보여줌
            GetText((int)Texts.ItemCountText).gameObject.SetActive(true);
            GetText((int)Texts.ItemCountText).text = _item.Count.ToString();
        }

        SelectBg(_item.TemplateData.Grade);

    }

    void OnClickObject()
    {
        //TODO Open popup?
        _inventoryPopupUI.SelectItem(_item);
    }

    void SelectBg(Define.EItemGrade grade)
    {
        string gradeString = "";
        UnityEngine.UI.Image image = GetImage((int)Images.ItemFrameImage);
        switch (grade)
        {
            case Define.EItemGrade.None:
                return;

            case Define.EItemGrade.Normal:
                gradeString = "Normal";
                image.color = Util.HexToColor("806600");
                break;
            case Define.EItemGrade.Rare:
                gradeString = "Rare";
                image.color = Color.white;
                break;
            case Define.EItemGrade.Epic:
                gradeString = "Epic";
                image.color = Color.white;
                break;
            case Define.EItemGrade.Legendary:
                gradeString = "Legendary";
                image.color = Color.white;
                break;
        }
        
        image.sprite = Managers.Resource.Load<Sprite>($"{gradeString}ItemFrame");
    }
}
