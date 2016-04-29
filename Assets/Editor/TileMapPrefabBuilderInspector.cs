using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileMapPrefabBuilder))]
public class TileMapPrefabBuilderInspector : Editor {

    Vector2 selectorCoord = new Vector2();
    Vector2 selectorSnap = new Vector2();
    Tile.TYPE selectorType = Tile.TYPE.none;

    bool topExp;
    bool botExp;
    bool leftExp;
    bool rightExp;

    Vector2 size;
    Vector2 newSize;
    //serialized fields
    SerializedProperty tileSize;
    SerializedProperty mat;
    SerializedProperty tileDefs;
    SerializedProperty preview;
    SerializedProperty tg;

    void OnEnable()
    {
        ((TileMapPrefabBuilder)target).tileMap = ((TileMapPrefabBuilder)target).gameObject.GetComponent<TileMap>();
        tileSize = serializedObject.FindProperty("tileSize");
        mat = serializedObject.FindProperty("mat");
        tileDefs = serializedObject.FindProperty("tileDefs");
        preview = serializedObject.FindProperty("preview");
        tg = serializedObject.FindProperty("tg");
        size = ((TileMapPrefabBuilder)target).tileMap.size;
    }

    public override void OnInspectorGUI()
    {
        // removes the builder safely if marked for destruction
        if (((TileMapPrefabBuilder)target).killme && Event.current.type == EventType.Repaint)
        {
            DestroyImmediate((TileMapPrefabBuilder)target);
            return;
        }

        newSize = EditorGUILayout.Vector2Field("Size of Prefab", ((TileMapPrefabBuilder)target).tileMap.size);
        if (newSize != size)
            ((TileMapPrefabBuilder)target).tileMap.NewTileMap((int)newSize.x, (int)newSize.y);
        size = newSize;

        tileSize.floatValue = EditorGUILayout.FloatField("Preview Tile Size", tileSize.floatValue);
        mat.objectReferenceValue = (Material)EditorGUILayout.ObjectField("Material", mat.objectReferenceValue, typeof(Material), true);
        tileDefs.objectReferenceValue = (TileTextureDefs)EditorGUILayout.ObjectField("Texture Defs", tileDefs.objectReferenceValue, typeof(TileTextureDefs), true);

        if (GUILayout.Button("Build/Reset Tilemap"))
            GenerateTileMap();

        EditorGUILayout.Separator();

        if (!preview.boolValue)
        {
            GUI.color = Color.green;
            if (GUILayout.Button("Show Preview"))
            {
                if (((TileMapPrefabBuilder)target).mat != null)
                {
                    preview.boolValue = !preview.boolValue;
                    tg.objectReferenceValue = ((TileMapPrefabBuilder)target).gameObject.GetComponent<TileGraphics>();
                    if (tg.objectReferenceValue == null)
                    {
                        ((TileMapPrefabBuilder)target).gameObject.AddComponent<MeshFilter>().hideFlags = HideFlags.HideInInspector;
                        ((TileMapPrefabBuilder)target).gameObject.AddComponent<MeshRenderer>().sharedMaterial = ((TileMapPrefabBuilder)target).mat;
                        ((TileMapPrefabBuilder)target).gameObject.GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
                        tg.objectReferenceValue = ((TileMapPrefabBuilder)target).gameObject.AddComponent<TileGraphics>();
                        ((TileGraphics)tg.objectReferenceValue).hideFlags = HideFlags.HideInInspector;
                    }
                    ((TileGraphics)tg.objectReferenceValue).tileDefs = (TileTextureDefs)tileDefs.objectReferenceValue;
                    ((TileGraphics)tg.objectReferenceValue).BuildMesh((((TileMapPrefabBuilder)target).tileMap), ((TileMapPrefabBuilder)target).tileSize);
                }
            }
            GUI.color = Color.white;
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUI.color = Color.red;
            if (GUILayout.Button("Hide Preview"))
            {
                preview.boolValue = !preview.boolValue;
                DestroyImmediate(tg.objectReferenceValue);
                DestroyImmediate(((TileMapPrefabBuilder)target).gameObject.GetComponent<MeshRenderer>());
                DestroyImmediate(((TileMapPrefabBuilder)target).gameObject.GetComponent<MeshFilter>());
            }
            GUI.color = Color.white;
            if (GUILayout.Button("Update Preview"))
            {
                ((TileGraphics)tg.objectReferenceValue).BuildMesh((((TileMapPrefabBuilder)target).tileMap), ((TileMapPrefabBuilder)target).tileSize);
            }
            GUILayout.EndHorizontal();
        }

        selectorCoord = EditorGUILayout.Vector2Field("Coordinate to Edit", selectorCoord);
        selectorSnap = EditorGUILayout.Vector2Field("Selector to Snap by", selectorSnap);
        selectorType = (Tile.TYPE)EditorGUILayout.EnumPopup(selectorType);

        GUI.color = new Color(1f, 1f, .4f, 1f);
        if (GUILayout.Button("Change Selected Tile"))
        {
            ((TileMapPrefabBuilder)target).tileMap.SetTile((int)selectorCoord.x, (int)selectorCoord.y, selectorType);
            selectorCoord += selectorSnap;
            ((TileGraphics)tg.objectReferenceValue).BuildMesh((((TileMapPrefabBuilder)target).tileMap), ((TileMapPrefabBuilder)target).tileSize);
        }
        GUI.color = Color.white;

        GUILayout.Space(10);
        GUILayout.Label("Keep these hidden when not in use");

        topExp = EditorGUILayout.Foldout(topExp, "Top Openings");
        if (topExp)
        {
            ((TileMapPrefabBuilder)target).tileMap.topSize = EditorGUILayout.IntField("Size", ((TileMapPrefabBuilder)target).tileMap.topSize);
            int[] top = ((TileMapPrefabBuilder)target).tileMap.top;
            for(int i=0; i < top.Length; i++)
            {
                top[i] = EditorGUILayout.IntField(i + ":", top[i]);
            }
            ((TileMapPrefabBuilder)target).tileMap.top = top;
        }

        botExp = EditorGUILayout.Foldout(botExp, "Bot Openings");
        if (botExp)
        {
            ((TileMapPrefabBuilder)target).tileMap.botSize = EditorGUILayout.IntField("Size", ((TileMapPrefabBuilder)target).tileMap.botSize);
            int[] bot = ((TileMapPrefabBuilder)target).tileMap.top;
            for (int i = 0; i < bot.Length; i++)
            {
                bot[i] = EditorGUILayout.IntField(i + ":", bot[i]);
            }
            ((TileMapPrefabBuilder)target).tileMap.top = bot;
        }

        leftExp = EditorGUILayout.Foldout(leftExp, "Left Openings");
        if (leftExp)
        {
            ((TileMapPrefabBuilder)target).tileMap.leftSize = EditorGUILayout.IntField("Size", ((TileMapPrefabBuilder)target).tileMap.leftSize);
            int[] left = ((TileMapPrefabBuilder)target).tileMap.top;
            for (int i = 0; i < left.Length; i++)
            {
                left[i] = EditorGUILayout.IntField(i + ":", left[i]);
            }
            ((TileMapPrefabBuilder)target).tileMap.top = left;
        }

        rightExp = EditorGUILayout.Foldout(rightExp, "Right Openings");
        if (rightExp)
        {
            ((TileMapPrefabBuilder)target).tileMap.rightSize = EditorGUILayout.IntField("Size", ((TileMapPrefabBuilder)target).tileMap.rightSize);
            int[] right = ((TileMapPrefabBuilder)target).tileMap.right;
            for (int i = 0; i < right.Length; i++)
            {
                right[i] = EditorGUILayout.IntField(i + ":", right[i]);
            }
            ((TileMapPrefabBuilder)target).tileMap.top = right;
        }

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        EditorUtility.SetDirty(((TileMapPrefabBuilder)target).tileMap);
    }

    void GenerateTileMap()
    {
        // temporary random tiles algorithm
        ((TileMapPrefabBuilder)target).tileMap.NewTileMap(((TileMapPrefabBuilder)target).tileMap.sizeX, ((TileMapPrefabBuilder)target).tileMap.sizeY);
        for (int y = 0; y < ((TileMapPrefabBuilder)target).tileMap.sizeY; y++)
        {
            for (int x = 0; x < ((TileMapPrefabBuilder)target).tileMap.sizeX; x++)
            {
                ((TileMapPrefabBuilder)target).tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.none));
            }
        }
    }
}
