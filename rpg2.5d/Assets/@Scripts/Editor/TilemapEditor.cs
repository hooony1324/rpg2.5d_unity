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
        sb.Append("GameObject�� �̸��� �Ʒ��� �̸����� ������ �� ��� ��밡��");
        GUILayout.Label(sb.ToString());
        sb.Clear();

        sb.AppendLine("[name]_Ground : Combine�� �޽� ����"); 
        sb.AppendLine("[name]_CollisionEditor : CollisionData����");
        GUILayout.Label(sb.ToString());

        if (tilemapName.Contains("Ground"))
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("Walkable�� ���� ����");
            EditorGUILayout.Space(10);

            if (GUILayout.Button("MeshCollider(Generated) ����"))
                GenerateCombinedMeshCollider();

            if (GUILayout.Button("GroundTile��� �����"))
                ClearGroundTiles();
        }

        if (tilemapName.Contains("CollisionEditor"))
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("���� ���������� �� Collision����");
            EditorGUILayout.Space(10);

            if (GUILayout.Button("���� CollisionTile �ڵ�����"))
                GenerateCollision();

            if (GUILayout.Button("CollisionTile��� �����"))
                ClearCollisionTiles();

            if (GUILayout.Button("Toggle Collision Visible"))
                ToggleCollisionVisible();
        }

    }


    #region Ground
    /* 
     * ObjectBrush�� ��ġ�� TileMap, Batch������ �ʹ� ŭ
     * �ϳ��� MeshFilter�� ����
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

                // ���� ť���� ����, UV, �ﰢ�� ������
                Vector3[] meshVertices = m.vertices;
                Vector2[] meshUV = m.uv;
                int[] meshTriangles = m.triangles;

                // ������ ��ü �޽� ���� ����Ʈ�� �߰�
                foreach (Vector3 vertex in meshVertices)
                {
                    vertices.Add(cube.transform.TransformPoint(vertex));
                }

                // UV ��ǥ�� ��ü �޽� UV ����Ʈ�� �߰�
                uv.AddRange(meshUV);

                // �ﰢ�� �ε����� ��ü �޽� �ﰢ�� ����Ʈ�� �߰�
                foreach (int triangle in meshTriangles)
                {
                    triangles.Add(triangle + vertexOffset);
                }

                vertexOffset += meshVertices.Length;
            }

            t.gameObject.SetActive(false);
        }

        // Combine����� Object
        GameObject tilemapCollisionMesh = new GameObject();
        tilemapCollisionMesh.name = "MeshCollider(Generated)";
        
        tilemapCollisionMesh.transform.parent = _tilemap.transform;
        tilemapCollisionMesh.tag = "Ground";
        tilemapCollisionMesh.transform.SetAsFirstSibling();

        // ���ο� �޽� ����
        Mesh combinedMesh = new Mesh();
        combinedMesh.vertices = vertices.ToArray();
        combinedMesh.uv = uv.ToArray();
        combinedMesh.triangles = triangles.ToArray();
        combinedMesh.RecalculateNormals();

        // ���ο� �޽� MeshFilter�� ����
        MeshFilter meshFilterComponent = Util.GetOrAddComponent<MeshFilter>(tilemapCollisionMesh.gameObject);
        meshFilterComponent.sharedMesh = combinedMesh;

        // MeshCollider����
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
