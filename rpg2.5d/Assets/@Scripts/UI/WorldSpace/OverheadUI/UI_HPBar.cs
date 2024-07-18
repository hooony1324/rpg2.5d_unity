using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Define;
using Slider = UnityEngine.UI.Slider;

public class UI_HPBar : UI_Base
{
    enum Sliders
    {
        HP,
    }

    InteractionObject _owner;
    Slider _hp;
    Slider _mp;
    RectTransform _rect;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindSlider(typeof(Sliders));
        _hp = GetSlider((int)Sliders.HP);

        _rect = gameObject.GetComponent<RectTransform>();

        return true;
    }

    public void SetInfo(InteractionObject owner)
    {
        _owner = owner;

        //..
        _rect.anchoredPosition = new Vector3(0, _owner.OverheadOffset, 0);
        
        switch (owner.ObjectType)
        {
            case EObjectType.Hero:
            case EObjectType.Monster:
                break;
        }
    }

    //private void Update()
    //{
    //    _rect.position = _camera.WorldToScreenPoint(_owner.OverheadPosition);
    //}

    public void Refresh(float ratio)
    {
        // StatChangeEvent -> Refresh(need StatComp)
        _hp.value = ratio;
    }

}
