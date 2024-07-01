using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using static Define;

public class CCBase : EffectBase
{
    protected ECreatureState lastState;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;


        return true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        _knockbackCoroutine = null;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        lastState = Owner.CreatureState;
        if (lastState == ECreatureState.OnDamaged || lastState == ECreatureState.Death)
        {
            ClearEffect(EEffectClearType.Disable);
            return;
        }

        Owner.CreatureState = ECreatureState.OnDamaged;

        switch (EffectData.EffectType)
        {
            case EEffectType.Knockback:
                if (_knockbackCoroutine == null)
                {
                    _knockbackCoroutine = StartCoroutine(CoKnockBack());
                }
                break;
            case EEffectType.Airborne:
                StopCoroutine((DoAirborn(lastState)));
                StartCoroutine(DoAirborn(lastState));
                break;
            case EEffectType.Stun:
            case EEffectType.Pull:
            case EEffectType.Freeze:
                StartCoroutine(StartTimer());
                break;
        }
    }

    public override bool ClearEffect(EEffectClearType clearType)
    {
        base.ClearEffect(clearType);

        return false;
    }

    #region KnockBack
    private Coroutine _knockbackCoroutine;

    IEnumerator CoKnockBack()
    {
        Vector3 dir = (Owner.Position - Source.Position).normalized;
        Owner.LookAtTarget(Source.Position);
        
        yield return Owner.CoLerpInDirection(dir, 0.15f);

        yield return new WaitForSeconds(0.85f);

        ClearEffect(EEffectClearType.EndOfCC);
        _knockbackCoroutine = null;
    }
    #endregion

    #region Airborne
    IEnumerator DoAirborn(ECreatureState lastState)
    {
        yield return null;
    }
    #endregion
}
