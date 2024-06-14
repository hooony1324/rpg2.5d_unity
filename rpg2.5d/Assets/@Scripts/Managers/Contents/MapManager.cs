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
        CellGrid = map.GetComponent<Grid>();
    }

    void DestroyMap()
    {

    }



    #region Grid
    public Grid CellGrid { get; private set; }

    private int _minX;
    private int _maxX;
    private int _minZ;
    private int _maxZ;

    //private int _minY;
    //private int _maxY;
    public Vector3Int World2Cell(Vector3 worldPos)
    {
        return CellGrid.WorldToCell(worldPos);
    }

    public Vector3 Cell2World(Vector3Int cellPos)
    {
        return CellGrid.CellToWorld(cellPos);
    }

    #endregion


}
