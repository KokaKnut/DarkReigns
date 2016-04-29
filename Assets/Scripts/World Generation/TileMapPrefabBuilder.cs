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
    public Material mat = null;
    public TileTextureDefs tileDefs = null;

    void Awake()
    {
        tg.enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.enabled = false;
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
