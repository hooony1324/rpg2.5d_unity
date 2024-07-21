using Cinemachine;
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


        Managers.Map.LoadMap("BaseMap");

        Hero hero = Managers.Object.Spawn<Hero>(Managers.Game.SaveData.LastWorldPos, 201001);
        Managers.Game.PlayerHero = hero;

        Managers.Game.Cam.Init();
        Managers.Game.Cam.SetTarget(hero.transform);

        for (int i = 0; i < 1; i++)
        {
            {
                Monster monster = Managers.Object.Spawn<Monster>(Vector3.right * 10, 202001);
            }

            {
                Monster monster = Managers.Object.Spawn<Monster>(Vector3.right * 10, 202002);
            }
        }


        {
            //Monster monster = Managers.Object.Spawn<Monster>(Vector3.right * 10, 202003);
        }

        {
            //Monster monster = Managers.Object.Spawn<Monster>(Vector3.right * 10, 202004);
        }



        //Env env = Managers.Object.Spawn<Env>(Vector3.right * 5);

        Managers.UI.CacheAllPopups();

        UI_GameScene sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SceneUI = sceneUI;
        sceneUI.GetComponent<Canvas>().sortingOrder = 1;
        sceneUI.SetInfo();
        
        StartCoroutine(CoSaveGame());
        return true;
    }

    WaitForSeconds wait = new WaitForSeconds(1);
    private IEnumerator CoSaveGame()
    {
        while(true)
        {
            yield return wait;
            Managers.Game.SaveGame();
        }
    }

    public override void Clear() { }
}
