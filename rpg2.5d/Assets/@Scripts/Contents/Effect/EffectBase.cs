using Data;
using NSubstitute.Routing.Handlers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EffectBase : BaseObject
{
    public Creature Owner;      // Effect를 받음
    public Creature Source;     // Effect를 발생시킴
    public EffectData EffectData;
    public EEffectType EffectType;

    [field: SerializeField] protected float Remains { get; set; } = 0;
    protected EEffectSpawnType _spawnType;

    protected bool isLoop = true;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        return true;
    }

    public virtual void SetInfo(EffectData data, InteractionObject owner, InteractionObject source, EEffectSpawnType spawnType)
    {
        EffectData = data;
        EffectType = data.EffectType;
        Owner = owner as Creature;
        Source = source as Creature;
        _spawnType = spawnType;
        isLoop = true;

        if (_spawnType == EEffectSpawnType.External)
            Remains = float.MaxValue;
        else
            Remains = EffectData.TickTime * EffectData.TickCount;

        if (EffectType == EEffectType.Freeze)
            isLoop = false;
    }

    public virtual void ApplyEffect()
    {
        //ShowEffect();

    }
    protected void ShowEffect()
    {
        //Play Animation
    }

    public virtual bool ClearEffect(EEffectClearType clearType)
    {
        if (Owner == null)
            return false;

        switch (clearType)
        {
            case EEffectClearType.TimeOut:
            case EEffectClearType.TriggerOutAoE:
            case EEffectClearType.EndOfCC:
                if (IsCrowdControl() && Owner.IsValid())
                {
                    Owner.CreatureState = Owner.IsGuardActivated ? ECreatureState.Skill : ECreatureState.Idle;
                }
                Owner.Effects.RemoveEffect(this);
                return true;

            case EEffectClearType.ClearSkill:
                Owner.Effects.RemoveEffect(this);
                return true;

            case EEffectClearType.Disable:
                Owner.Effects.RemoveEffect(this);
                break;
        }

        return false;
    }

    protected virtual void ProcessDot() 
    {

    }

    protected IEnumerator StartTimer()
    {
        if (EffectType == EEffectType.Airborne || EffectType == EEffectType.Knockback)
        {
            yield break;
        }

        float tickTimer = 0f;

        ProcessDot();

        if (EffectType == Define.EEffectType.Instant)
        {
            yield return new WaitForSeconds(1f);
        }
        else
        {
            while (Remains > 0)
            {
                Remains -= Time.deltaTime;
                tickTimer += Time.deltaTime;

                // 틱마다 ProcessDotTick 호출
                if (tickTimer >= EffectData.TickTime)
                {
                    ProcessDot();
                    tickTimer -= EffectData.TickTime;
                }

                yield return null;
            }
        }
        Remains = 0;
        ClearEffect(EEffectClearType.TimeOut);
    }

    private bool IsCrowdControl()
    {
        switch (EffectType)
        {
            case EEffectType.Knockback:
            case EEffectType.Airborne:
            case EEffectType.Stun:
            case EEffectType.Pull:
            case EEffectType.Freeze:
                return true;
        }

        return false;
    }
}
