using System.Collections;

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
            island.Add(new int[] { x, y });
        }
        return null;
    }

    ArrayList _Percolate(int x, int y, ArrayList island)
    {
        if (_tiles[x, y].type == Tile.TYPE.air)
            return island;
        if (island.Contains(new int[] { x, y }))
            _Percolate(x,y,island);
        return island;
    }
}