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

        
        Hero hero = Managers.Object.Spawn<Hero>(Managers.Game.SaveData.LastWorldPos, 201001);
        Managers.Game.PlayerHero = hero;

        for (int i = 0; i < 3; i++)
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



        Env env = Managers.Object.Spawn<Env>(Vector3.right * 5);

        Managers.UI.CacheAllPopups();

        UI_GameScene sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SceneUI = sceneUI;
        sceneUI.GetComponent<Canvas>().sortingOrder = 1;
        sceneUI.SetInfo();

        Managers.Game.Cam.transform.position = hero.transform.position;
        Managers.Game.Cam.Target = hero;

        
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
