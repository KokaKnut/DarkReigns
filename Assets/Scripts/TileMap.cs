using System;
using System.Collections;
using System.Collections.Generic;

public class TileMap {

    Tile[,] _tiles;
    int _sizeX;
    int _sizeY;

    public int sizeX
    {
        get
        {
            return _sizeX;
        }
    }

    public int sizeY
    {
        get
        {
            return _sizeY;
        }
    }

    public TileMap(int x, int y)
    {
        _sizeX = x;
        _sizeY = y;
        _tiles = new Tile[x, y];
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
            return _tiles[x, y];
        return null;
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
            _tiles[x, y] = tile;
    }

    public List<Tile> Percolate(int x, int y)
    {
        if(x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
        {
            List<Tile> island = new List<Tile>();
            return _Percolate(x, y, island, new Tile.TYPE[] { Tile.TYPE.air });
        }
        return null;
    }

    public List<Tile> Percolate(int x, int y, Tile.TYPE[] percType)
    {
        if (x >= 0 && x < _sizeX && y >= 0 && y < _sizeY && percType != null)
        {
            List<Tile> island = new List<Tile>();
            return _Percolate(x, y, island, percType);
        }
        return null;
    }

    List<Tile> _Percolate(int x, int y, List<Tile> island, Tile.TYPE[] percType)
    {
        if (x < 0 || x >= _sizeX || y < 0 || y >= _sizeY)
            return island;
        for (int i = 0; i < percType.Length; i++)
        {
            if (_tiles[x, y].type != percType[i])
                return island;
        }
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