using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileMapPrefabBuilder))]
public class TileMapPrefabBuilderInspector : Editor {

    Vector3 mouseCoord = new Vector3();

    Vector2 size;
    Vector2 newSize;
    //serialized fields
    SerializedProperty tileSize;
    SerializedProperty tileDefs;
    SerializedProperty preview;
    SerializedProperty tg;
    SerializedProperty painting;
    SerializedProperty tileHighlight;
    SerializedProperty tilePaint;

    void OnEnable()
    {
        tileSize = serializedObject.FindProperty("tileSize");
        tileDefs = serializedObject.FindProperty("tileDefs");
        preview = serializedObject.FindProperty("preview");
        tg = serializedObject.FindProperty("tg");
        painting = serializedObject.FindProperty("painting");
        tileHighlight = serializedObject.FindProperty("tileHighlight");
        tilePaint = serializedObject.FindProperty("tilePaint");
        size = ((TileMapPrefabBuilder)target).tileMap.size;
        
        //((TileMapPrefabBuilder)target).gameObject.GetComponent<SpriteRenderer>().hideFlags = HideFlags.HideInInspector;
        tg.objectReferenceValue = ((TileMapPrefabBuilder)target).gameObject.GetComponent<TileGraphics>();
        //((TileGraphics)tg.objectReferenceValue).hideFlags = HideFlags.HideInInspector;
    }

    void OnSceneGUI()
    {
        mouseCoord = new Vector3(Event.current.mousePosition.x, Event.current.mousePosition.y);
        mouseCoord.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mouseCoord.y; // fixes inverted mouse data
        mouseCoord = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mouseCoord); // convert the position from screen to world coordinates
        mouseCoord.z = 0;

        if (painting.boolValue)
        {
            Vector2 tileCoord = new Vector2((mouseCoord.x - ((TileMapPrefabBuilder)target).transform.position.x) / tileSize.floatValue, (mouseCoord.y - ((TileMapPrefabBuilder)target).transform.position.y) / tileSize.floatValue);

            if (tileCoord.x > 0 && tileCoord.x < size.x && tileCoord.y > 0 && tileCoord.y < size.y)
            {
                tileHighlight.vector2Value = new Vector2((int)tileCoord.x, (int)tileCoord.y);

                Event e = Event.current;
                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                EventType eventType = e.GetTypeForControl(controlID);

                if (e.isMouse && e.button == 0 && (e.type == EventType.MouseDrag || e.type == EventType.MouseDown || e.type == EventType.MouseUp))
                {
                    ((TileMapPrefabBuilder)target).PaintTile((int)tileCoord.x, (int)tileCoord.y, (Tile.TYPE)tilePaint.enumValueIndex);
                    switch (eventType)
                    {
                        case EventType.MouseDown:
                            GUIUtility.hotControl = controlID;
                            break;
                        case EventType.MouseUp:
                            GUIUtility.hotControl = 0;
                            break;
                    }
                    e.Use();
                }
            }
            else
            {
                tileHighlight.vector2Value = new Vector2(-1,-1);
            }
        }

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }

    public override void OnInspectorGUI()
    {

        newSize = EditorGUILayout.Vector2Field("Size of Prefab", ((TileMapPrefabBuilder)target).tileMap.size);
        if (newSize != size)
            ((TileMapPrefabBuilder)target).tileMap = new TileMap((int)newSize.x, (int)newSize.y);
        size = newSize;

        tileSize.floatValue = EditorGUILayout.FloatField("Preview Tile Size", tileSize.floatValue);
        tileDefs.objectReferenceValue = (TileTextureDefs)EditorGUILayout.ObjectField("Texture Defs", tileDefs.objectReferenceValue, typeof(TileTextureDefs), true);

        if (GUILayout.Button("Build/Reset Tilemap"))
            GenerateTileMap();

        EditorGUILayout.Separator();

        if (!preview.boolValue)
        {
            GUI.color = Color.green;
            if (GUILayout.Button("Show Preview"))
            {

                //swap the value for preview
                preview.boolValue = !preview.boolValue;
                //find the tile graphics component
                tg.objectReferenceValue = ((TileMapPrefabBuilder)target).gameObject.GetComponent<TileGraphics>();
                //give tilegraphics the tile definitions
                ((TileGraphics)tg.objectReferenceValue).tileDefs = (TileTextureDefs)tileDefs.objectReferenceValue;
                //build the mesh
                ((TileGraphics)tg.objectReferenceValue).BuildSprite((((TileMapPrefabBuilder)target).tileMap), ((TileMapPrefabBuilder)target).tileSize);
                //enable the components for rendering
                ((TileMapPrefabBuilder)target).Enable();
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
                ((TileMapPrefabBuilder)target).Disable();
            }
            GUI.color = Color.white;
            if (GUILayout.Button("Update Preview"))
            {
                ((TileGraphics)tg.objectReferenceValue).BuildSprite((((TileMapPrefabBuilder)target).tileMap), ((TileMapPrefabBuilder)target).tileSize);
            }
            GUILayout.EndHorizontal();
        }

        if (!painting.boolValue)
        {
            if (GUILayout.Button("Start Painting"))
            {
                //swap the value for painting
                painting.boolValue = !painting.boolValue;
            }
        }
        else
        {
            GUI.color = Color.cyan;
            if (GUILayout.Button("Stop Painting"))
            {
                //swap the value for painting
                painting.boolValue = !painting.boolValue;
            }
            GUI.color = Color.white;
        }

        tilePaint.enumValueIndex = (int)(Tile.TYPE)EditorGUILayout.EnumPopup((Tile.TYPE)tilePaint.enumValueIndex);

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }

    void GenerateTileMap()
    {
        // temporary random tiles algorithm
        ((TileMapPrefabBuilder)target).tileMap = new TileMap(((TileMapPrefabBuilder)target).tileMap.sizeX, ((TileMapPrefabBuilder)target).tileMap.sizeY);
        for (int y = 0; y < ((TileMapPrefabBuilder)target).tileMap.sizeY; y++)
        {
            for (int x = 0; x < ((TileMapPrefabBuilder)target).tileMap.sizeX; x++)
            {
                ((TileMapPrefabBuilder)target).tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.none));
            }
        }
    }
}
