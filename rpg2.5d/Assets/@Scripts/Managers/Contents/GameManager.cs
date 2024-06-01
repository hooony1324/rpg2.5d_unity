using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public void Init()
    {

    }

    private CameraController _cam;

    public CameraController Cam
    {
        get
        {
            if (_cam == null)
            {
                _cam = Object.FindObjectOfType<CameraController>();
            }

            return _cam;
        }
    }
}
