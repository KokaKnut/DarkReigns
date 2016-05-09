using UnityEngine;
using System;
using System.Collections.Generic;

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
        System.Random random = new System.Random();

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

        List<TileMapPrefabDef> uniques = new List<TileMapPrefabDef>();
        foreach(TileMapPrefabDef prefab in prefabs.prefabTypes)
        {
            if (prefab.unique)
                uniques.Add(prefab);
        }

        List<TileMapPrefabDef> commons = new List<TileMapPrefabDef>();
        foreach (TileMapPrefabDef prefab in prefabs.prefabTypes)
        {
            if (!(prefab.unique))
                commons.Add(prefab);
        }

        List<TileMapPrefabDef> drawn = new List<TileMapPrefabDef>();

        foreach (TileMapPrefabDef prefab in uniques)
        {
            if (prefab.rarity >= random.NextDouble())
            {
                for (int y = 0; y < prefab.tileMap.sizeY; y++)
                {
                    for (int x = 0; x < prefab.tileMap.sizeX; x++)
                    {
                        tileMap.SetTile(x + (int)prefab.coords.x, y + (int)prefab.coords.y, new Tile(x + (int)prefab.coords.x, y + (int)prefab.coords.y, prefab.tileMap.GetTile(x, y).type));
                    }
                }

                drawn.Add(prefab);
            }
        }

        int panic = 0;
        bool done = false;
        while(!done)
        {
            //choose random prefab that is currently in the map
            TileMapPrefabDef startPrefab = drawn[(int)(random.NextDouble() * drawn.Count)];
            //choose random opening
            int index = (int)(random.NextDouble() * (startPrefab.tileMap.topSize + startPrefab.tileMap.botSize + startPrefab.tileMap.leftSize + startPrefab.tileMap.rightSize));
            string side = "top";
            if (index >= startPrefab.tileMap.topSize)
            {
                side = "bot";
                index -= startPrefab.tileMap.topSize;
                if (index >= startPrefab.tileMap.botSize)
                {
                    side = "left";
                    index -= startPrefab.tileMap.botSize;
                    if (index >= startPrefab.tileMap.leftSize)
                    {
                        side = "right";
                        index -= startPrefab.tileMap.leftSize;
                    }
                }
            }

            TileMapPrefabDef prefab;
            int x = 0, y = 0;
            int px = 0, py = 0;
            switch (side)
            {
                case "top":
                    x = (int)startPrefab.coords.x + startPrefab.tileMap.top[index];
                    y = (int)startPrefab.coords.y + startPrefab.tileMap.sizeY - 1;
                    prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    while (prefab.tileMap.botSize == 0 || prefab.rarity < random.NextDouble())
                        prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    px = prefab.tileMap.bot[(int)(random.NextDouble() * prefab.tileMap.botSize)];
                    if ((x - px) > 0 && (x + (prefab.tileMap.sizeX - px)) < (tileMap.sizeX - 1) && (y + prefab.tileMap.sizeY) < (tileMap.sizeY - 1))
                    {
                        bool free = true;
                        for (int i = 0; i < prefab.tileMap.sizeY; i++)
                        {
                            for (int j = 0; j < prefab.tileMap.sizeX; j++)
                            {
                                if (tileMap.GetTile(x - px + j, y + 1 + i) != null)
                                    free = false;
                            }
                        }
                        if (free)
                        {
                            for (int i = 0; i < prefab.tileMap.sizeY; i++)
                            {
                                for (int j = 0; j < prefab.tileMap.sizeX; j++)
                                {
                                    tileMap.SetTile(x - px + j, y + 1 + i, new Tile(x - px + j, y + 1 + i, prefab.tileMap.GetTile(j, i).type));
                                }
                            }
                            prefab.coords = new Vector2(x - px, y + 1);
                            drawn.Add(prefab);
                        }
                    }
                    break;
                case "bot":
                    x = (int)startPrefab.coords.x + startPrefab.tileMap.bot[index];
                    y = (int)startPrefab.coords.y;
                    prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    while (prefab.tileMap.topSize == 0 || prefab.rarity < random.NextDouble())
                        prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    px = prefab.tileMap.top[(int)(random.NextDouble() * prefab.tileMap.topSize)];
                    py = prefab.tileMap.sizeY;
                    if ((x - px) > 0 && (x + (prefab.tileMap.sizeX - px)) < (tileMap.sizeX - 1) && (y - py) > 0)
                    {
                        bool free = true;
                        for (int i = 0; i < prefab.tileMap.sizeY; i++)
                        {
                            for (int j = 0; j < prefab.tileMap.sizeX; j++)
                            {
                                if (tileMap.GetTile(x - px + j, y - py + i) != null)
                                    free = false;
                            }
                        }
                        if (free)
                        {
                            for (int i = 0; i < prefab.tileMap.sizeY; i++)
                            {
                                for (int j = 0; j < prefab.tileMap.sizeX; j++)
                                {
                                    tileMap.SetTile(x - px + j, y - py + i, new Tile(x - px + j, y - py + i, prefab.tileMap.GetTile(j, i).type));
                                }
                            }
                            prefab.coords = new Vector2(x - px, y - py);
                            drawn.Add(prefab);
                        }
                    }
                    break;
                case "left":
                    x = (int)startPrefab.coords.x;
                    y = (int)startPrefab.coords.y + startPrefab.tileMap.left[index];
                    prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    while (prefab.tileMap.rightSize == 0 || prefab.rarity < random.NextDouble())
                        prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    px = prefab.tileMap.sizeX;
                    py = prefab.tileMap.right[(int)(random.NextDouble() * prefab.tileMap.rightSize)];
                    if((y - py) > 0 && (y + (prefab.tileMap.sizeY - py)) < (tileMap.sizeY - 1) && (x - px) > 0)
                    {
                        bool free = true;
                        for (int i = 0; i < prefab.tileMap.sizeY; i++)
                        {
                            for (int j = 0; j < prefab.tileMap.sizeX; j++)
                            {
                                if (tileMap.GetTile(x - px + j, y - py + i) != null)
                                    free = false;
                            }
                        }
                        if (free)
                        {
                            for (int i = 0; i < prefab.tileMap.sizeY; i++)
                            {
                                for (int j = 0; j < prefab.tileMap.sizeX; j++)
                                {
                                    tileMap.SetTile(x - px + j, y - py + i, new Tile(x - px + j, y - py + i, prefab.tileMap.GetTile(j, i).type));
                                }
                            }
                            prefab.coords = new Vector2(x - px, y - py);
                            drawn.Add(prefab);
                        }
                    }
                    break;
                case "right":
                    x = (int)startPrefab.coords.x + startPrefab.tileMap.sizeX - 1;
                    y = (int)startPrefab.coords.y + startPrefab.tileMap.right[index];
                    prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    while (prefab.tileMap.leftSize == 0 || prefab.rarity < random.NextDouble())
                        prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    py = prefab.tileMap.left[(int)(random.NextDouble() * prefab.tileMap.leftSize)];
                    if ((y - py) > 0 && (y + (prefab.tileMap.sizeY - py)) < (tileMap.sizeY - 1) && (x + prefab.tileMap.sizeX) < (tileMap.sizeX - 1))
                    {
                        bool free = true;
                        for (int i = 0; i < prefab.tileMap.sizeY; i++)
                        {
                            for (int j = 0; j < prefab.tileMap.sizeX; j++)
                            {
                                if (tileMap.GetTile(x + 1 + j, y - py + i) != null)
                                    free = false;
                            }
                        }
                        if(free)
                        {
                            for (int i = 0; i < prefab.tileMap.sizeY; i++)
                            {
                                for (int j = 0; j < prefab.tileMap.sizeX; j++)
                                {
                                    tileMap.SetTile(x  + 1 + j, y - py + i, new Tile(x + 1 + j, y - py + i, prefab.tileMap.GetTile(j, i).type));
                                }
                            }
                            prefab.coords = new Vector2(x  + 1, y - py);
                            drawn.Add(prefab);
                        }
                    }
                    break;
            }
            
            if (panic++ >= 50000)
            {
                print("Prefab populating caused overflow panic!");
                done = true;
            }
        }

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                if (tileMap.GetTile(x, y) == null)
                    tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.air));
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
                    return new Vector3((x + .5f) * tileSize + gameObject.transform.position.x, (y + .5f) * tileSize + gameObject.transform.position.y, gameObject.transform.position.z);
            }
        }
        return new Vector3(100,100,0);
    }

    public Vector3 GoalPos()
    {
        return new Vector3();
    }
}