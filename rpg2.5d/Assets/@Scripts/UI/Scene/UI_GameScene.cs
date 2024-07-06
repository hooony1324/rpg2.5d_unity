using Castle.Core.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        InputGuide,
    }

    enum Texts
    {
        InputText,
        InputGuideText,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));

        GetObject((int)GameObjects.InputGuide).gameObject.SetActive(false);


        Refresh();
        return true;
    }

    public void SetInfo()
    {

    }

    void Refresh()
    {

    }
    
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
