using NSubstitute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class GameScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        //LoadMap


        
       
        Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero, 201001);

        {
            Monster monster = Managers.Object.Spawn<Monster>(Vector3.right * 10, 202001);
        }

        {
            Monster monster = Managers.Object.Spawn<Monster>(Vector3.right * 10, 202002);
        }

        {
            Monster monster = Managers.Object.Spawn<Monster>(Vector3.right * 10, 202003);
        }

        {
            Monster monster = Managers.Object.Spawn<Monster>(Vector3.right * 10, 202004);
        }



        Env env = Managers.Object.Spawn<Env>(Vector3.right * 5);

        Managers.UI.CacheAllPopups();

        UI_GameScene sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SceneUI = sceneUI;
        sceneUI.GetComponent<Canvas>().sortingOrder = 1;
        sceneUI.SetInfo();

        Managers.Game.Cam.transform.position = hero.transform.position;
        Managers.Game.Cam.Target = hero;

        Managers.Game.PlayerHero = hero;
        
        return true;
    }

    public override void Clear()
    {
        
    }
}
