using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class BaseObject : InitBase
{// 월드 내 배치되는 Objectj
    public Vector3 Position => transform.position;
    protected SpriteRenderer Sprite => _spriteRenderer;
    public Animator Anim => _animator;
    //public AnimEventController AEC { get; set; }

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    //private AnimEventController _animEventController;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _spriteRenderer = Util.FindChild<SpriteRenderer>(gameObject);
        _animator = Util.FindChild<Animator>(gameObject);

        return true;
    }

    bool _lookLeft = true;
    public bool LookLeft
    {
        get { return _lookLeft; }
        set
        {
            _lookLeft = value;
            Flip(value);
        }
    }

    protected virtual void Flip(bool flag)
    {
        if (Sprite != null)
            Sprite.flipX = flag;
    }

    protected virtual void OnDisable()
    {
       
    }
}
