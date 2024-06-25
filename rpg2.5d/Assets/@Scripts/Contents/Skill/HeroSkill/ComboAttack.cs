using Castle.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using static UnityEngine.UI.GridLayoutGroup;

public class ComboAttack : SkillBase
{
    
    UI_ComboBar _comboBar;
    int _curComboIndex = 0;
    int _maxComboIndex = 0;
    List<Pair<float, float>> _comboInfo = new List<Pair<float, float>>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SkillType = ESkillType.ComboSkill;

        _comboBar = Managers.UI.MakeWorldSpaceUI<UI_ComboBar>();

        return true;
    }


    public override void SetInfo(int skillId)
    {
        base.SetInfo(skillId);

        // AttackEvent 시점에 따라 콤보 DetectArea 설정됨
        foreach (AnimationClip clip in Owner.Anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Contains("Attack"))
            {
                float normalizedDetectTime = clip.events[0].time / clip.length;
                _comboInfo.Add(new Pair<float, float>(normalizedDetectTime, 0.25f));
            }
        }

        _comboBar.SetInfo(Owner);
        _comboBar.gameObject.SetActive(false);
        _maxComboIndex = _comboInfo.Count - 1;

        _targetMask = LayerMask.GetMask("Monster");
    }

    public override void CancelSkill()
    {
        _curComboIndex = 0;
        _activated = true;
        _comboBar.SliderValue = 0;
        _comboBar.gameObject.SetActive(false);
        Owner.CreatureState = ECreatureState.Idle;
        Owner.Rigid.velocity = Vector3.zero;
    }
    

    public override void DoSkill()
    {
        PlayAnimation(_curComboIndex++);
        //Dash();

        if (_activated == false)
            return;

        _activated = false;
        StartComboAttack();
    }


    void StartComboAttack()
    {
        float detectStart = Mathf.Clamp01(_comboInfo[_curComboIndex].First - _comboInfo[_curComboIndex].Second / 2);
        float detectEnd = Mathf.Clamp01(_comboInfo[_curComboIndex].First + _comboInfo[_curComboIndex].Second / 2);

        _comboBar.RefreshDetectArea(detectStart, detectEnd);
        _comboBar.gameObject.SetActive(true);

        if (_combo == null)
            _combo = StartCoroutine(CoComboAttack());
        else
        {
            StopCoroutine(_combo);
            _combo = StartCoroutine(CoComboAttack());
        }
    }

    Coroutine _combo = null;
    IEnumerator CoComboAttack()
    {
        float totalTime = 1.0f;
        float startTime = Time.time;
        float normalizedTime = 0f;

        yield return null;

        while (!(Input.GetMouseButtonDown((int)MouseButton.Left)))
        {
            normalizedTime = (Time.time - startTime) / totalTime;
            if (normalizedTime >= 1.0f)
            {
                CancelSkill();
                yield break;
            }

            _comboBar.SliderValue = normalizedTime;

            yield return null;
        }

        if (_comboBar.IsInDetectArea)
        {// 콤보 성공
            PlayAnimation(_curComboIndex++);
            //Dash();
            if (_curComboIndex <= _maxComboIndex)
            {
                StartComboAttack();
                yield break;
            }
            else
            {// 마지막 타격
                float remainTime = 1 - normalizedTime * totalTime;
                float exitTime = 0.7f;
                _comboBar.gameObject.SetActive(false);
                Invoke("CancelSkill", remainTime * exitTime);
                yield break;
            }
        }

        CancelSkill();
        _combo = null;
    }

    void PlayAnimation(int comboIdx)
    {
        Owner.Anim.SetFloat("MeleeAttackBlend", comboIdx);
        Owner.Anim.SetTrigger(AnimName.MELEEATTACK);
    }


    public void OnAttackEvent()
    {
        Vector3 frontPosition = Owner.Position + (Owner.LookLeft ? Vector3.left : Vector3.right) + Vector3.up;

        Collider[] hitColliders = Physics.OverlapBox(frontPosition, Vector3.one * 2, Quaternion.identity, _targetMask);

        if (hitColliders.Length > 0)
        {
            Debug.Log($"{hitColliders[0].name} hit!!");
        }
        
    }
    
    private void OnDrawGizmos()
    {
        Vector3 frontPosition = Owner.Position + (Owner.LookLeft ? Vector3.left : Vector3.right) + Vector3.up;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(frontPosition, Vector3.one * 2);
    }


}
