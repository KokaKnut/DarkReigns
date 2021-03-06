﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public struct TileMapPrefabDef
{
    public TileMap tileMap;
    public bool unique;
    public float rarity;
    public Vector2 coords;
}

public class TileMapPrefabDefs : MonoBehaviour
{

    public TileMapPrefabDef[] prefabTypes;
}
