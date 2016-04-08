using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileGraphics))]
[RequireComponent(typeof(TileCollision))]
public class TileMapGenerator : MonoBehaviour {

    [Header("Size of Tile Map")]
    public int sizeX;
    public int sizeY;
    public float tileSize = 1f;
    public float temp = 1f;

    TileMap tileMap;

	void Awake () {
        //BuildTileMap();
	}

    [ContextMenu("Rebuild Tilemap")]
    public void BuildTileMap()
    {
        tileMap = new TileMap(sizeX, sizeY);

        // temporary random tiles algorithm
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                tileMap.SetTile(x, y, new Tile(x, y, (Tile.TYPE)((int)UnityEngine.Random.Range(0.0f, temp))));
            }
        }
        
        // Now build the mesh with the tile map we made
        gameObject.GetComponent<TileGraphics>().BuildMesh(tileMap, tileSize);

        // Now build the polycollider with the tile data
        //gameObject.GetComponent<TileCollision>().BuildCollider(tileMap, tileSize);
    }

    [ContextMenu("Try Collision")]
    public void TryCollision()
    {
        gameObject.GetComponent<TileCollision>().BuildCollider(tileMap, tileSize);
    }
}