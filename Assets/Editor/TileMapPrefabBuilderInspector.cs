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
        // removes the builder safely if marked for destruction
        if (((TileMapPrefabBuilder)target).killme && Event.current.type == EventType.Repaint)
        {
            DestroyImmediate((TileMapPrefabBuilder)target);
            return;
        }

        ((TileMapPrefabBuilder)target).size = EditorGUILayout.Vector2Field("Size of Prefab", ((TileMapPrefabBuilder)target).size);
        ((TileMapPrefabBuilder)target).tileSize = EditorGUILayout.FloatField("Preview Tile Size", ((TileMapPrefabBuilder)target).tileSize);
        ((TileMapPrefabBuilder)target).textureRes = EditorGUILayout.IntField("Texture Resolution", ((TileMapPrefabBuilder)target).textureRes);
        ((TileMapPrefabBuilder)target).mat = (Material)EditorGUILayout.ObjectField("Material: ", ((TileMapPrefabBuilder)target).mat, typeof(Material), true);
        ((TileMapPrefabBuilder)target).tileTexture = (Texture2D)EditorGUILayout.ObjectField("Texture: ", ((TileMapPrefabBuilder)target).tileTexture, typeof(Texture2D), true);

        if (!((TileMapPrefabBuilder)target).preview)
        {
            if (GUILayout.Button("Show Preview"))
            {
                if (((TileMapPrefabBuilder)target).tileTexture != null && ((TileMapPrefabBuilder)target).mat != null)
                {
                    ((TileMapPrefabBuilder)target).preview = !((TileMapPrefabBuilder)target).preview;
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
            }
        }
        else
        {
            if (GUILayout.Button("Hide Preview"))
            {
                ((TileMapPrefabBuilder)target).preview = !((TileMapPrefabBuilder)target).preview;
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
