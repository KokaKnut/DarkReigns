using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(TileMap))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(TileGraphics))]
public class TileMapPrefabBuilder : MonoBehaviour
{
    // for removing this object
    public bool killme = false;

    public TileMap tileMap = null;
    public bool preview = false;
    public TileGraphics tg;
    public float tileSize = 0f;
    public TileTextureDefs tileDefs = null;

    void Awake()
    {
        Disable();
    }

    [ContextMenu("Hide Contents")]
    public void Disable()
    {
        gameObject.GetComponent<TileGraphics>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        enabled = false;
    }

    [ContextMenu("Show Contents")]
    public void Enable()
    {
        gameObject.GetComponent<TileGraphics>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        enabled = true;
    }

    public void Destroy()
    {
        killme = true;
    }
}
