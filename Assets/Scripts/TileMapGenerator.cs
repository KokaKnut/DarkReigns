using UnityEngine;
using System.Collections;

public class TileMapGenerator : MonoBehaviour {
    
    public int sizeX;
    public int sizeY;

    TileMap tileMap;

	void Start () {
        tileMap = new TileMap(sizeX, sizeY);
        BuildTileMap();
	}
	
	void BuildTileMap()
    {

    }
}