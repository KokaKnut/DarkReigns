using UnityEngine;
using System;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {

    Tile[,] _tiles;
    public int sizeX;
    public int sizeY;

    //prefabed tilemap data
    public bool prefab = false;
    public int[] top;
    public int[] bot;
    public int[] left;
    public int[] right;

    public void NewTileMap(int x, int y)
    {
        sizeX = x;
        sizeY = y;
        _tiles = new Tile[x, y];
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < sizeX && y >= 0 && y < sizeY)
            return _tiles[x, y];
        return null;
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < sizeX && y >= 0 && y < sizeY)
            _tiles[x, y] = tile;
    }

    public void SetTile(int x, int y, Tile.TYPE type)
    {
        if (x >= 0 && x < sizeX && y >= 0 && y < sizeY)
            _tiles[x, y].type = type;
    }

    public int topSize
    {
        get
        {
            return top.Length;
        }
        set
        {
            if (value > sizeX)
                value = sizeX;
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
            if (value > sizeX)
                value = sizeX;
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
            if (value > sizeY)
                value = sizeY;
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
            if (value > sizeY)
                value = sizeY;
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
        if (x >= 0 && x < sizeX && y >= 0 && y < sizeY && percType != null)
        {
            List<Tile> island = new List<Tile>();
            return _Percolate(x, y, island, percType).ToArray();
        }
        return null;
    }

    //the private percolation function
    List<Tile> _Percolate(int x, int y, List<Tile> island, Tile.TYPE[] percType)
    {
        if (x < 0 || x >= sizeX || y < 0 || y >= sizeY)
            return island;
        if (!(Array.Exists(percType, tile => tile == _tiles[x, y].type)))
            return island;
        if (!island.Contains(_tiles[x, y]))
        {
            island.Add(_tiles[x, y]);

            island = _Percolate(x + 1, y, island, percType);
            island = _Percolate(x - 1, y, island, percType);
            island = _Percolate(x, y + 1, island, percType);
            island = _Percolate(x, y - 1, island, percType);
        }
        return island;
    }
}