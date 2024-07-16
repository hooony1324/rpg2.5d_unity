using Data;
using NSubstitute;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using static UnityEngine.UI.GridLayoutGroup;

public interface IItemAccessible
{
    public ItemHolder TargetItemHolder { get; set; }
    public void TrySetTargetItemHolder(ItemHolder itemHolder);
}

[RequireComponent(typeof(CapsuleCollider))]
public class ItemHolder : BaseObject
{
    private ItemData _itemData;
    private RewardData _rewardData;
    private SpriteRenderer _currentSprite;
    private ParabolaMotion _parabolaMotion;
    private TextMeshPro _text;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.ItemHolder;
        
        _currentSprite = Util.FindChild(gameObject, "Mesh").GetOrAddComponent<SpriteRenderer>();
        _currentSprite.sortingOrder = SortingLayers.DROP_ITEM;

        _parabolaMotion = gameObject.GetOrAddComponent<ParabolaMotion>();
        _text = Util.FindChild<TextMeshPro>(gameObject, "Text");

        _text.gameObject.SetActive(false);

        return true;
    }

    public void SetInfo(RewardData rewardData, Vector3 startPos, Vector3 endPos)
    {
        _rewardData = rewardData;
        _itemData = Managers.Data.ItemDic[rewardData.ItemTemplateId];
        _currentSprite.sprite = Managers.Resource.Load<Sprite>(_itemData.SpriteName);
        _parabolaMotion.SetInfo(startPos, endPos, null, null, 3f, endCallback: Arrived);

        string name = Managers.GetText(_itemData.NameTextId, ETextType.Name);
        _text.SetText(name);
        _text.color = Util.GetTextColor(_itemData.Grade);
    }

    void Arrived()
    {
        _text.gameObject.SetActive(true);
        GetComponent<CapsuleCollider>().enabled = true;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    _text.gameObject.SetActive(true);

    //    var itemAccessor = other.GetComponent<IItemAccessible>();
    //    if (itemAccessor == null)
    //        return;

    //    itemAccessor.SetTargetItemHolder(this);

    //    if (itemAccessor.TargetItemHolder == this)
    //    {
    //        Managers.UI.SceneUI.GetComponent<UI_GameScene>().ActivateInputGuide("AcquireItemGuide");
    //    }
    //}

    public void OnTriggerStay(Collider other)
    {
        _text.gameObject.SetActive(true);

        var itemAccessor = other.GetComponent<IItemAccessible>();
        if (itemAccessor == null)
            return;

        itemAccessor.TrySetTargetItemHolder(this);

        if (itemAccessor.TargetItemHolder == this)
        {
            Managers.UI.SceneUI.GetComponent<UI_GameScene>().ActivateInputGuide("AcquireItem");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _text.gameObject.SetActive(false);

        var itemAccessor = other.GetComponent<IItemAccessible>();
        if (itemAccessor == null)
            return;

        if (itemAccessor.TargetItemHolder == this)
        {
            itemAccessor.TargetItemHolder = null;
            Managers.UI.SceneUI.GetComponent<UI_GameScene>().DeActivateInputGuide();
        }
    }

    public void AcquireItem()
    {
        switch(_itemData.ItemGroupType)
        {
            case EItemGroupType.Equipment:
                if (Managers.Inventory.IsInventoryFull())
                {
                    Managers.UI.ShowToast("Inventory is full.");
                    break;
                }
                if (Managers.Data.EquipmentDic.TryGetValue(_itemData.DataId, out EquipmentData equipData))
                {
                    Managers.Inventory.MakeItem(_itemData.DataId);
                    Managers.UI.ShowToast($"Items : {Managers.GetText(equipData.NameTextId, ETextType.Name)}");
                }
                break;
            case EItemGroupType.Consumable:
                if (Managers.Inventory.IsInventoryFull())
                {
                    Managers.UI.ShowToast("Inventory is full.");
                    break;
                }
                if (Managers.Data.ConsumableDic.TryGetValue(_itemData.DataId, out ConsumableData consumableData))
                {
                    Managers.Inventory.MakeItem(_itemData.DataId);
                    Managers.UI.ShowToast($"Items : {Managers.GetText(consumableData.NameTextId, ETextType.Name)}");
                }
                break;
            case EItemGroupType.Currency:
                CurrencyData currencyData = _itemData as CurrencyData;
                if (currencyData != null)
                {
                    Managers.Inventory.EarnCurrency(currencyData.currencyType, _rewardData.Count);
                }
                break;
        }


        Managers.Object.Despawn(this);
        Managers.UI.SceneUI.GetComponent<UI_GameScene>().DeActivateInputGuide();
    }

    private EItemSubType[] equipmentItems = new EItemSubType[]
    {
        EItemSubType.PinkRune,
        EItemSubType.RedRune,
        EItemSubType.YellowRune,
        EItemSubType.MintRune,
    };
    private int GetRandomEquipment()
    {
        EquipmentData[] equipments = Managers.Data.EquipmentDic.Values.ToArray();

        //1. µå·ÓÇÒ Àåºñ ÆÄÃ÷ °áÁ¤(Çï¸ä Àå°© ¹«±â µî)
        EItemSubType subType = ChooseSubEquipmentType();

        //2. µî±Þ °áÁ¤
        EItemGrade grade = Util.ChooseItemGrade();

        List<EquipmentData> filtered = equipments.Where(e => e.SubType == subType && e.Grade == grade).ToList();


        EquipmentData selected = null;
        if (filtered.Count > 0)
        {
            selected = filtered[UnityEngine.Random.Range(0, filtered.Count)];
        }
        else
        {
            Debug.LogError("WHY?");
        }

        if (selected != null)
            return selected.DataId;

        return -1;
    }

    private EItemSubType ChooseSubEquipmentType()
    {
        int index = UnityEngine.Random.Range(0, equipmentItems.Length);
        return equipmentItems[index];
    }
}

