using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using UnityEngine.Rendering;
using Data;
using UnityEngine.Diagnostics;
using UnityEngine.AI;

public class InteractionObject : BaseObject
{// 서로 상호작용 가능 Object

    public int TemplateId {  get; set; }
    public EffectComponent Effects { get; set; }

    public event Action<InteractionObject> EventOnDead;

    protected UI_HPBar _hpBar;

    public Vector3 OverheadPosition => Position + Vector3.up * OverheadOffset;
    protected float OverheadOffset => _collider.height * 1.2f;
    protected CapsuleCollider _collider;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        GameObject effectObj = new GameObject();
        effectObj.name = "Effects";
        effectObj.transform.parent = gameObject.transform;
        Effects = effectObj.AddComponent<EffectComponent>();
        Effects.SetInfo(this);

        _collider = gameObject.GetComponent<CapsuleCollider>();
        return true;
    }

    public virtual void SetInfo(int templateId)
    {


        _hpBar = Managers.UI.MakeOverlayUI<UI_HPBar>(transform);
        _hpBar.SetInfo(this);
    }

    public virtual void OnDamage(InteractionObject attacker, float value)
    {
        
    }

    protected virtual void OnDead()
    {
        EventOnDead?.Invoke(this);
    }

    #region DropItem
    public void DropItem(int dropItemId)
    {
        StartCoroutine(CoDropItem(dropItemId));
    }
    void SpawnItemHolder(int dropItemId, RewardData rewardData)
    {
        var itemHolder = Managers.Object.Spawn<ItemHolder>(OverheadPosition, dropItemId);

        Vector3 dropPos = OverheadPosition.GetRandomPointInCircle(2.5f);
        int areaMask = 1 << NavMesh.GetAreaFromName("Walkable");
        NavMesh.SamplePosition(dropPos, out NavMeshHit hit, 10, areaMask);
        dropPos = hit.position;

        itemHolder.SetInfo(rewardData, OverheadPosition, dropPos);
    }

    IEnumerator CoDropItem(int dropItemId)
    {
        List<RewardData> rewards = GetRewards(dropItemId);
        if (rewards != null)
        {
            foreach (var reward in rewards)
            {
                SpawnItemHolder(dropItemId, reward);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    List<RewardData> GetRewards(int dropItemId)
    {
        if (Managers.Data.DropTableDic.TryGetValue(dropItemId, out DropTableData dropTableData) == false)
            return null;

        if (dropTableData.Rewards.Count <= 0)
            return null;

        List<RewardData> rewardDatas = new List<RewardData>();

        int sum = 0;
        int randValue = UnityEngine.Random.Range(0, 100);

        foreach (RewardData item in dropTableData.Rewards)
        {
            if (item.Probability == 100)
            {
                //확정드롭아이템
                rewardDatas.Add(item);
                continue;
            }

            //확정드롭아이템을 제외한 아이템
            sum += item.Probability;
            if (randValue <= sum)
            {
                rewardDatas.Add(item);
                break;
            }

        }

        return rewardDatas;
    }
    #endregion
}
