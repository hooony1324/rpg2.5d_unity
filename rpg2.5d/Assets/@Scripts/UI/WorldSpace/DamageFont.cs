using UnityEngine;
using TMPro;
using DG.Tweening;
using static Define;

public class DamageFont : BaseObject
{
    private TextMeshPro _damageText;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        

        return true;
    }
}
