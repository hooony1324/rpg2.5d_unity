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
        MP,
    }

    Camera _camera;
    InteractionObject _owner;
    Slider _hp;
    Slider _mp;
    RectTransform _rect;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _camera = Camera.main;

        BindSlider(typeof(Sliders));
        _hp = GetSlider((int)Sliders.HP);
        _mp = GetSlider((int)Sliders.MP);

        _mp.gameObject.SetActive(false);

        _rect = _hp.transform.parent.GetComponent<RectTransform>();

        return true;
    }

    public void SetInfo(InteractionObject owner)
    {
        _owner = owner;

        //..

        switch(owner.ObjectType)
        {
            case EObjectType.Hero:
            case EObjectType.Monster:
                break;
        }
    }

    private void Update()
    {
        _rect.position = _camera.WorldToScreenPoint(_owner.OverheadPosition);
    }

    public void Refresh(float ratio)
    {
        // StatChangeEvent -> Refresh(need StatComp)
        _hp.value = ratio;
    }

}
