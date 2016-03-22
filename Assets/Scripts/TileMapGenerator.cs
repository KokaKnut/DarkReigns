using UnityEngine;
using System.Collections;

public class TileMapGenerator : MonoBehaviour {

    [Header("Size of Tile Map")]
    public int sizeX;
    public int sizeY;

    TileMap tileMap;

	void Awake () {
        BuildTileMap();
	}

    [ContextMenu("Rebuild Tilemap")]
    void BuildTileMap()
    {
        tileMap = new TileMap(sizeX, sizeY);
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                tileMap.SetTile(x,y, new Tile((Tile.TYPE)Random.Range(0, 2)));
            }
        }

        GetComponent<TileGraphics>().BuildMesh(tileMap);
    }
}