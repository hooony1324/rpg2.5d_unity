using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using UnityEngine.Rendering;

public class InteractionObject : BaseObject
{// 서로 상호작용 가능 Object

    public int TemplateId {  get; set; }
    public EffectComponent Effects { get; set; }

    public event Action<InteractionObject> EventOnDead;

    protected UI_HPBar _hpBar;

    protected HurtFlashEffect _hurtFlash;
    public Vector3 OverheadPosition => Position + Vector3.up * OverheadOffset;
    public Vector3 OverheadLocalPosition => Vector3.up * OverheadOffset;
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

        _hurtFlash = gameObject.GetOrAddComponent<HurtFlashEffect>();
        _hurtFlash.Init();

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

}
