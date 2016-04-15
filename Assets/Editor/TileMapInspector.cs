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
            GUI.color = Color.red;
            if (GUILayout.Button("Make Data Structure"))
            {
                ((TileMap)target).prefab = false;
                ((TileMap)target).gameObject.GetComponent<TileMapPrefabBuilder>().Disable();
            }
        }
        else
        {
            GUI.color = Color.green;
            if (GUILayout.Button("Make Prefab"))
            {
                ((TileMap)target).prefab = true;
                if (((TileMap)target).gameObject.GetComponent<TileMapPrefabBuilder>() == null)
                    ((TileMap)target).gameObject.AddComponent<TileMapPrefabBuilder>();
            }
        }
    }
}
