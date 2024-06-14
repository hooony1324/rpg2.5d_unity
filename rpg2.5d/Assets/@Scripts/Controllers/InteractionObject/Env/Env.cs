using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Env : InteractionObject
{
    //public Data.EnvData EnvData;

    [SerializeField] private EEnvState _envState = EEnvState.Idle;

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
                break;
            case EEnvState.Dead:
                //Start : CoReserveSpawn : 몇 초뒤 리스폰
                break;
        }
    }
}
