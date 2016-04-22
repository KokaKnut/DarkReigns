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
    public TileMapPrefabDefs prefabs;

    private TileMap tileMap;

	void Awake () {
        BuildTileMap();
	}
    
    public void BuildTileMap()
    {
        tileMap = GetComponent<TileMap>();
        tileMap.NewTileMap(sizeX, sizeY);

        // temporary random tiles algorithm
        GenerateWorld();
        
        // Now build the mesh with the tile map we made
        gameObject.GetComponent<TileGraphics>().BuildMesh(tileMap, tileSize);

        // Now build the polycollider with the tile data
        if (collison)
            gameObject.GetComponent<TileCollision>().BuildCollider(tileMap, tileSize);
        else
            gameObject.GetComponent<TileCollision>().RemoveCollision();
    }

    public void GenerateWorld()
    {
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.air));
            }
        }

        foreach(TileMapPrefabDef prefab in prefabs.prefabTypes)
        {
            if(Random.value < prefab.rarity)
            {

            }
        }

        if (border)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (y == 0 || y == sizeY - 1 || x == 0 || x == sizeX - 1)
                        tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.ground));
                }
            }
        }
    }

    [ContextMenu("Random Tilemap")]
    public void BuildSomeShit()
    {
        tileMap = GetComponent<TileMap>();

        // temporary random tiles algorithm
        int rand = 0;
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


        if (border)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (y == 0 || y == sizeY - 1 || x == 0 || x == sizeX - 1)
                        tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.ground));
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

    public Vector3 SpawnPos()
    {
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                if (tileMap.GetTile(x,y).type == Tile.TYPE.spawn)
                    return new Vector3(x * tileSize + gameObject.transform.position.x, y * tileSize + gameObject.transform.position.y, gameObject.transform.position.z);
            }
        }
        return new Vector3(100,100,0);
    }

    public Vector3 GoalPos()
    {
        return new Vector3();
    }
}