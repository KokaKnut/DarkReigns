using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileMapGenerator))]
public class TileMapGenoratorInspector : Editor {

	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Tilemap"))
        {
            ((TileMapGenerator)target).BuildTileMap();
        }
    }
}