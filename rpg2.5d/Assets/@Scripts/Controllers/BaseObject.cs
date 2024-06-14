using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class BaseObject : InitBase
{// 월드 내 배치되는 Object
    public Define.EObjectType ObjectType { get; set; }
    public Vector3 Position => transform.position;
    protected SpriteRenderer Sprite => _spriteRenderer;
    public Animator Anim => _animator;
    //public AnimEventController AEC { get; set; }

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    //private AnimEventController _animEventController;

    public Vector3 OverheadPosition => Position + Vector3.up * OverheadOffset;
    protected float OverheadOffset = 1.8f;
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
        if (_spriteRenderer != null)
            _spriteRenderer.flipX = flag;
    }

    public void LookAtTarget(Vector3 targetPos)
    {
        Vector2 dir = targetPos - Position;
        if (dir.x < 0)
            LookLeft = true;
        else if (dir.x > 0)
            LookLeft = false;
    }

    protected virtual void OnDisable()
    {
       
    }

    public float GetSpriteHeight()
    {
        return _spriteRenderer.bounds.center.y;
    }
}
