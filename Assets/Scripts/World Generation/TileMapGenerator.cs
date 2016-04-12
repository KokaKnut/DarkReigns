using UnityEngine;

[RequireComponent(typeof(TileMap))]
[RequireComponent(typeof(TileGraphics))]
[RequireComponent(typeof(TileCollision))]
public class TileMapGenerator : MonoBehaviour {

    [Header("Size of Tile Map")]
    public int sizeX;
    public int sizeY;
    public float tileSize = 1f;

    [Header("Generation vars")]
    public float temp = 1f;
    public bool border = true;
    public bool collison = true;

    [Header("Pregenerated Content")]
    public TileMap[] prefabs;

    private TileMap tileMap;

	void Awake () {
        BuildTileMap();
	}
    
    public void BuildTileMap()
    {
        tileMap = GetComponent<TileMap>();
        tileMap.NewTileMap(sizeX, sizeY);

        // temporary random tiles algorithm
        BuildSomeShit();
        
        // Now build the mesh with the tile map we made
        gameObject.GetComponent<TileGraphics>().BuildMesh(tileMap, tileSize);

        // Now build the polycollider with the tile data
        if (collison)
            gameObject.GetComponent<TileCollision>().BuildCollider(tileMap, tileSize);
        else
            gameObject.GetComponent<TileCollision>().RemoveCollision();
    }

    [ContextMenu("Rebuild Tilemap")]
    public void BuildSomeShit()
    {
        tileMap = GetComponent<TileMap>();

        int rand = 0;
        // temporary random tiles algorithm
        if (border)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (y == 0 || y == sizeY - 1 || x == 0 || x == sizeX - 1)
                        tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.ground));
                    else
                        tileMap.SetTile(x, y, new Tile(x, y, (Tile.TYPE)(int)UnityEngine.Random.Range(temp, 2f)));
                }
            }
        }
        else
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    rand = (int)UnityEngine.Random.Range(temp, 2f);
                    if (rand == 0)
                        tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.ground));
                    else
                        tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.air));
                }
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