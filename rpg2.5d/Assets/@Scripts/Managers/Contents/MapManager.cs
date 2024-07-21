using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MapManager
{
    public GameObject Map { get; private set; }

    private ECellCollisionType[,] _collision;

    public void LoadMap(string mapName)
    {
        DestroyMap();

        GameObject map = Managers.Resource.Instantiate(mapName);
        map.transform.position = Vector3.zero;
        map.name = $"@Map_{mapName}";

        Map = map;
    }

    void DestroyMap()
    {
        if (Map != null)
            Managers.Resource.Destroy(Map);
    }

}
