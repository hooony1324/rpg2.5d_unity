using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

public class InitBase : MonoBehaviour
{
    protected bool _init = false;

    protected virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }

    private void Awake()
    {
        Init();
    }

}
