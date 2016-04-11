using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileMapPrefabBuilder))]
public class TileMapPrefabBuilderInspector : Editor {

    void OnEnable()
    {
        ((TileMapPrefabBuilder)target).tileMap = ((TileMapPrefabBuilder)target).gameObject.GetComponent<TileMap>();
    }

    public override void OnInspectorGUI()
    {
        ((TileMapPrefabBuilder)target).size = EditorGUILayout.Vector2Field("Size of Prefab", ((TileMapPrefabBuilder)target).size);
        ((TileMapPrefabBuilder)target).tileSize = EditorGUILayout.FloatField("Preview Tile Size", ((TileMapPrefabBuilder)target).tileSize);
        ((TileMapPrefabBuilder)target).textureRes = EditorGUILayout.IntField("Texture Resolution", ((TileMapPrefabBuilder)target).textureRes);
        ((TileMapPrefabBuilder)target).mat = (Material)EditorGUILayout.ObjectField("Material: ", ((TileMapPrefabBuilder)target).mat, typeof(Material), true);
        ((TileMapPrefabBuilder)target).tileTexture = (Texture2D)EditorGUILayout.ObjectField("Texture: ", ((TileMapPrefabBuilder)target).tileTexture, typeof(Texture2D), true);

        if (GUILayout.Button("Preview"))
        {
            ((TileMapPrefabBuilder)target).preview = !((TileMapPrefabBuilder)target).preview;
            if (((TileMapPrefabBuilder)target).preview && ((TileMapPrefabBuilder)target).tileTexture != null && ((TileMapPrefabBuilder)target).mat != null)
            {
                GenerateTileMap();
                ((TileMapPrefabBuilder)target).tg = ((TileMapPrefabBuilder)target).gameObject.GetComponent<TileGraphics>();
                if (((TileMapPrefabBuilder)target).tg == null)
                {
                    ((TileMapPrefabBuilder)target).gameObject.AddComponent<MeshFilter>().hideFlags = HideFlags.HideInInspector;
                    ((TileMapPrefabBuilder)target).gameObject.AddComponent<MeshRenderer>().sharedMaterial = ((TileMapPrefabBuilder)target).mat;
                    ((TileMapPrefabBuilder)target).gameObject.GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
                    ((TileMapPrefabBuilder)target).tg = ((TileMapPrefabBuilder)target).gameObject.AddComponent<TileGraphics>();
                    ((TileMapPrefabBuilder)target).tg.hideFlags = HideFlags.HideInInspector;
                }
                ((TileMapPrefabBuilder)target).tg.tileTexture = ((TileMapPrefabBuilder)target).tileTexture;
                ((TileMapPrefabBuilder)target).tg.tileResolution = ((TileMapPrefabBuilder)target).textureRes;
                ((TileMapPrefabBuilder)target).tg.BuildMesh((((TileMapPrefabBuilder)target).tileMap), ((TileMapPrefabBuilder)target).tileSize);

            }
            else
            {
                DestroyImmediate(((TileMapPrefabBuilder)target).tg);
                DestroyImmediate(((TileMapPrefabBuilder)target).gameObject.GetComponent<MeshRenderer>());
                DestroyImmediate(((TileMapPrefabBuilder)target).gameObject.GetComponent<MeshFilter>());
            }
        }
    }

    void GenerateTileMap()
    {
        // temporary random tiles algorithm
        ((TileMapPrefabBuilder)target).tileMap.NewTileMap(((TileMapPrefabBuilder)target).tileMap.sizeX, ((TileMapPrefabBuilder)target).tileMap.sizeY);
        for (int y = 0; y < ((TileMapPrefabBuilder)target).tileMap.sizeY; y++)
        {
            for (int x = 0; x < ((TileMapPrefabBuilder)target).tileMap.sizeX; x++)
            {
                ((TileMapPrefabBuilder)target).tileMap.SetTile(x, y, new Tile(x, y, (Tile.TYPE)(int)UnityEngine.Random.Range(.8f, 2f)));
            }
        }
    }
}
