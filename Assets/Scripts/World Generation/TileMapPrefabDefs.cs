using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileMapPrefabDef
{
    TileMap tileMap;
    bool unique;
    float rarity;
}

public class TileMapPrefabDefs : MonoBehaviour {

    public TileMapPrefabDef[] prefabTypes;
}
