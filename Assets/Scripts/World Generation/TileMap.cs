using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class TileMap {

    [SerializeField]
    private Tile[] tiles;

    [SerializeField]
    private int _sizeX;
    public int sizeX
    {
        get
        {
            return _sizeX;
        }
    }

    [SerializeField]
    private int _sizeY;
    public int sizeY
    {
        get
        {
            return _sizeY;
        }
    }

    public Vector2 size
    {
        get
        {
            return new Vector2(_sizeX, _sizeY);
        }
    }

    //prefabed tilemap data and link hoookups
    public bool prefab = false;
    public int[] top;
    public int[] bot;
    public int[] left;
    public int[] right;

    public TileMap(int x, int y)
    {
        _sizeX = x;
        _sizeY = y;
        tiles = new Tile[x * y];
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
            return tiles[x + y * _sizeX];
        return null;
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
            tiles[x + y * _sizeX] = tile;
    }

    public void SetTile(int x, int y, Tile.TYPE type)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
        {
            if (tiles[x + y * _sizeX] != null)
            {
                tiles[x + y * _sizeX].type = type;
            }
            else
            {
                Debug.Log("Tile does not exist: " + x + ", " + y);
            }
        }
    }

    public int topSize
    {
        get
        {
            return top.Length;
        }
        set
        {
            if (value > _sizeX)
                value = _sizeX;
            if (value < 0)
                value = 0;
            int[] temp = new int[value];
            for (int i = 0; i < value && i < top.Length; i++)
                temp[i] = top[i];
            top = temp;
        }
    }

    public int botSize
    {
        get
        {
            return bot.Length;
        }
        set
        {
            if (value > _sizeX)
                value = _sizeX;
            if (value < 0)
                value = 0;
            int[] temp = new int[value];
            for (int i = 0; i < value && i < bot.Length; i++)
                temp[i] = bot[i];
            bot = temp;
        }
    }

    public int leftSize
    {
        get
        {
            return left.Length;
        }
        set
        {
            if (value > _sizeY)
                value = _sizeY;
            if (value < 0)
                value = 0;
            int[] temp = new int[value];
            for (int i = 0; i < value && i < left.Length; i++)
                temp[i] = left[i];
            left = temp;
        }
    }

    public int rightSize
    {
        get
        {
            return right.Length;
        }
        set
        {
            if (value > _sizeY)
                value = _sizeY;
            if (value < 0)
                value = 0;
            int[] temp = new int[value];
            for (int i = 0; i < value && i < right.Length; i++)
                temp[i] = right[i];
            right = temp;
        }
    }


    /// <summary>
    /// Percolates through tiles of the given types at the given coordinates
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="percType">Array of tile types.</param>
    /// <returns>Returns an unsorted array of the tiles that percolated.</returns>
    public Tile[] Percolate(int x, int y, Tile.TYPE[] percType)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY && percType != null)
        {
            List<Tile> island = new List<Tile>();
            return _Percolate(x, y, island, percType).ToArray();
        }
        return null;
    }

    //the private percolation function
    List<Tile> _Percolate(int x, int y, List<Tile> island, Tile.TYPE[] percType)
    {
        if (x < 0 || x >= _sizeX || y < 0 || y >= _sizeY)
            return island;
        if (!(Array.Exists(percType, tile => tile == tiles[x + y * _sizeX].type)))
            return island;
        if (!island.Contains(tiles[x + y * _sizeX]))
        {
            island.Add(tiles[x + y * _sizeX]);

            island = _Percolate(x + 1, y, island, percType);
            island = _Percolate(x - 1, y, island, percType);
            island = _Percolate(x, y + 1, island, percType);
            island = _Percolate(x, y - 1, island, percType);
        }
        return island;
    }

    /// <summary>
    /// Splits this tilemap into smaller ones of the given size
    /// </summary>
    /// <param name="x">length in X direction to split by.</param>
    /// <param name="y">length in Y direction to split by.</param>
    /// <returns>Returns an array of the smaller tilmaps.</returns>
    public TileMap[] Split(int x, int y)
    {
        if (x == 0 || y == 0)
            return new TileMap[1] { this };

        int numberX, numberY;
        numberX = (sizeX / x) + Mathf.Clamp(sizeX % x, 0, 1);
        numberY = (sizeY / y) + Mathf.Clamp(sizeY % y, 0, 1);

        TileMap[] maps = new TileMap[numberX * numberY];
        for(int j = 0; j < numberY; j++)
        {
            for(int i = 0; i < numberX; i++)
            {
                int width = x;
                if (i == numberX - 1 && sizeX % x != 0)
                    width = sizeX % x;
                int height = y;
                if (j == numberY - 1 && sizeY % y != 0)
                    height = sizeY % y;
                
                maps[j * numberX + i] = new TileMap(width, height);

                int adjX = x * i;
                int adjY = y * j;

                for (int Y = adjY; Y < adjY + y; Y++)
                {
                    for (int X = adjX; X < adjX + x; X++)
                    {
                        maps[j * numberX + i].SetTile(X - adjX, Y - adjY, GetTile(X, Y));
                    }
                }
            }
        }

        return maps;
    }

    /// <summary>
    /// Finds all of the linkages in a tilemap, based on the data from each tile
    /// </summary>
    /// <returns>Returns an array of the coordinates of each linkage tile.</returns>
    public Vector2[] GetLinkages()
    {
        List<Vector2> linkageList = new List<Vector2>();
        for(int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {
                if (GetTile(i, j).linkageTile)
                    linkageList.Add(new Vector2(i, j));
            }
        }

        return linkageList.ToArray();
    }

    public void ResetLinkages()
    {
        //set all to having to linkage property
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                GetTile(i, j).linkageTile = false;
            }
        }

        //set ones that should have linkage prperty
        for (int i = 0; i < top.Length; i++)
        {
            GetTile(top[i], sizeY - 1).linkageTile = true;
        }
        for (int i = 0; i < bot.Length; i++)
        {
            GetTile(bot[i], 0).linkageTile = true;
        }
        for (int i = 0; i < left.Length; i++)
        {
            GetTile(0, left[i]).linkageTile = true;
        }
        for (int i = 0; i < right.Length; i++)
        {
            GetTile(sizeX - 1, right[i]).linkageTile = true;
        }
    }

    [ContextMenu("Print Contents")]
    public void PrintContents()
    {
        for (int y = 0; y < _sizeY; y++)
        {
            for (int x = 0; x < _sizeX; x++)
            {
                Debug.Log(GetTile(x, y).ToUnityString());
            }
        }
    }
}