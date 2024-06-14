using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;
using Slider = UnityEngine.UI.Slider;

public class UI_ComboBar : UI_Base
{
    private Creature _owner;
    private Slider _slider;
    private GameObject _handle;
    private RectTransform _detect;

    public float SliderValue { get { return _slider.value; } set { _slider.value = value; } }
    public bool IsInDetectArea => (_startRatio <= _slider.value && _slider.value <= _endRatio);
    float _startRatio = 0;
    float _endRatio = 0;

    private enum GameObjects
    {
        ComboBar,
        DetectArea,
        HandleArea,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<GameObject>(typeof(GameObjects));
        GetComponent<Canvas>().sortingOrder = SortingLayers.HERO + 1;
        _slider = GetObject((int)GameObjects.ComboBar).GetComponent<Slider>();
        _handle = GetObject((int)GameObjects.HandleArea);
        _detect = GetObject((int)GameObjects.DetectArea).GetComponent<RectTransform>();

        _slider.value = 0;

        return true;
    }

    public void SetInfo(Creature owner)
    {
        _owner = owner;

        transform.localPosition = owner.OverheadPosition;

        RefreshDetectArea(0, 0);

    }

    /*
     * 부모 rect기준 비율로 영역 정함
     * startRatio, endRatio : 0.0 ~ 1.0
     */
    public void RefreshDetectArea(float startRatio, float endRatio)
    {
        _startRatio = startRatio;
        _endRatio = endRatio;
        _detect.anchorMin = new Vector2(startRatio, 0);
        _detect.anchorMax = new Vector2(endRatio, 1);
        _detect.offsetMin = new Vector2(0, _detect.offsetMin.y);
        _detect.offsetMax = new Vector2(0, _detect.offsetMax.y);
    }

}
