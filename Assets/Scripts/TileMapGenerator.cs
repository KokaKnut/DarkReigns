using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileGraphics))]
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
                tileMap.SetTile(x, y, new Tile(x, y, (Tile.TYPE)((int)Random.Range(0.0f, 1.1f))));
            }
        }

        gameObject.GetComponent<TileGraphics>().BuildMesh(tileMap);
    }
}