using System;
using Data;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Define;
using Unity.VisualScripting;

public class InventoryManager
{
    public const int DEFAULT_INVENTORY_SLOT_COUNT = 50;
    public int MaxWood
    {
        get { return Managers.Game.SaveData.MaxWood; }
        private set { Managers.Game.SaveData.MaxWood = value; }
    }

    public int MaxMineral
    {
        get { return Managers.Game.SaveData.MaxMineral; }
        private set { Managers.Game.SaveData.MaxMineral = value; }
    }

    public int MaxMeat
    {
        get { return Managers.Game.SaveData.MaxMeat; }
        private set { Managers.Game.SaveData.MaxMeat = value; }
    }

    // 가지고 있는 모든 아이템
    public List<Item> AllItems { get; } = new List<Item>();

    // Cache용도
    Dictionary<int /*EquipSlot*/, Item> EquippedItems = new Dictionary<int, Item>();
    Dictionary<ECurrencyType, Item> Currencies = new Dictionary<ECurrencyType, Item>(); 
    List<Item> InventoryItems = new List<Item>(); // 가방
    List<Item> WarehouseItems = new List<Item>(); // 창고

    public Item MakeItem(int itemTemplateId, int count = 1)
    {
        if (Managers.Data.ItemDic.TryGetValue(itemTemplateId, out ItemData itemData) == false)
            return null;

        ItemSaveData itemSaveData = MakeItemData(itemTemplateId, count);
        return AddItem(itemSaveData);
    }

    public ItemSaveData MakeItemData(int itemTemplateId, int count = 1)
    {
        int itemDbId = Managers.Game.GenerateItemDbId();

        // 장비아이템의 경우 옵션 추가
        List<int> optionIds = new List<int>();
        if (Managers.Data.EquipmentDic.TryGetValue(itemTemplateId, out EquipmentData equipmentData))
        {
            if (equipmentData.SubOptionCount > 0)
                optionIds = GenerateOptionIds(equipmentData);
        }

        ItemSaveData saveData = new ItemSaveData()
        {
            InstanceId = itemDbId,
            DbId = itemDbId,
            TemplateId = itemTemplateId,
            Count = count,
            EquipSlot = (int)EEquipSlotType.Inventory,
            EnchantCount = 0,
            OptionIds = optionIds
        };

        return saveData;
    }


    public Item AddItem(ItemSaveData itemInfo)
    {
        Item item = Item.MakeItem(itemInfo);
        if (item == null)
            return null;

        if (item.IsEquippedItem())
        {
            EquippedItems.Add(item.SaveData.EquipSlot, item);
        }
        else if (item.IsInInventory())
        {
            InventoryItems.Add(item);
        }
        else if (item.IsInWarehouse())
        {
            WarehouseItems.Add(item);
        }

        if (item.TemplateData.ItemGroupType == EItemGroupType.Currency)
        {
            CurrencyData data = (CurrencyData)item.TemplateData;
            Currencies.Add(data.currencyType, item);
        }

        AllItems.Add(item);
        Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);

        return item;
    }

    public void RemoveItem(int instanceId)
    {
        Item item = AllItems.Find(x => x.SaveData.InstanceId == instanceId);
        if (item == null)
            return;

        if (item.IsEquippedItem())
        {
            EquippedItems.Remove(item.SaveData.EquipSlot);
        }
        else if (item.IsInInventory())
        {
            InventoryItems.Remove(item);
        }
        else if (item.IsInWarehouse())
        {
            WarehouseItems.Remove(item);
        }

        AllItems.Remove(item);
        Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);
    }
    public void EquipItem(int instanceId)
    {
        Item item = InventoryItems.Find(x => x.SaveData.InstanceId == instanceId);
        if (item == null)
        {
            Debug.Log("아이템존재안함");
            return;
        }

        EEquipSlotType equipSlotType = item.GetEquipItemEquipSlot();
        if (equipSlotType == EEquipSlotType.None)
            return;

        // 기존 아이템 해제
        if (EquippedItems.TryGetValue((int)equipSlotType, out Item prev))
            UnEquipItem(prev.InstanceId);

        // 아이템 장착
        item.EquipSlot = (int)equipSlotType;
        EquippedItems[(int)equipSlotType] = item;
        InventoryItems.Remove(item);
        Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);
    }

    public void UnEquipItem(int instanceId, bool checkFull = true)
    {
        var item = EquippedItems.Values.Where(x => x.InstanceId == instanceId).FirstOrDefault();
        if (item == null)
            return;

        // TODO

        if (checkFull && IsInventoryFull())
            return;

        EquippedItems.Remove((int)item.EquipSlot);

        item.EquipSlot = (int)EEquipSlotType.Inventory;
        InventoryItems.Add(item);
        Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);
    }

    public void UseConsumableItem(int templateId)
    {
        Consumable item = AllItems.Find(x => x.SaveData.TemplateId == templateId) as Consumable;

        if (item == null)
            return;

        item.Count--;
        if (item.Count == 0)
            AllItems.Remove(item);

        Managers.Game.PlayerHero.Effects.GenerateEffects(item.EffectIds, EEffectSpawnType.External, Managers.Game.PlayerHero);
        Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);
    }

    #region EquippedItem
    public float GetStatModifier(ECalcStatType calcStatType, EStatModType type)
    {
        float value = 0;
        foreach (var item in EquippedItems.Values)
        {
            Equipment equipment = item as Equipment;
            if (equipment != null)
            {
                value += equipment.GetStatModifier(calcStatType, type);
            }
        }

        return value;
    }

    #endregion

    #region Currency
    public int GetCurrency(ECurrencyType type)
    {
        if (Currencies.TryGetValue(type, out Item currency))
        {
            return currency.Count;
        }
        else
        {
            Debug.LogError($"CurrencyError: {type}");
        }

        return -1;
    }

    public bool CheckCurrency(ECurrencyType type, int amount)
    {
        int currency = GetCurrency(type);
        if (currency < 0)
            return false;

        if (currency >= amount)
            return true;

        return GetCurrency(type) >= amount;
    }

    public bool SpendCurrency(ECurrencyType type, int amount)
    {
        if (!CheckCurrency(type, amount))
        {
            Managers.UI.ShowToast(Managers.GetText("LackCurrency", ETextType.Message));
            return false;
        }

        if (Currencies.TryGetValue(type, out Item currency))
        {
            int prev = currency.Count;
            currency.Count = Mathf.Max(0, currency.Count - amount);
            int diff = currency.Count - prev;
            Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeCurrency, type, diff);
        }
        else
        {
            return false;
        }

        return true;
    }

    public void EarnCurrency(ECurrencyType type, int amount)
    {
        if (Currencies.TryGetValue(type, out Item currency) == false)
            return;

        int prev = currency.Count;
        currency.Count = Mathf.Clamp(currency.Count + amount, 0, GetMax(type));
        int diff = currency.Count - prev;
        Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeCurrency, type, diff);
    }

    public int GetMax(ECurrencyType type)
    {
        switch (type)
        {
            case ECurrencyType.Wood:
                return MaxWood;
            case ECurrencyType.Mineral:
                return MaxMineral;
            case ECurrencyType.Meat:
                return MaxMeat;
            case ECurrencyType.Gold:
            case ECurrencyType.Dia:
                return int.MaxValue;
        }

        return 9999;
    }
    #endregion

    #region RandomOptionGenerator
    private List<int> GenerateOptionIds(EquipmentData equipmentData)
    {
        // Equipment의 ItemLevel과 같은 OptionData적용
        // OptionGrade는 Equipoment의 EquipmentGrade와 같을 필요는 없음
        // ex) Rare장비에 Legendary옵션 뜰 수도 있음
        // 단, 하나의 장비에 같은 OptionType이 2개 존재할 수 없음

        List<int> optionIds = new List<int>();
        ECalcStatType[] optionTypes = ChooseRandomItemOptions(equipmentData.SubOptionCount);

        List<EquipmentOptionData> options = Managers.Data.EquipmentOptionDic.Values.ToList();
        foreach (var optionType in optionTypes)
        {
            EItemGrade grade = Util.ChooseItemGrade();

            var filtered = options.Where(e => e.ItemLevel == equipmentData.ItemLevel && e.CalcStatType == optionType && e.OptionGrade == grade).ToList();

            if (filtered.Count > 0)
            {
                EquipmentOptionData selected = filtered[UnityEngine.Random.Range(0, filtered.Count)];
                optionIds.Add(selected.DataId);
            }
        }
        return optionIds;
    }
    private ECalcStatType[] ChooseRandomItemOptions(int count)
    {
        var values = Enum.GetValues(typeof(ECalcStatType)).Cast<ECalcStatType>()
            .Where(e => e != ECalcStatType.Hp && e != ECalcStatType.Count).ToArray();

        values.Shuffle();
        return values.Take(count).ToArray();
    }

    #endregion

    public void Clear()
    {
        AllItems.Clear();

        EquippedItems.Clear();
        InventoryItems.Clear();
        WarehouseItems.Clear();
    }

    #region Helper
    public Item GetItem(int instanceId)
    {
        return AllItems.Find(item => item.InstanceId == instanceId);
    }

    public Item GetEquippedItem(EEquipSlotType equipSlotType)
    {
        EquippedItems.TryGetValue((int)equipSlotType, out Item item);

        return item;
    }

    public Item GetEquippedItem(int instanceId)
    {
        return EquippedItems.Values.Where(x => x.InstanceId == instanceId).FirstOrDefault();
    }

    public Item GetEquippedItemBySubType(EItemSubType subType)
    {
        return EquippedItems.Values.Where(x => x.SubType == subType).FirstOrDefault();
    }

    public Item GetItemInInventory(int instanceId)
    {
        return InventoryItems.Find(x => x.SaveData.InstanceId == instanceId);
    }

    public bool IsInventoryFull()
    {
        return InventoryItems.Count >= InventorySlotCount();
    }

    public int InventorySlotCount()
    {
        return DEFAULT_INVENTORY_SLOT_COUNT;
    }

    public List<Item> GetItemsByGroupType(EItemGroupType groupType)
    {
        return AllItems.Where(x => x.TemplateData.ItemGroupType == groupType).ToList();
    }

    public int GetCountByGroupType(EItemGroupType groupType)
    {
        return AllItems.Count(x => x.TemplateData.ItemGroupType == groupType);
    }

    public List<Item> GetEquippedItems()
    {
        return EquippedItems.Values.ToList();
    }

    public List<ItemSaveData> GetEquippedItemInfos()
    {
        return EquippedItems.Values.Select(x => x.SaveData).ToList();
    }

    public List<Item> GetInventoryItems()
    {
        return InventoryItems.ToList();
    }

    public List<ItemSaveData> GetInventoryItemInfos()
    {
        return InventoryItems.Select(x => x.SaveData).ToList();
    }

    public List<ItemSaveData> GetInventoryItemInfosOrderbyGrade()
    {
        return InventoryItems.OrderByDescending(y => (int)y.TemplateData.Grade)
                        .ThenBy(y => (int)y.TemplateId)
                        .Select(x => x.SaveData)
                        .ToList();
    }

    public List<ItemSaveData> GetWarehouseItemInfos()
    {
        return WarehouseItems.Select(x => x.SaveData).ToList();
    }

    public List<ItemSaveData> GetWarehouseItemInfosOrderbyGrade()
    {
        return WarehouseItems.OrderByDescending(y => (int)y.TemplateData.Grade)
                                    .ThenBy(y => (int)y.TemplateId)
                                    .Select(x => x.SaveData)
                                    .ToList();
    }

    public int GetEquippedItemScore()
    {
        int score = 0;
        foreach (var item in EquippedItems.Values)
        {
            Equipment equipment = item as Equipment;

            if (equipment == null)
                continue;

            score += equipment.ItemScore;
        }

        return score;
    }

    #endregion
}
