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

        GameObject playerStart = GameObject.Find("@PlayerStart");
        if (playerStart == null)
            GameObject.Instantiate(new GameObject{ name = "@PlayerStart" });

        Hero hero = Managers.Object.Spawn<Hero>(playerStart.transform.position);

        Managers.Game.Cam.transform.position = hero.transform.position;
        Managers.Game.Cam.Target = hero;

        Managers.Game.PlayerHero = hero;
        
        return true;
    }

    public override void Clear()
    {
        
    }
}
