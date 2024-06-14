using Codice.Client.Commands.TransformerRule;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using VFolders.Libs;

[CustomEditor(typeof(Tilemap))]
public class TilemapEditor : Editor
{
#if UNITY_EDITOR
    private Tilemap _tilemap;

    public void OnEnable()
    {
        
    }

    public void OnDisable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _tilemap = (Tilemap)target;

        string tilemapName = _tilemap.gameObject.name;

        // Tool Explanation
        EditorGUILayout.Space(10);
        GUILayout.Label("Tools", EditorStyles.boldLabel);

        StringBuilder sb = new StringBuilder();
        sb.Append("GameObject의 이름을 아래의 이름으로 설정시 툴 기능 사용가능");
        GUILayout.Label(sb.ToString());
        sb.Clear();

        sb.AppendLine("[name]_Ground : Combine한 메쉬 생성"); 
        sb.AppendLine("[name]_CollisionEditor : CollisionData생성");
        GUILayout.Label(sb.ToString());

        if (tilemapName.Contains("Ground"))
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("Walkable한 영역 생성");
            EditorGUILayout.Space(10);

            if (GUILayout.Button("MeshCollider(Generated) 생성"))
                GenerateCombinedMeshCollider();

            if (GUILayout.Button("GroundTile모두 지우기"))
                ClearGroundTiles();
        }

        if (tilemapName.Contains("CollisionEditor"))
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("고정 지형지물들 의 Collision생성");
            EditorGUILayout.Space(10);

            if (GUILayout.Button("지형 CollisionTile 자동생성"))
                GenerateCollision();

            if (GUILayout.Button("CollisionTile모두 지우기"))
                ClearCollisionTiles();

            if (GUILayout.Button("Toggle Collision Visible"))
                ToggleCollisionVisible();
        }

    }


    #region Ground
    /* 
     * ObjectBrush로 배치한 TileMap, Batch사이즈 너무 큼
     * 하나의 MeshFilter로 통합
     */
    private void GenerateCombinedMeshCollider()
    {
        List<Transform> cubes = _tilemap.transform.GetChildren();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        int vertexOffset = 0;

        foreach (Transform t in cubes)
        {
            if (!t.name.Contains("GroundTile"))
                continue;

            GameObject cube = t.gameObject;
            MeshFilter meshFilter = cube.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                Mesh m = meshFilter.sharedMesh;

                // 현재 큐브의 정점, UV, 삼각형 데이터
                Vector3[] meshVertices = m.vertices;
                Vector2[] meshUV = m.uv;
                int[] meshTriangles = m.triangles;

                // 정점을 전체 메쉬 정점 리스트에 추가
                foreach (Vector3 vertex in meshVertices)
                {
                    vertices.Add(cube.transform.TransformPoint(vertex));
                }

                // UV 좌표를 전체 메쉬 UV 리스트에 추가
                uv.AddRange(meshUV);

                // 삼각형 인덱스를 전체 메쉬 삼각형 리스트에 추가
                foreach (int triangle in meshTriangles)
                {
                    triangles.Add(triangle + vertexOffset);
                }

                vertexOffset += meshVertices.Length;
            }

            t.gameObject.SetActive(false);
        }

        // Combine결과물 Object
        GameObject tilemapCollisionMesh = new GameObject();
        tilemapCollisionMesh.name = "MeshCollider(Generated)";
        
        tilemapCollisionMesh.transform.parent = _tilemap.transform;
        tilemapCollisionMesh.tag = "Ground";
        tilemapCollisionMesh.transform.SetAsFirstSibling();

        // 새로운 메쉬 생성
        Mesh combinedMesh = new Mesh();
        combinedMesh.vertices = vertices.ToArray();
        combinedMesh.uv = uv.ToArray();
        combinedMesh.triangles = triangles.ToArray();
        combinedMesh.RecalculateNormals();

        // 새로운 메쉬 MeshFilter에 적용
        MeshFilter meshFilterComponent = Util.GetOrAddComponent<MeshFilter>(tilemapCollisionMesh.gameObject);
        meshFilterComponent.sharedMesh = combinedMesh;

        // MeshCollider생성
        MeshCollider meshCollider = Util.GetOrAddComponent<MeshCollider>(tilemapCollisionMesh.gameObject);
        meshCollider.sharedMesh = combinedMesh;

        MeshRenderer meshRenderer = Util.GetOrAddComponent<MeshRenderer>(tilemapCollisionMesh.gameObject);
        Material protoMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/@Resources/Materials/Blue_Prototype.mat");
        meshRenderer.sharedMaterial = protoMat;
    }

    void ClearGroundTiles()
    {
        foreach (Transform t in _tilemap.transform.GetChildren())
        {
            if (t.name.Contains("GroundTile"))
                DestroyImmediate(t.gameObject);
        }
    }
    #endregion


    #region CollisionEditor
    void GenerateCollision()
    {

    }

    void ClearCollisionTiles()
    {
        foreach (Transform t in _tilemap.transform.GetChildren())
        {
            if (t.name.Contains("CollisionTile"))
                DestroyImmediate(t.gameObject);
        }
    }

    void ToggleCollisionVisible()
    {
        foreach (Transform t in _tilemap.transform.GetChildren())
        {
            if (t.name.Contains("CollisionTile"))
                t.gameObject.SetActive(!t.gameObject.active);
        }
    }
    #endregion

    public void OnSceneGUI()
    {

        if (target.name.Contains("CollisionEditor"))
        {
            Handles.color = Color.red;
            Handles.DrawWireCube(_tilemap.origin, _tilemap.size);
        }
    }
#endif
}
