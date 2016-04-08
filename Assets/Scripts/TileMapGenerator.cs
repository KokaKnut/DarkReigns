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

    public TileMap tileMap;

	void Awake () {
        BuildTileMap();
	}

    [ContextMenu("Rebuild Tilemap")]
    public void BuildTileMap()
    {
        tileMap = new TileMap(sizeX, sizeY);

        // temporary random tiles algorithm
        BuildSomeShit();
        
        // Now build the mesh with the tile map we made
        gameObject.GetComponent<TileGraphics>().BuildMesh(tileMap, tileSize);

        // Now build the polycollider with the tile data
        gameObject.GetComponent<TileCollision>().BuildCollider(tileMap, tileSize);
    }

    public void BuildSomeShit()
    {
        tileMap = new TileMap(sizeX, sizeY);

        // temporary random tiles algorithm
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                if(y == 0)
                    tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.ground));
                else
                    tileMap.SetTile(x, y, new Tile(x, y, (Tile.TYPE)( 2 - (int)UnityEngine.Random.Range(0.0f, temp + 1))));
            }
        }
    }

    [ContextMenu("Draw Textures")]
    public void DrawTextures()
    {
        gameObject.GetComponent<TileGraphics>().BuildMesh(tileMap, tileSize);
    }

    [ContextMenu("Draw Collision")]
    public void DrawCollision()
    {
        gameObject.GetComponent<TileCollision>().BuildCollider(tileMap, tileSize);
    }
}