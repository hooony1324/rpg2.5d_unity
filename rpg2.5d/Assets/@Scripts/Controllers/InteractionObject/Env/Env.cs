using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Env : InteractionObject
{
    //public Data.EnvData EnvData;

    [SerializeField] private EEnvState _envState = EEnvState.Idle;
    GameObject meshObj;

    public float MaxHp { get; set; }
    [field: SerializeField]public float Hp { get; set; }



    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Env;

        meshObj = Util.FindChild(gameObject, "Mesh");

        MaxHp = 100;
        Hp = MaxHp;


        return true;
    }

    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        EnvState = EEnvState.Idle;
    }

    public EEnvState EnvState
    {
        get => _envState;
        set
        {
            _envState = value;
            UpdateAnimation();
        }
    }

    protected void UpdateAnimation()
    {
        switch (EnvState)
        {
            case EEnvState.Idle:
                break;
            case EEnvState.OnDamaged:
                PlayDamagedSequence();
                break;
            case EEnvState.Dead:
                //Start : CoReserveSpawn : �� �ʵ� ������
                PlayFadeOutSequence();
                break;
        }
    }

    public override void OnDamage(InteractionObject attacker, float value)
    {
        base.OnDamage(attacker, value);

        if (EnvState != EEnvState.Idle)
            return;

        _hurtFlash.Flash();

        EnvState = EEnvState.OnDamaged;

        Hp = Mathf.Clamp(Hp - value, 0, MaxHp);
        float ratio = Hp / MaxHp;
        _hpBar.Refresh(ratio);

        Managers.Object.ShowDamageFont(OverheadPosition, value, transform, EDamageResult.Hit);

    }

    [SerializeField] float duration = 0.5f; // ���� �ð�
    [SerializeField] float positionStrength = 0.1f; // ���� ����
    [SerializeField] int vibrato = 30; // ���� Ƚ��
    [SerializeField] float randomness = 90f;

    void PlayDamagedSequence()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(meshObj.transform.DOShakePosition(duration, positionStrength, vibrato, randomness));
        seq.OnComplete(() =>
        {
            if (Hp <= 0)
            {
                EnvState = EEnvState.Dead;
            }
            else
            {
                EnvState = EEnvState.Idle;
            }
        });
    }

    float fadeOutDuration = 2.0f;
    void PlayFadeOutSequence()
    {
        Vector3 targetPosition = Position + Vector3.down * 3.0f;

        Sequence seq = DOTween.Sequence();

        seq.Append(meshObj.transform.DOShakePosition(fadeOutDuration, positionStrength, 120, randomness))
        .Insert(0, transform.DOMove(targetPosition, fadeOutDuration).SetEase(Ease.Linear))
        .OnComplete(() =>
        {
            Managers.Object.Despawn(this);
        })
        .Play();
    }

}
