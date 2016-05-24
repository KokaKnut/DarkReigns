using UnityEngine;
using System.Collections;
using System;

public class TileMapPrefabBuilder : MonoBehaviour
{

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
}
