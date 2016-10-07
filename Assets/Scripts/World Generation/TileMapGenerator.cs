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

        byte[,] solution = GetComponent<ImgScanner>().ScanImg(0);

        uniques = new List<TileMapPrefabDef>();
        foreach(TileMapPrefabDef prefab in prefabs.prefabTypes)
        {
            if (prefab.unique)
                uniques.Add(prefab);
        }
        
        commons = new List<TileMapPrefabDef>();

        WeightedShuffler<TileMapPrefabDef> shuffler = new WeightedShuffler<TileMapPrefabDef>(seed);
        if (!isSeed)
            shuffler = new WeightedShuffler<TileMapPrefabDef>();

        foreach (TileMapPrefabDef prefab in prefabs.prefabTypes)
        {
            if (!(prefab.unique))
            {
                commons.Add(prefab);
                shuffler.Add(prefab, prefab.rarity);
            }
        }

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

        while (drawn.Count > 0 && !done)
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

            int fO = 0;
            //run through list of opennings until linkages are filled
            while(firstPrefab.linkages < firstPrefab.linkageNumber && fO < opennings.Count && !done)
            {
                //call the shuffler for a new list
                List<TileMapPrefabDef> shuffledList = shuffler.GetShufledList();

                int p = 0;
                //run through list of prefabs
                while (p < shuffledList.Count && !done)
                {
                    //make list of prefab opennings on prefab
                    TileMapPrefabDef prospectivePrefab = shuffledList[p];

                    //make list of opennings
                    List<int[]> prospectiveOpennings = new List<int[]>();
                    foreach (int openning in prospectivePrefab.tileMapPrefab.tileMap.top)
                    {
                        prospectiveOpennings.Add(new int[] { 0, openning });
                    }
                    foreach (int openning in prospectivePrefab.tileMapPrefab.tileMap.bot)
                    {
                        prospectiveOpennings.Add(new int[] { 1, openning });
                    }
                    foreach (int openning in prospectivePrefab.tileMapPrefab.tileMap.left)
                    {
                        prospectiveOpennings.Add(new int[] { 2, openning });
                    }
                    foreach (int openning in prospectivePrefab.tileMapPrefab.tileMap.right)
                    {
                        prospectiveOpennings.Add(new int[] { 3, openning });
                    }

                    //shuffle list of opennings
                    for (int i = prospectiveOpennings.Count - 1, j; i >= 1; i--)
                    {
                        j = random.Next(i);
                        int[] tempint = prospectiveOpennings[j];
                        prospectiveOpennings[j] = prospectiveOpennings[i];
                        prospectiveOpennings[i] = tempint;
                    }

                    int x = 0, y = 0;
                    int px = 0, py = 0;
                    int pO = 0;
                    //run through list of opennings on prefab
                    while (pO < prospectiveOpennings.Count && !done)
                    {
                        //place prefab if it fits, and if it does, exit loop and exit the next one too (this leaves us continueing the list of opennings)
                        switch (prospectiveOpennings[pO][0])
                        {
                            case 0: //top
                                x = (int)firstPrefab.coords.x + opennings[fO][1];
                                y = (int)firstPrefab.coords.y + firstPrefab.tileMapPrefab.tileMap.sizeY - 1;
                                px = prospectiveOpennings[pO][1];
                                if ((x - px) > 0 && (x + (prospectivePrefab.tileMapPrefab.tileMap.sizeX - px)) < (tileMap.sizeX - 1) && (y + prospectivePrefab.tileMapPrefab.tileMap.sizeY) < (tileMap.sizeY - 1))
                                {
                                    bool free = true;
                                    for (int i = 0; free && i < prospectivePrefab.tileMapPrefab.tileMap.sizeY; i++)
                                    {
                                        for (int j = 0; free && j < prospectivePrefab.tileMapPrefab.tileMap.sizeX; j++)
                                        {
                                            if (tileMap.GetTile(x - px + j, y + 1 + i) != null)
                                                free = false;
                                        }
                                    }
                                    if (free)
                                    {
                                        for (int i = 0; i < prospectivePrefab.tileMapPrefab.tileMap.sizeY; i++)
                                        {
                                            for (int j = 0; j < prospectivePrefab.tileMapPrefab.tileMap.sizeX; j++)
                                            {
                                                tileMap.SetTile(x - px + j, y + 1 + i, new Tile(x - px + j, y + 1 + i, prospectivePrefab.tileMapPrefab.tileMap.GetTile(j, i).type));
                                            }
                                        }
                                        prospectivePrefab.coords = new Vector2(x - px, y + 1);
                                        firstPrefab.linkages++; // TODO: make this recalculate, not just increment
                                        drawn.Remove(firstPrefab);
                                        drawn.Insert(0, firstPrefab);
                                        prospectivePrefab.linkages++;
                                        drawn.Add(prospectivePrefab);
                                    }
                                }
                                break;
                            case 1: //bot
                                x = (int)firstPrefab.coords.x + opennings[fO][1];
                                y = (int)firstPrefab.coords.y;
                                px = prospectiveOpennings[pO][1];
                                py = prospectivePrefab.tileMapPrefab.tileMap.sizeY;
                                if ((x - px) > 0 && (x + (prospectivePrefab.tileMapPrefab.tileMap.sizeX - px)) < (tileMap.sizeX - 1) && (y - py) > 0)
                                {
                                    bool free = true;
                                    for (int i = 0; free && i < prospectivePrefab.tileMapPrefab.tileMap.sizeY; i++)
                                    {
                                        for (int j = 0; free && j < prospectivePrefab.tileMapPrefab.tileMap.sizeX; j++)
                                        {
                                            if (tileMap.GetTile(x - px + j, y - py + i) != null)
                                                free = false;
                                        }
                                    }
                                    if (free)
                                    {
                                        for (int i = 0; i < prospectivePrefab.tileMapPrefab.tileMap.sizeY; i++)
                                        {
                                            for (int j = 0; j < prospectivePrefab.tileMapPrefab.tileMap.sizeX; j++)
                                            {
                                                tileMap.SetTile(x - px + j, y - py + i, new Tile(x - px + j, y - py + i, prospectivePrefab.tileMapPrefab.tileMap.GetTile(j, i).type));
                                            }
                                        }
                                        prospectivePrefab.coords = new Vector2(x - px, y - py);
                                        firstPrefab.linkages++; // TODO: make this recalculate, not just increment
                                        drawn.RemoveAt(0);
                                        drawn.Insert(0, firstPrefab);
                                        prospectivePrefab.linkages++;
                                        drawn.Add(prospectivePrefab);
                                    }
                                }
                                break;
                            case 2: //left
                                x = (int)firstPrefab.coords.x;
                                y = (int)firstPrefab.coords.y + opennings[fO][1];
                                px = prospectivePrefab.tileMapPrefab.tileMap.sizeX;
                                py = prospectiveOpennings[pO][1];
                                if ((y - py) > 0 && (y + (prospectivePrefab.tileMapPrefab.tileMap.sizeY - py)) < (tileMap.sizeY - 1) && (x - px) > 0)
                                {
                                    bool free = true;
                                    for (int i = 0; free && i < prospectivePrefab.tileMapPrefab.tileMap.sizeY; i++)
                                    {
                                        for (int j = 0; free && j < prospectivePrefab.tileMapPrefab.tileMap.sizeX; j++)
                                        {
                                            if (tileMap.GetTile(x - px + j, y - py + i) != null)
                                                free = false;
                                        }
                                    }
                                    if (free)
                                    {
                                        for (int i = 0; i < prospectivePrefab.tileMapPrefab.tileMap.sizeY; i++)
                                        {
                                            for (int j = 0; j < prospectivePrefab.tileMapPrefab.tileMap.sizeX; j++)
                                            {
                                                tileMap.SetTile(x - px + j, y - py + i, new Tile(x - px + j, y - py + i, prospectivePrefab.tileMapPrefab.tileMap.GetTile(j, i).type));
                                            }
                                        }
                                        prospectivePrefab.coords = new Vector2(x - px, y - py);
                                        firstPrefab.linkages++; // TODO: make this recalculate, not just increment
                                        drawn.RemoveAt(0);
                                        drawn.Insert(0, firstPrefab);
                                        prospectivePrefab.linkages++;
                                        drawn.Add(prospectivePrefab);
                                    }
                                }
                                break;
                            case 3: //right
                                x = (int)firstPrefab.coords.x + firstPrefab.tileMapPrefab.tileMap.sizeX - 1;
                                y = (int)firstPrefab.coords.y + opennings[fO][1];
                                py = prospectiveOpennings[pO][1];
                                if ((y - py) > 0 && (y + (prospectivePrefab.tileMapPrefab.tileMap.sizeY - py)) < (tileMap.sizeY - 1) && (x + prospectivePrefab.tileMapPrefab.tileMap.sizeX) < (tileMap.sizeX - 1))
                                {
                                    bool free = true;
                                    for (int i = 0; free && i < prospectivePrefab.tileMapPrefab.tileMap.sizeY; i++)
                                    {
                                        for (int j = 0; free && j < prospectivePrefab.tileMapPrefab.tileMap.sizeX; j++)
                                        {
                                            if (tileMap.GetTile(x + 1 + j, y - py + i) != null)
                                                free = false;
                                        }
                                    }
                                    if (free)
                                    {
                                        for (int i = 0; i < prospectivePrefab.tileMapPrefab.tileMap.sizeY; i++)
                                        {
                                            for (int j = 0; j < prospectivePrefab.tileMapPrefab.tileMap.sizeX; j++)
                                            {
                                                tileMap.SetTile(x + 1 + j, y - py + i, new Tile(x + 1 + j, y - py + i, prospectivePrefab.tileMapPrefab.tileMap.GetTile(j, i).type));
                                            }
                                        }
                                        prospectivePrefab.coords = new Vector2(x + 1, y - py);
                                        firstPrefab.linkages++; // TODO: make this recalculate, not just increment
                                        drawn.RemoveAt(0);
                                        drawn.Insert(0, firstPrefab);
                                        prospectivePrefab.linkages++;
                                        drawn.Add(prospectivePrefab);
                                    }
                                }
                                break;
                        }

                        pO++;

                        if (panic++ >= 50000)
                        {
                            print("Prefab checking list caused overflow panic!");
                            done = true;
                        }
                    }

                    p++;

                    if (panic++ >= 50000)
                    {
                        print("Shuffled list caused overflow panic!");
                        done = true;
                    }
                }

                fO++;

                if (panic++ >= 50000)
                {
                    print("Linkage filling caused overflow panic!");
                    done = true;
                }
            }

            //remove first item from list "drawn" and add it to "drawnFully"
            drawnFully.Add(drawn[0]);
            drawn.RemoveAt(0);

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
                    tileMap.SetTile(x, y, new Tile(x, y, Tile.TYPE.ground));
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