using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(TileMap))]
public class TileMapPrefabBuilder : MonoBehaviour
{
    // for removing this object
    public bool killme = false;

    public TileMap tileMap = null;
    public bool preview = false;
    public TileGraphics tg;
    public float tileSize = 0f;
    public int textureRes = 0;
    public Texture2D tileTexture = null;
    public Material mat = null;
    public TileTextureDefs tileDefs = null;

    void Awake()
    {
        Disable();
    }

    public Vector2 size
    {
        get
        {
            return new Vector2(tileMap.sizeX, tileMap.sizeY);
        }
        set
        {
            tileMap.sizeX = (int)value.x;
            tileMap.sizeY = (int)value.y;
        }
    }

    // for removing this object
    public void Disable()
    {
        DestroyImmediate(tg);
        DestroyImmediate(gameObject.GetComponent<MeshRenderer>());
        DestroyImmediate(gameObject.GetComponent<MeshFilter>());
        killme = true;
    }
}
