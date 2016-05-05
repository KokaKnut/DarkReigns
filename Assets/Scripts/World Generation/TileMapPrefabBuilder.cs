using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(TileMap))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(TileGraphics))]
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
        Disable();
    }

    [ContextMenu("Hide Contents")]
    public void Disable()
    {
        gameObject.GetComponent<TileGraphics>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        enabled = false;
    }

    [ContextMenu("Show Contents")]
    public void Enable()
    {
        gameObject.GetComponent<TileGraphics>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        enabled = true;
    }

    public void Destroy()
    {
        killme = true;
    }
}
