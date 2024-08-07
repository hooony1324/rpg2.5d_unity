using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MonsterSpawner : UI_Base
{
    private enum GameObjects
    {
        MonsterDropdown,
    }

    private enum Buttons
    {
        MonsterSpawnButton,
    }

    int _index = 0;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        var namesList = Managers.Data.MonsterDic.Values.Select(x => Managers.GetText(x.TextID, Define.ETextType.Name)).ToList();

        TMP_Dropdown dropdown = GetObject((int)GameObjects.MonsterDropdown).gameObject.GetComponent<TMP_Dropdown>();
        dropdown.AddOptions(namesList);

        dropdown.onValueChanged.AddListener(OnValueChanged);
        GetButton((int)Buttons.MonsterSpawnButton).gameObject.BindEvent(Spawn);

        return true;
    }

    private void OnValueChanged(int index)
    {
        _index = index;
    }

    private void Spawn()
    {
        Monster monster = Managers.Object.Spawn<Monster>(Managers.Game.PlayerHero.Position + Vector3.right * 3, 202001 + _index);
    }
}
