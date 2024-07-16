using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class InputController : InitBase
{
    Hero _owner;
    Vector2 _inputDir = Vector2.zero;

    public void Init(Hero hero)
    {
        _owner = hero;
    }

    public void HandleInput()
    {
        _inputDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            _inputDir.y += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            _inputDir.y -= 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            _inputDir.x += 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _inputDir.x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_owner.TargetItemHolder != null)
            {
                _owner.TargetItemHolder.AcquireItem();
                _owner.TargetItemHolder = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Managers.UI.GetSceneUI<UI_GameScene>().ToggleInventoryPopup();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            
        }

        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            //UI Click
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            _owner.Skills.TrySkill(ESkillSlot.Default);
        }
    }

    public void HandleMovement()
    {
        _owner.MoveDir = DirVec.ZERO;

        if (!(_inputDir.sqrMagnitude > 0))
        {
            _owner.HeroMoveState = EHeroMoveState.None;
        }
        else
        {
            _owner.HeroMoveState = EHeroMoveState.ForceMove;
            float angle = Mathf.Atan2(_inputDir.x, _inputDir.y) * 180 / Mathf.PI;
            if (angle > 15f && angle <= 75f)
                _owner.MoveDir = DirVec.UP_LEFT;
            else if (angle > 75f && angle <= 105f)
                _owner.MoveDir = DirVec.LEFT;
            else if (angle > 105f && angle <= 160f)
                _owner.MoveDir = DirVec.DOWN_LEFT;
            else if (angle > 160f || angle <= -160f)
                _owner.MoveDir = DirVec.DOWN;
            else if (angle < -15f && angle >= -75f)
                _owner.MoveDir = DirVec.UP_RIGHT;
            else if (angle < -75f && angle >= -105f)
                _owner.MoveDir = DirVec.RIGHT;
            else if (angle < -105f && angle >= -160f)
                _owner.MoveDir = DirVec.DOWN_RIGHT;
            else
                _owner.MoveDir = DirVec.UP;
        }
    }
}
