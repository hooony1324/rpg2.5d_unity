using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ItemHolder : BaseObject
{
    private ItemData _itemData;
    private RewardData _rewardData;
    private SpriteRenderer _currentSprite;
    private ParabolaMotion _parabolaMotion;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.ItemHolder;
        
        _currentSprite = Util.FindChild(gameObject, "Mesh").GetOrAddComponent<SpriteRenderer>();
        _currentSprite.sortingOrder = SortingLayers.DROP_ITEM;
        _parabolaMotion = gameObject.GetOrAddComponent<ParabolaMotion>();

        return true;
    }

    public void SetInfo(RewardData rewardData, Vector3 startPos, Vector3 endPos)
    {
        _rewardData = rewardData;
        _itemData = Managers.Data.ItemDic[rewardData.ItemTemplateId];
        _currentSprite.sprite = Managers.Resource.Load<Sprite>(_itemData.SpriteName);
        _parabolaMotion.SetInfo(startPos, endPos, null, null, 3f, endCallback: Arrived);
    }

    void Arrived()
    {

    }
}
