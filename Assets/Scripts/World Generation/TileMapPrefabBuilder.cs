using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(TileMap))]
public class TileMapPrefabBuilder : MonoBehaviour {

	public TileMap tileMap = null;
    public bool preview = false;
    public TileGraphics tg;
    public float tileSize = 0f;
    public int textureRes = 0;
    public Texture2D tileTexture = null;
    public Material mat = null;

    public Vector2 size
    {
        get
        {
            return new Vector2(gameObject.GetComponent<TileMap>().sizeX, gameObject.GetComponent<TileMap>().sizeY);
        }
        set
        {
            gameObject.GetComponent<TileMap>().sizeX = (int)value.x;
            gameObject.GetComponent<TileMap>().sizeY = (int)value.y;
        }
    }

    public void DestroyImmediate()
    {
        //DestroyImmediate()
        throw new NotImplementedException();
    }
}
