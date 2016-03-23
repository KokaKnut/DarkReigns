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

    public ArrayList Percolate(int x, int y)
    {
        if(x >= 0 && x < _sizeX && y >= 0 && y < _sizeY)
        {
            ArrayList island = new ArrayList();
            return _Percolate(x, y, island, new Tile.TYPE[] { Tile.TYPE.air });
        }
        return null;
    }
    
    ArrayList _Percolate(int x, int y, ArrayList island, Tile.TYPE[] percType)
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