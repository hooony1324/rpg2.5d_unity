using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager
{
    private Hero _playerHero;
    public Hero PlayerHero
    {
        get => _playerHero;
        set { _playerHero = value; }
    }

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
