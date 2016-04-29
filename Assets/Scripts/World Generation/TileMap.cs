using UnityEngine;
using System;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {

    [SerializeField]
    Tile[] _tiles;

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

    //prefabed tilemap data
    public bool prefab = false;
    public int[] top;
    public int[] bot;
    public int[] left;
    public int[] right;

    public void NewTileMap(int x, int y)
    {
        _sizeX = x;
        _sizeY = y;
        _tiles = new Tile[x + y * _sizeX];
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
            return _tiles[x + y * _sizeX];
        return null;
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
            _tiles[x + y * _sizeX] = tile;
    }

    public void SetTile(int x, int y, Tile.TYPE type)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
        {
            if (_tiles[x + y * _sizeX] != null)
            {
                _tiles[x + y * _sizeX].type = type;
            }
            else
            {
                print("Tile does not exist: " + x + ", " + y);
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
        if (!(Array.Exists(percType, tile => tile == _tiles[x + y * _sizeX].type)))
            return island;
        if (!island.Contains(_tiles[x + y * _sizeX]))
        {
            island.Add(_tiles[x + y * _sizeX]);

            island = _Percolate(x + 1, y, island, percType);
            island = _Percolate(x - 1, y, island, percType);
            island = _Percolate(x, y + 1, island, percType);
            island = _Percolate(x, y - 1, island, percType);
        }
        return island;
    }

    [ContextMenu("Print Contents")]
    public void PrintContents()
    {
        for (int y = 0; y < _sizeY; y++)
        {
            for (int x = 0; x < _sizeX; x++)
            {
                GetTile(x, y).ToString();
            }
        }
    }
}