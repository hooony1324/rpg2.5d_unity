using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using VFolders.Libs;
using UnityEngine.Diagnostics;




#if UNITY_EDITOR
using Newtonsoft.Json;
using UnityEditor;
#endif

public class MapEditor : MonoBehaviour
{
#if UNITY_EDITOR

    [MenuItem("Tools/GenerateMap %#m")]
    private static void GenerateMap()
    {
        

        GameObject tm = Util.FindChild(Selection.activeGameObject, "Tilemap_CollisionEditor", true);
        Grid grid = Selection.activeGameObject.GetComponent<Grid>();
        
        GameObject[] gameObjects = Selection.gameObjects;

        // GroundTile의 위치 정보로 X by Z 크기의 text데이터 생성
        Tilemap collisionMap = tm.GetComponent<Tilemap>();
        //collisionMap.cellBounds.

        int xMin = collisionMap.cellBounds.xMin;
        int xMax = collisionMap.cellBounds.xMax;
        int zMin = collisionMap.cellBounds.zMin;
        int zMax = collisionMap.cellBounds.zMax;
        
        
        char[,] groundTiles = new char[zMax - zMin, xMax - xMin];
        groundTiles.Initialize(Define.MAP_TOOL_WALL);
        
        foreach (Transform stage in Selection.activeGameObject.transform.GetChildren())
        {
            Transform tilemap_ground = stage.Find("Tilemap_Ground");
            if (tilemap_ground != null)
            {
                foreach(Transform child in tilemap_ground.transform.GetChildren())
                {
                    if (child.name.Equals("GroundTile"))
                    {

                    }
                }
            }
        }


    }


#endif
}
