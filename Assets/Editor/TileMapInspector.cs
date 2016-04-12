using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileMap))]
public class TileMapInspector : Editor
{
    SerializedProperty prefab;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        prefab = serializedObject.FindProperty("prefab");
    }

    public override void OnInspectorGUI()
    {
        if (prefab.boolValue)
        {
            if (GUILayout.Button("Make Data Structure"))
            {
                ((TileMap)target).prefab = false;
                ((TileMap)target).gameObject.GetComponent<TileMapPrefabBuilder>().Disable();
            }
        }
        else
        {
            if (GUILayout.Button("Make Prefab"))
            {
                ((TileMap)target).prefab = true;
                ((TileMap)target).gameObject.AddComponent<TileMapPrefabBuilder>();
            }
        }
    }
}
