using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(TileGraphics))]
[RequireComponent(typeof(TileCollision))]
public class TileMapGenerator : MonoBehaviour {

    [Header("Size of Tile Map")]
    public int sizeX;
    public int sizeY;
    public float tileSize = 1f;

    [Header("Generation vars")]
    public bool isSeed = false;
    public int seed = 0;
    public float temp = 1f;
    public bool border = true;
    public bool collison = true;

    [Header("Pregenerated Content")]
    public TileMapPrefabDefs prefabs;

    private TileMap tileMap;
    private List<TileMapPrefabDef> commons;
    private List<TileMapPrefabDef> uniques;

    void Awake () {
        BuildTileMap();
	}

    public void BuildTileMap()
    {
        tileMap = new TileMap(sizeX, sizeY);

        // temporary random tiles algorithm
        while(!GenerateWorld())
            tileMap = new TileMap(sizeX, sizeY);
        
        // Now build the mesh with the tile map we made
        gameObject.GetComponent<TileGraphics>().BuildMesh(tileMap, tileSize);

        // Now build the polycollider with the tile data
        if (collison)
        {
            gameObject.GetComponent<TileCollision>().BuildColliderFast(tileMap, tileSize);
            gameObject.GetComponent<TileCollision>().BuildRopeColliders(tileMap, tileSize);
        }
        else
            gameObject.GetComponent<TileCollision>().RemoveCollision();
    }

    public bool GenerateWorld()
    {
        System.Random random = new System.Random(seed);
        if (!isSeed)
            random = new System.Random();

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

        uniques = new List<TileMapPrefabDef>();
        foreach(TileMapPrefabDef prefab in prefabs.prefabTypes)
        {
            if (prefab.unique)
                uniques.Add(prefab);
        }
        
        commons = new List<TileMapPrefabDef>();
        WeightedShuffler<TileMapPrefabDef> shuffler = new WeightedShuffler<TileMapPrefabDef>();
        foreach (TileMapPrefabDef prefab in prefabs.prefabTypes)
        {
            if (!(prefab.unique))
            {
                commons.Add(prefab);
            }
        }

        int[,] solution = GenerateSolution();

        List<TileMapPrefabDef> drawn = new List<TileMapPrefabDef>();
        List<TileMapPrefabDef> drawnFully = new List<TileMapPrefabDef>();

        foreach (TileMapPrefabDef prefab in uniques)
        {
            if (prefab.rarity >= random.NextDouble())
            {
                for (int y = 0; y < prefab.tileMapPrefab.tileMap.sizeY; y++)
                {
                    for (int x = 0; x < prefab.tileMapPrefab.tileMap.sizeX; x++)
                    {
                        tileMap.SetTile(x + (int)prefab.coords.x, y + (int)prefab.coords.y, new Tile(x + (int)prefab.coords.x, y + (int)prefab.coords.y, prefab.tileMapPrefab.tileMap.GetTile(x, y).type));
                    }
                }

                drawn.Add(prefab);
            }
        }

        int panic = 0;
        bool done = false;

        if (drawn.Count == 0 || commons.Count == 0)
            done = true;

        while (!done)
        {
            //choose first item on the list "drawn"
            TileMapPrefabDef firstPrefab = drawn[0];

            //make list of opennings
            List<int[]> opennings = new List<int[]>();
            foreach (int openning in firstPrefab.tileMapPrefab.tileMap.top)
            {
                opennings.Add(new int[] { 0, openning });
            }
            foreach (int openning in firstPrefab.tileMapPrefab.tileMap.bot)
            {
                opennings.Add(new int[] { 1, openning });
            }
            foreach (int openning in firstPrefab.tileMapPrefab.tileMap.left)
            {
                opennings.Add(new int[] { 2, openning });
            }
            foreach (int openning in firstPrefab.tileMapPrefab.tileMap.right)
            {
                opennings.Add(new int[] { 3, openning });
            }

            //shuffle list of opennings
            for (int i = opennings.Count - 1, j; i >= 1; i--)
            {
                j = random.Next(i);
                int[] tempint = opennings[j];
                opennings[j] = opennings[i];
                opennings[i] = tempint;
            }

            //run through list of opennings until linkages are filled
            while(firstPrefab.linkages < firstPrefab.linkageNumber)
            {
                //call the shuffler for a new list
                

                //run through list of prefabs

                    //make list of prefab opennings on prefab

                    //run through list of opennings on prefab

                        //place prefab if it fits, and if it does, exit loop and exi the next one too (this leaves us continueing the list of opennings)
            }

            //remove first item from list "drawn" and add it to "drawnFully"

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

        return true;
    }

    private int[,] GenerateSolution()
    {
        int[,] solution = new int[sizeX, sizeY];

        

        return solution;
    }

    public bool GenerateWorldOld()
    {
        System.Random random = new System.Random(seed);
        if (!isSeed)
            random = new System.Random();

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

        uniques = new List<TileMapPrefabDef>();
        foreach (TileMapPrefabDef prefab in prefabs.prefabTypes)
        {
            if (prefab.unique)
                uniques.Add(prefab);
        }

        commons = new List<TileMapPrefabDef>();
        foreach (TileMapPrefabDef prefab in prefabs.prefabTypes)
        {
            if (!(prefab.unique))
                commons.Add(prefab);
        }

        int[,] solution = GenerateSolution();

        List<TileMapPrefabDef> drawn = new List<TileMapPrefabDef>();
        List<TileMapPrefabDef> drawnFully = new List<TileMapPrefabDef>();

        foreach (TileMapPrefabDef prefab in uniques)
        {
            if (prefab.rarity >= random.NextDouble())
            {
                for (int y = 0; y < prefab.tileMapPrefab.tileMap.sizeY; y++)
                {
                    for (int x = 0; x < prefab.tileMapPrefab.tileMap.sizeX; x++)
                    {
                        tileMap.SetTile(x + (int)prefab.coords.x, y + (int)prefab.coords.y, new Tile(x + (int)prefab.coords.x, y + (int)prefab.coords.y, prefab.tileMapPrefab.tileMap.GetTile(x, y).type));
                    }
                }

                drawn.Add(prefab);
            }
        }

        int panic = 0;
        bool done = false;

        if (drawn.Count == 0 || commons.Count == 0)
            done = true;

        while (!done)
        {
            //choose random prefab that is currently in the map
            int drawnIndex = (int)(random.NextDouble() * drawn.Count);
            TileMapPrefabDef startPrefab = drawn[drawnIndex];
            while (startPrefab.linkages >= startPrefab.linkageNumber)
            {
                drawn.RemoveAt(drawnIndex);
                drawnFully.Add(startPrefab);

                drawnIndex = (int)(random.NextDouble() * drawn.Count);
                startPrefab = drawn[drawnIndex];
            }
            //choose random opening
            int index = (int)(random.NextDouble() * (startPrefab.tileMapPrefab.tileMap.topSize + startPrefab.tileMapPrefab.tileMap.botSize + startPrefab.tileMapPrefab.tileMap.leftSize + startPrefab.tileMapPrefab.tileMap.rightSize));
            string side = "top";
            if (index >= startPrefab.tileMapPrefab.tileMap.topSize)
            {
                side = "bot";
                index -= startPrefab.tileMapPrefab.tileMap.topSize;
                if (index >= startPrefab.tileMapPrefab.tileMap.botSize)
                {
                    side = "left";
                    index -= startPrefab.tileMapPrefab.tileMap.botSize;
                    if (index >= startPrefab.tileMapPrefab.tileMap.leftSize)
                    {
                        side = "right";
                        index -= startPrefab.tileMapPrefab.tileMap.leftSize;
                    }
                }
            }

            TileMapPrefabDef prefab;
            int x = 0, y = 0;
            int px = 0, py = 0;
            switch (side)
            {
                case "top":
                    x = (int)startPrefab.coords.x + startPrefab.tileMapPrefab.tileMap.top[index];
                    y = (int)startPrefab.coords.y + startPrefab.tileMapPrefab.tileMap.sizeY - 1;
                    prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    while (prefab.tileMapPrefab.tileMap.botSize == 0 || prefab.rarity < random.NextDouble())
                        prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    px = prefab.tileMapPrefab.tileMap.bot[(int)(random.NextDouble() * prefab.tileMapPrefab.tileMap.botSize)];
                    if ((x - px) > 0 && (x + (prefab.tileMapPrefab.tileMap.sizeX - px)) < (tileMap.sizeX - 1) && (y + prefab.tileMapPrefab.tileMap.sizeY) < (tileMap.sizeY - 1))
                    {
                        bool free = true;
                        for (int i = 0; i < prefab.tileMapPrefab.tileMap.sizeY; i++)
                        {
                            for (int j = 0; j < prefab.tileMapPrefab.tileMap.sizeX; j++)
                            {
                                if (tileMap.GetTile(x - px + j, y + 1 + i) != null)
                                    free = false;
                            }
                        }
                        if (free)
                        {
                            for (int i = 0; i < prefab.tileMapPrefab.tileMap.sizeY; i++)
                            {
                                for (int j = 0; j < prefab.tileMapPrefab.tileMap.sizeX; j++)
                                {
                                    tileMap.SetTile(x - px + j, y + 1 + i, new Tile(x - px + j, y + 1 + i, prefab.tileMapPrefab.tileMap.GetTile(j, i).type));
                                }
                            }
                            prefab.coords = new Vector2(x - px, y + 1);
                            startPrefab.linkages++;
                            drawn.RemoveAt(drawnIndex);
                            drawn.Add(startPrefab);
                            prefab.linkages++;
                            drawn.Add(prefab);
                        }
                    }
                    break;
                case "bot":
                    x = (int)startPrefab.coords.x + startPrefab.tileMapPrefab.tileMap.bot[index];
                    y = (int)startPrefab.coords.y;
                    prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    while (prefab.tileMapPrefab.tileMap.topSize == 0 || prefab.rarity < random.NextDouble())
                        prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    px = prefab.tileMapPrefab.tileMap.top[(int)(random.NextDouble() * prefab.tileMapPrefab.tileMap.topSize)];
                    py = prefab.tileMapPrefab.tileMap.sizeY;
                    if ((x - px) > 0 && (x + (prefab.tileMapPrefab.tileMap.sizeX - px)) < (tileMap.sizeX - 1) && (y - py) > 0)
                    {
                        bool free = true;
                        for (int i = 0; i < prefab.tileMapPrefab.tileMap.sizeY; i++)
                        {
                            for (int j = 0; j < prefab.tileMapPrefab.tileMap.sizeX; j++)
                            {
                                if (tileMap.GetTile(x - px + j, y - py + i) != null)
                                    free = false;
                            }
                        }
                        if (free)
                        {
                            for (int i = 0; i < prefab.tileMapPrefab.tileMap.sizeY; i++)
                            {
                                for (int j = 0; j < prefab.tileMapPrefab.tileMap.sizeX; j++)
                                {
                                    tileMap.SetTile(x - px + j, y - py + i, new Tile(x - px + j, y - py + i, prefab.tileMapPrefab.tileMap.GetTile(j, i).type));
                                }
                            }
                            prefab.coords = new Vector2(x - px, y - py);
                            startPrefab.linkages++;
                            drawn.RemoveAt(drawnIndex);
                            drawn.Add(startPrefab);
                            prefab.linkages++;
                            drawn.Add(prefab);
                        }
                    }
                    break;
                case "left":
                    x = (int)startPrefab.coords.x;
                    y = (int)startPrefab.coords.y + startPrefab.tileMapPrefab.tileMap.left[index];
                    prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    while (prefab.tileMapPrefab.tileMap.rightSize == 0 || prefab.rarity < random.NextDouble())
                        prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    px = prefab.tileMapPrefab.tileMap.sizeX;
                    py = prefab.tileMapPrefab.tileMap.right[(int)(random.NextDouble() * prefab.tileMapPrefab.tileMap.rightSize)];
                    if ((y - py) > 0 && (y + (prefab.tileMapPrefab.tileMap.sizeY - py)) < (tileMap.sizeY - 1) && (x - px) > 0)
                    {
                        bool free = true;
                        for (int i = 0; i < prefab.tileMapPrefab.tileMap.sizeY; i++)
                        {
                            for (int j = 0; j < prefab.tileMapPrefab.tileMap.sizeX; j++)
                            {
                                if (tileMap.GetTile(x - px + j, y - py + i) != null)
                                    free = false;
                            }
                        }
                        if (free)
                        {
                            for (int i = 0; i < prefab.tileMapPrefab.tileMap.sizeY; i++)
                            {
                                for (int j = 0; j < prefab.tileMapPrefab.tileMap.sizeX; j++)
                                {
                                    tileMap.SetTile(x - px + j, y - py + i, new Tile(x - px + j, y - py + i, prefab.tileMapPrefab.tileMap.GetTile(j, i).type));
                                }
                            }
                            prefab.coords = new Vector2(x - px, y - py);
                            startPrefab.linkages++;
                            drawn.RemoveAt(drawnIndex);
                            drawn.Add(startPrefab);
                            prefab.linkages++;
                            drawn.Add(prefab);
                        }
                    }
                    break;
                case "right":
                    x = (int)startPrefab.coords.x + startPrefab.tileMapPrefab.tileMap.sizeX - 1;
                    y = (int)startPrefab.coords.y + startPrefab.tileMapPrefab.tileMap.right[index];
                    prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    while (prefab.tileMapPrefab.tileMap.leftSize == 0 || prefab.rarity < random.NextDouble())
                        prefab = commons[(int)(random.NextDouble() * commons.Count)];
                    py = prefab.tileMapPrefab.tileMap.left[(int)(random.NextDouble() * prefab.tileMapPrefab.tileMap.leftSize)];
                    if ((y - py) > 0 && (y + (prefab.tileMapPrefab.tileMap.sizeY - py)) < (tileMap.sizeY - 1) && (x + prefab.tileMapPrefab.tileMap.sizeX) < (tileMap.sizeX - 1))
                    {
                        bool free = true;
                        for (int i = 0; i < prefab.tileMapPrefab.tileMap.sizeY; i++)
                        {
                            for (int j = 0; j < prefab.tileMapPrefab.tileMap.sizeX; j++)
                            {
                                if (tileMap.GetTile(x + 1 + j, y - py + i) != null)
                                    free = false;
                            }
                        }
                        if (free)
                        {
                            for (int i = 0; i < prefab.tileMapPrefab.tileMap.sizeY; i++)
                            {
                                for (int j = 0; j < prefab.tileMapPrefab.tileMap.sizeX; j++)
                                {
                                    tileMap.SetTile(x + 1 + j, y - py + i, new Tile(x + 1 + j, y - py + i, prefab.tileMapPrefab.tileMap.GetTile(j, i).type));
                                }
                            }
                            prefab.coords = new Vector2(x + 1, y - py);
                            startPrefab.linkages++;
                            drawn.RemoveAt(drawnIndex);
                            drawn.Add(startPrefab);
                            prefab.linkages++;
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

        return true;
    }

    [ContextMenu("Random Tilemap")]
    public void BuildSomeShit()
    {
        tileMap = new TileMap(sizeX, sizeY);

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
        gameObject.GetComponent<TileCollision>().BuildColliderFast(tileMap, tileSize);
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