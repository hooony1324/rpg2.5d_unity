using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : BaseObject
{// 서로 상호작용 가능 Object

    public event Action<InteractionObject> EventOnDead;

    public virtual void OnDamage(InteractionObject attacker, float value)
    {
        
    }

    protected virtual void OnDead()
    {
        EventOnDead?.Invoke(this);
    }
}
