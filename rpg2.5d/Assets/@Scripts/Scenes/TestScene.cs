using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;


        Hero hero = GameObject.Find("HeroPrefab").GetComponent<Hero>();
        
        Managers.Game.Cam.transform.position = hero.transform.position;
        Managers.Game.Cam.Target = hero;

        Managers.Game.PlayerHero = hero;

        return true;
    }
    public override void Clear() { }
}
