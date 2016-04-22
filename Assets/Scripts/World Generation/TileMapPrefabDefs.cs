using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileMapPrefabDef
{
    public TileMap tileMap;
    public bool unique;
    public float rarity;
}

public class TileMapPrefabDefs : MonoBehaviour
{

    public TileMapPrefabDef[] prefabTypes;
}
