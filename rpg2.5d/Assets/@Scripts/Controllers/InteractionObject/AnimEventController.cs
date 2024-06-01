using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEventController : MonoBehaviour
{
    private Action _onTriggeredEvent;
    //private Action _onCompletedEvent;

    public void BindEvent(Action action)
    {
        _onTriggeredEvent = action;
    }

    public void TriggerEvent(AnimationEvent animEvent)
    {
        _onTriggeredEvent?.Invoke();
        Debug.Log(animEvent.data);
    }

    public void ClearEvent()
    {
        _onTriggeredEvent = null;
    }
}
